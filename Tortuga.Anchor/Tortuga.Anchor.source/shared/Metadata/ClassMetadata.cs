using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Tortuga.Anchor.Modeling;

#if !DataAnnotations_Missing
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations.Schema;
#endif

namespace Tortuga.Anchor.Metadata
{
    /// <summary>
    /// Cached Metadata about the indicated type.
    /// </summary>

    public partial class ClassMetadata
    {

        internal ClassMetadata(TypeInfo typeInfo)
        {
#if !DataAnnotations_Missing
            var table = (TableAttribute)typeInfo.GetCustomAttributes(typeof(TableAttribute), true).SingleOrDefault();
            if (table != null)
            {
                MappedTableName = table.Name;
                MappedSchemaName = table.Schema;
            }
#endif

#if Weird_Reflection
            var shadowingProperties = (from p in typeInfo.DeclaredProperties where IsHidingMember(p) select p).ToList();
            var propertyList = typeInfo.DeclaredProperties.ToList();
#elif TypeInfo_Is_Not_Type
            var type = typeInfo.AsType();
            var shadowingProperties = (from p in type.GetProperties() where IsHidingMember(p) select p).ToList();
            var propertyList = type.GetProperties(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
#else
            var shadowingProperties = (from p in typeInfo.GetProperties() where IsHidingMember(p) select p).ToList();
            var propertyList = typeInfo.GetProperties(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
#endif 

            Func<PropertyInfo, bool> IsHidden = propertyInfo => !shadowingProperties.Contains(propertyInfo) && shadowingProperties.Any(p => p.Name == propertyInfo.Name); 

            Properties = new PropertyMetadataCollection(propertyList.Where(p => !IsHidden(p)).Select(p => new PropertyMetadata(p)));


            //List the properties that are affected when the indicated property is modified.
            foreach (var property in Properties)
                foreach (CalculatedFieldAttribute fieldList in property.PropertyInfo.GetCustomAttributes(typeof(CalculatedFieldAttribute), true))
                    foreach (var field in fieldList.SourceProperties)
                    {
                        if (!Properties.Contains(field))
                            throw new InvalidOperationException(string.Format("Cannot find property {0} on type {1}. This is needed for the calculated property {2}", field, typeInfo.FullName, property.Name));

                        Properties[field].AddCalculatedField(property);
                    }

            foreach (var property in Properties)
                property.EndInit();

            Constructors = new ConstructorMetadataCollection(typeInfo.DeclaredConstructors);
        }

        /// <summary>
        /// Properties on the indicated class
        /// </summary>
        public PropertyMetadataCollection Properties { get; internal set; } 

        /// <summary>
        /// Properties on the indicated class
        /// </summary>
        public ConstructorMetadataCollection Constructors { get; }


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

#if !DataAnnotations_Missing

        private ImmutableArray<string> m_ColumnMap;

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
        /// Schema referred to by TableAttribute.
        /// </summary>
        public string MappedSchemaName { get; }

        /// <summary>
        /// Table referred to by TableAttribute.
        /// </summary>
        public string MappedTableName { get; }
#endif
    }





}
