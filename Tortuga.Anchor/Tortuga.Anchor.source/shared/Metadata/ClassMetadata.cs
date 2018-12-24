using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using Tortuga.Anchor.Modeling;

namespace Tortuga.Anchor.Metadata
{
    /// <summary>
    /// Cached Metadata about the indicated type.
    /// </summary>

    public partial class ClassMetadata
    {
        readonly TypeInfo m_TypeInfo;

        private ImmutableArray<string> m_ColumnMap;

        string m_CSharpFullName;

        internal ClassMetadata(TypeInfo typeInfo)
        {
            m_TypeInfo = typeInfo;

            var table = (TableAttribute)typeInfo.GetCustomAttributes(typeof(TableAttribute), true).SingleOrDefault();
            if (table != null)
            {
                MappedTableName = table.Name;
                MappedSchemaName = table.Schema;
            }

            var shadowingProperties = (from p in typeInfo.GetProperties() where IsHidingMember(p) select p).ToList();
            var propertyList = typeInfo.GetProperties(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            bool IsHidden(PropertyInfo propertyInfo) => !shadowingProperties.Contains(propertyInfo) && shadowingProperties.Any(p => p.Name == propertyInfo.Name);

            Properties = new PropertyMetadataCollection(propertyList.Where(p => !IsHidden(p)).Select(p => new PropertyMetadata(p)));

            //List the properties that are affected when the indicated property is modified.
            foreach (var property in Properties)
                foreach (CalculatedFieldAttribute fieldList in property.PropertyInfo.GetCustomAttributes(typeof(CalculatedFieldAttribute), true))
                    foreach (var field in fieldList.SourceProperties)
                    {
                        if (!Properties.Contains(field))
                            throw new InvalidOperationException($"Cannot find property {field} on type {typeInfo.FullName}. This is needed for the calculated property {property.Name}");

                        Properties[field].AddCalculatedField(property);
                    }

            foreach (var property in Properties)
                property.EndInit();

            Constructors = new ConstructorMetadataCollection(typeInfo.DeclaredConstructors);
        }

        /// <summary>
        /// Gets the database columns for this class.
        /// </summary>
        /// <remarks>This honors the Column, NotMapped, and Decompose attributes.</remarks>
        public ImmutableArray<string> ColumnsFor
        {
            get
            {
                if (m_ColumnMap == null)
                    m_ColumnMap = ImmutableArray.CreateRange(MetadataCache.GetColumnsFor(this, null));
                return m_ColumnMap;
            }
        }

        /// <summary>
        /// Properties on the indicated class
        /// </summary>
        public ConstructorMetadataCollection Constructors { get; }

        /// <summary>
        /// Gets the fully quantified name in C# format.
        /// </summary>
        public string CSharpFullName
        {
            get
            {
                if (m_CSharpFullName == null)
                {
                    var result = new StringBuilder(m_TypeInfo.ToString().Length);
                    BuildCSharpFullName(m_TypeInfo.AsType(), null, result);

                    m_CSharpFullName = result.ToString();
                }
                return m_CSharpFullName;
            }
        }

        /// <summary>
        /// Schema referred to by TableAttribute.
        /// </summary>
        public string MappedSchemaName { get; }

        /// <summary>
        /// Table referred to by TableAttribute.
        /// </summary>
        public string MappedTableName { get; }

        /// <summary>
        /// Properties on the indicated class
        /// </summary>
        public PropertyMetadataCollection Properties { get; internal set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return m_TypeInfo.ToString();
        }

        static void BuildCSharpFullName(Type typeInfo, List<Type> typeArgs, StringBuilder result)
        {
            var localTypeParamCount = typeInfo.GetTypeInfo().GenericTypeParameters.Length;
            var localTypeArgCount = typeInfo.GetTypeInfo().GenericTypeArguments.Length;

            if (typeArgs == null)
                typeArgs = new List<Type>(typeInfo.GetTypeInfo().GenericTypeArguments);

            if (typeInfo.IsNested)
            {
                BuildCSharpFullName(typeInfo.DeclaringType, typeArgs, result);
            }
            else
            {
                result.Append(typeInfo.Namespace);
            }

            result.Append(".");
            foreach (var c in typeInfo.Name)
            {
                if (c == '`') //we found a generic
                    break;
                result.Append(c);
            }

            if (localTypeParamCount > 0)
            {
                result.Append("<");

                for (int i = 0; i < localTypeParamCount; i++)
                {
                    if (i > 0)
                        result.Append(",");
                    BuildCSharpFullName(typeArgs[i], null, result); //note that we are "eating" the typeArgs that we passed to us from the nested type.
                }
                typeArgs.RemoveRange(0, localTypeParamCount); //remove the used args

                result.Append(">");
            }
            else if (localTypeArgCount > 0 && typeArgs.Count > 0)
            {
                result.Append("<");

                for (int i = 0; i < Math.Min(localTypeArgCount, typeArgs.Count); i++)
                {
                    if (i > 0)
                        result.Append(",");
                    BuildCSharpFullName(typeArgs[i], null, result);
                }
                result.Append(">");
            }
        }

        static bool IsHidingMember(PropertyInfo propertyInfo)
        {
            var baseType = propertyInfo.DeclaringType.GetTypeInfo().BaseType;
            if (baseType == null)
                return false;

            var baseProperty = baseType.GetRuntimeProperty(propertyInfo.Name);

            if (baseProperty == null)
                return false;

            if (baseProperty.DeclaringType == propertyInfo.DeclaringType)
                return false;

            if (baseProperty.GetMethod == null || propertyInfo.GetMethod == null)
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Cannot support classes with write-only properties. Class: {0} PropertyName {1}", propertyInfo.DeclaringType.Name, propertyInfo.Name));

            var basePropertyGetGetMethod = baseProperty.GetMethod;
            var propertyInfoGetGetMethod = propertyInfo.GetMethod;

            var baseMethodDefinition = basePropertyGetGetMethod.GetRuntimeBaseDefinition();
            var thisMethodDefinition = propertyInfoGetGetMethod.GetRuntimeBaseDefinition();

            return baseMethodDefinition.DeclaringType != thisMethodDefinition.DeclaringType;
        }
    }
}