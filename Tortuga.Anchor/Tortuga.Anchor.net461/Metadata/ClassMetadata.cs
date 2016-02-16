using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Tortuga.Anchor.Modeling;

namespace Tortuga.Anchor.Metadata
{
    /// <summary>
    /// Cached Metadata about the indicated type.
    /// </summary>

    public partial class ClassMetadata
    {
        private string m_MappedSchemaName;
        private string m_MappedTableName;

        internal ClassMetadata(Type type)
        {

            var table = (TableAttribute)type.GetCustomAttributes(typeof(TableAttribute), true).SingleOrDefault();
            if (table != null)
            {
                m_MappedTableName = table.Name;
                m_MappedSchemaName = table.Schema;
            }


            var shadowingProperties = (from p in type.GetProperties() where IsHidingMember(p) select p).ToList();
            var propertyList = type.GetProperties(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var propertyInfo in propertyList)
            {
                var isHidden = !shadowingProperties.Contains(propertyInfo) && shadowingProperties.Any(p => p.Name == propertyInfo.Name);

                if (isHidden)
                    continue;

                Properties.Add(new PropertyMetadata(propertyInfo));
            }

            //List the properties that are affected when the indicated property is modified.
            foreach (var property in Properties)
                foreach (CalculatedFieldAttribute fieldList in property.PropertyInfo.GetCustomAttributes(typeof(CalculatedFieldAttribute), true))
                    foreach (var field in fieldList.SourceProperties)
                    {
                        if (!Properties.Contains(field))
                            throw new InvalidOperationException(string.Format("Cannot find property {0} on type {1}. This is needed for the calculated property {2}", field, type.FullName, property.Name));

                        Properties[field].AddCalculatedField(property);
                    }

        }

        /// <summary>
        /// Schema referred to by TableAttribute.
        /// </summary>
        public string MappedSchemaName
        {
            get { return m_MappedSchemaName; }
        }

        /// <summary>
        /// Table referred to by TableAttribute.
        /// </summary>
        public string MappedTableName
        {
            get { return m_MappedTableName; }
        }

        /// <summary>
        /// Properties on the indicated class
        /// </summary>
        public PropertyMetadataCollection Properties { get; } = new PropertyMetadataCollection();

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
