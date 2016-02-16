using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Tortuga.Anchor.Modeling;

namespace Tortuga.Anchor.Metadata
{
    /// <summary>
    /// This is a cache of metadata about a specific property.
    /// </summary>
    public partial class PropertyMetadata
    {
        private readonly PropertyChangedEventArgs m_Args;
        private readonly List<PropertyMetadata> m_CalculatedFields = new List<PropertyMetadata>();
        private readonly MethodInfo m_GetMethod;
        private readonly bool m_IsIndexed;
        private readonly string m_Name;
        private readonly PropertyInfo m_PropertyInfo;
        private readonly Type m_PropertyType;
        private readonly MethodInfo m_SetMethod;
        private readonly bool m_Decompose;
        private readonly string m_DecompositionPrefix;
        private readonly bool m_IgnoreOnInsert;
        private readonly bool m_IgnoreOnUpdate;
        private readonly bool m_IsKey;
        private readonly string m_MappedColumnName;
        private readonly ReadOnlyCollection<ValidationAttribute> m_Validators;

        internal PropertyMetadata(PropertyInfo info)
        {
            m_PropertyInfo = info;

            var validators = new List<ValidationAttribute>();
            foreach (ValidationAttribute attribute in info.GetCustomAttributes(typeof(ValidationAttribute), true))
                validators.Add(attribute);
            m_Validators = new ReadOnlyCollection<ValidationAttribute>(validators);

            m_IsIndexed = info.GetIndexParameters().Length > 0;

            m_GetMethod = m_PropertyInfo.GetMethod;
            m_SetMethod = m_PropertyInfo.SetMethod;

            m_PropertyType = info.PropertyType;

            var name = info.ToString();
            m_Name = name.Substring(name.IndexOf(" ", StringComparison.Ordinal) + 1);

            if (IsIndexed)
                m_Args = new PropertyChangedEventArgs(info.Name + "[]");
            else
                m_Args = new PropertyChangedEventArgs(info.Name);

            m_IsKey = info.GetCustomAttributes(typeof(KeyAttribute), true).Any();

            var doNotMap = info.GetCustomAttributes(typeof(NotMappedAttribute), true).Any();
            if (!doNotMap)
            {
                var column = (ColumnAttribute)info.GetCustomAttributes(typeof(ColumnAttribute), true).SingleOrDefault();
                m_MappedColumnName = column != null ? column.Name : Name;
            }
            var decomposeAttribute = (DecomposeAttribute)(info.GetCustomAttributes(typeof(DecomposeAttribute), true).FirstOrDefault());
            if (decomposeAttribute != null)
            {
                m_Decompose = true;
                m_DecompositionPrefix = decomposeAttribute.Prefix;
            }
            m_IgnoreOnInsert = info.GetCustomAttributes(typeof(IgnoreOnInsertAttribute), true).Any();
            m_IgnoreOnUpdate = info.GetCustomAttributes(typeof(IgnoreOnUpdateAttribute), true).Any();

        }

        /// <summary>
        /// Returns true of this property needs to trigger updates to calculated fields
        /// </summary>
        public bool AffectsCalculatedFields
        {
            get { return m_CalculatedFields.Count > 0; }
        }

        /// <summary>
        /// This returns a list of calculated fields that need to be updated when this property is changed.
        /// </summary>
        public ReadOnlyCollection<PropertyMetadata> CalculatedFields
        {
            get { return new ReadOnlyCollection<PropertyMetadata>(m_CalculatedFields); }
        }

        /// <summary>
        /// Returns true if there is a public getter
        /// </summary>
        public bool CanRead
        {
            get { return m_GetMethod != null && m_GetMethod.IsPublic && !m_IsIndexed; }
        }

        /// <summary>
        /// Returns true is there is a public setter
        /// </summary>
        public bool CanWrite
        {
            get { return m_SetMethod != null && m_SetMethod.IsPublic && !m_IsIndexed; }
        }

        /// <summary>
        /// Gets a value indicating whether to map this object's columns to the child object's properties.
        /// </summary>
        public bool Decompose
        {
            get { return m_Decompose; }
        }

        /// <summary>
        /// Gets the decomposition prefix.
        /// </summary>
        /// <value>The decomposition prefix.</value>
        public string DecompositionPrefix
        {
            get { return m_DecompositionPrefix; }
        }

        /// <summary>
        /// Gets a value indicating whether to ignore this property during insert operations.
        /// </summary>
        public bool IgnoreOnInsert
        {
            get { return m_IgnoreOnInsert; }
        }

        /// <summary>
        /// Gets a value indicating whether to ignore this property during update operations.
        /// </summary>
        public bool IgnoreOnUpdate
        {
            get { return m_IgnoreOnUpdate; }
        }

        /// <summary>
        /// Returns true if this represents an indexed property
        /// </summary>
        public bool IsIndexed
        {
            get { return m_IsIndexed; }
        }

        /// <summary>
        /// Property implements the Key attribute.
        /// </summary>
        public bool IsKey
        {
            get { return m_IsKey; }
        }

        /// <summary>
        /// Column that this attribute is mapped to. Defaults to the property's name, but may be overridden by ColumnAttribute.
        /// </summary>
        public string MappedColumnName
        {
            get { return m_MappedColumnName; }
        }

        /// <summary>
        /// Public name of the property
        /// </summary>
        public string Name
        {
            get { return m_Name; }
        }

        /// <summary>
        /// Gets a cached instance of PropertyChangedEventArgs
        /// </summary>
        /// <remarks>For indexed properties such as "Item [Int32]" the property name will be reduced to "Item[]" to match ObservableCollection.</remarks>
        public PropertyChangedEventArgs PropertyChangedEventArgs
        {
            get { return m_Args; }
        }

        /// <summary>
        /// Gets the type of this property.
        /// </summary>
        public Type PropertyType
        {
            get { return m_PropertyType; }
        }

        /// <summary>
        /// List of validators that apply to the property
        /// </summary>
        public ReadOnlyCollection<ValidationAttribute> Validators
        {
            get { return m_Validators; }
        }

        /// <summary>
        /// Cached PropertyInfo for the property.
        /// </summary>
        internal PropertyInfo PropertyInfo
        {
            get { return m_PropertyInfo; }
        }

        /// <summary>
        /// Invokes this property's getter on the supplied object
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "CanRead")]
        public object InvokeGet(object target)
        {
            if (CanRead)
                try
                {
                    return m_GetMethod.Invoke(target, null);
                }
                catch (ArgumentException ex)
                {
                    throw new ArgumentException("Error getting property " + Name, ex);
                }
            else
                throw new InvalidOperationException($"CanRead is false on property {Name}.");
        }

        /// <summary>
        /// Invokes this property's setter on the supplied object
        /// </summary>
        /// <param name="target"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "CanWrite")]
        public object InvokeSet(object target, object value)
        {
            if (CanWrite)
                try
                {
                    return m_SetMethod.Invoke(target, new object[] { value });
                }
                catch (ArgumentException ex)
                {
                    throw new ArgumentException("Error setting property " + Name, ex);
                }
            else
                throw new InvalidOperationException($"CanWrite is false for property {Name}");
        }

        /// <summary>
        /// Adds a property to the list of calculated values watching this property.
        /// </summary>
        /// <param name="affectedProperty"></param>
        internal void AddCalculatedField(PropertyMetadata affectedProperty)
        {
            m_CalculatedFields.Add(affectedProperty);
        }
    }

}
