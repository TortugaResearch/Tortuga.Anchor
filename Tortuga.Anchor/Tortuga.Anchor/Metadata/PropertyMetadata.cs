using System.Collections.Immutable;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using Tortuga.Anchor.Modeling;

namespace Tortuga.Anchor.Metadata;

#if NETCOREAPP
#pragma warning disable IDE0057 // Use range operator
#endif

/// <summary>
/// This is a cache of metadata about a specific property.
/// </summary>
public partial class PropertyMetadata
{
	readonly List<PropertyMetadata> m_CalculatedFieldsBuilder = new();
	readonly MethodInfo? m_GetMethod;
	readonly MethodInfo? m_SetMethod;

	internal PropertyMetadata(PropertyInfo info, int propertyIndex)
	{
		PropertyInfo = info;
		PropertyIndex = propertyIndex;

		Attributes = ImmutableArray.CreateRange(Attribute.GetCustomAttributes(PropertyInfo, true));

		Validators = ImmutableArray.CreateRange(Attributes.OfType<ValidationAttribute>());

		IsIndexed = info.GetIndexParameters().Length > 0;

		m_GetMethod = PropertyInfo.GetMethod;
		m_SetMethod = PropertyInfo.SetMethod;

		PropertyType = info.PropertyType;

		var name = info.ToString()!;
		Name = name.Substring(name.IndexOf(" ", StringComparison.Ordinal) + 1);

		if (IsIndexed)
			PropertyChangedEventArgs = new PropertyChangedEventArgs(info.Name + "[]");
		else
			PropertyChangedEventArgs = new PropertyChangedEventArgs(info.Name);

		if (IsIndexed)
			PropertyChangingEventArgs = new PropertyChangingEventArgs(info.Name + "[]");
		else
			PropertyChangingEventArgs = new PropertyChangingEventArgs(info.Name);

		IsKey = Attributes.OfType<KeyAttribute>().Any();

		var doNotMap = Attributes.OfType<NotMappedAttribute>().Any();
		if (!doNotMap)
		{
			var column = Attributes.OfType<ColumnAttribute>().SingleOrDefault();
			MappedColumnName = (column != null && !column.Name.IsNullOrEmpty()) ? column.Name : Name;
		}
		var decomposeAttribute = Attributes.OfType<DecomposeAttribute>().SingleOrDefault();
		if (decomposeAttribute != null)
		{
			Decompose = true;
			DecompositionPrefix = decomposeAttribute.Prefix;
		}
		IgnoreOnInsert = Attributes.OfType<IgnoreOnInsertAttribute>().Any();
		IgnoreOnUpdate = Attributes.OfType<IgnoreOnUpdateAttribute>().Any();

		if (!PropertyType.IsValueType)
		{
			var nullableAttribute = Attributes.Where(a => a.GetType().FullName == "System.Runtime.CompilerServices.NullableAttribute").FirstOrDefault();

			if (nullableAttribute != null)
			{
				byte[] bytes = (byte[])nullableAttribute.GetType().GetField("NullableFlags")!.GetValue(nullableAttribute)!;

				if (bytes.Length >= 1)
					IsReferenceNullable = bytes[0] switch
					{
						0 => (bool?)null,
						1 => false,
						2 => true,
						_ => null
					};

				NullabilityFlags = ImmutableArray.Create<byte>(bytes);
			}
		}
	}

	/// <summary>
	/// Returns true of this property needs to trigger updates to calculated fields
	/// </summary>
	public bool AffectsCalculatedFields => m_CalculatedFieldsBuilder.Count > 0;

	/// <summary>
	/// This returns a list of calculated fields that need to be updated when this property is changed.
	/// </summary>
	public ImmutableArray<PropertyMetadata> CalculatedFields { get; private set; }

	/// <summary>
	/// Returns true if there is a public getter and it is not an indexed property.
	/// </summary>
	public bool CanRead
	{
		get { return m_GetMethod?.IsPublic == true && m_GetMethod?.IsFamily == false && !IsIndexed; }
	}

	/// <summary>
	/// Returns true if there is a getter and it is not an indexed property, even if that getter isn't public.
	/// </summary>
	public bool CanReadRestricted
	{
		get { return m_GetMethod != null && !IsIndexed; }
	}

	/// <summary>
	/// Returns true if there is a public getter and it is an indexed property.
	/// </summary>
	public bool CanReadIndexed
	{
		get { return m_GetMethod?.IsPublic == true && m_GetMethod?.IsFamily == false && IsIndexed; }
	}

	/// <summary>
	/// Returns true if there is a getter and it is an indexed property, even if that getter isn't public.
	/// </summary>
	public bool CanReadIndexedAndRestricted
	{
		get { return m_GetMethod != null && IsIndexed; }
	}

	/// <summary>
	/// Returns true is there is a public setter and it is not an indexed property.
	/// </summary>
	public bool CanWrite
	{
		get { return m_SetMethod?.IsPublic == true && m_SetMethod?.IsFamily == false && !IsIndexed; }
	}

	/// <summary>
	/// Returns true is there is a setter and it is not an indexed property, even if that setter isn't public.
	/// </summary>
	public bool CanWriteRestricted
	{
		get { return m_SetMethod != null && !IsIndexed; }
	}

	/// <summary>
	/// Returns true is there is a public setter and it is an indexed property
	/// </summary>
	public bool CanWriteIndexed
	{
		get { return m_SetMethod?.IsPublic == true && m_SetMethod?.IsFamily == false && IsIndexed; }
	}

	/// <summary>
	/// Returns true is there is a setter and and it is an indexed property, even if that setter isn't public.
	/// </summary>
	public bool CanWriteIndexedAndRestricted
	{
		get { return m_SetMethod != null && !IsIndexed; }
	}

	/// <summary>
	/// Gets a value indicating whether to map this object's columns to the child object's properties.
	/// </summary>
	public bool Decompose { get; }

	/// <summary>
	/// Gets the decomposition prefix.
	/// </summary>
	/// <value>The decomposition prefix.</value>
	public string? DecompositionPrefix { get; }

	/// <summary>
	/// Gets a value indicating whether to ignore this property during insert operations.
	/// </summary>
	public bool IgnoreOnInsert { get; }

	/// <summary>
	/// Gets a value indicating whether to ignore this property during update operations.
	/// </summary>
	public bool IgnoreOnUpdate { get; }

	/// <summary>
	/// Returns true if this represents an indexed property
	/// </summary>
	public bool IsIndexed { get; }

	/// <summary>
	/// Property implements the Key attribute.
	/// </summary>
	public bool IsKey { get; }

	/// <summary>
	/// Gets a value indicating whether this is nullable reference type.
	/// </summary>
	/// <value><c>null</c> if null agnostic, <c>true</c> if nullable; <c>false</c> is non-nullable.</value>
	public bool? IsReferenceNullable { get; }

	/// <summary>
	/// Column that this attribute is mapped to. Defaults to the property's name, but may be overridden by ColumnAttribute.
	/// </summary>
	public string? MappedColumnName { get; }

	/// <summary>
	/// Public name of the property
	/// </summary>
	public string Name { get; }

	/// <summary>
	/// Gets the nullability flags from the property's NullableAttribute.
	/// </summary>
	/// <value>The nullability flags.</value>
	/// <remarks>See https://github.com/dotnet/roslyn/blob/master/docs/features/nullable-metadata.md for more information.</remarks>
	public ImmutableArray<byte> NullabilityFlags { get; }

	/// <summary>
	/// Gets a cached instance of PropertyChangedEventArgs
	/// </summary>
	/// <remarks>For indexed properties such as "Item [Int32]" the property name will be reduced to "Item[]" to match ObservableCollection.</remarks>
	public PropertyChangedEventArgs PropertyChangedEventArgs { get; }

	/// <summary>
	/// Gets a cached instance of PropertyChangingEventArgs
	/// </summary>
	/// <remarks>For indexed properties such as "Item [Int32]" the property name will be reduced to "Item[]" to match ObservableCollection.</remarks>
	public PropertyChangingEventArgs PropertyChangingEventArgs { get; }

	/// <summary>
	/// Gets the type of this property.
	/// </summary>
	public Type PropertyType { get; }

	/// <summary>
	/// List of validators that apply to the property
	/// </summary>
	public ImmutableArray<ValidationAttribute> Validators { get; }

	/// <summary>
	/// Complete list of attributes that apply to the property
	/// </summary>
	public ImmutableArray<Attribute> Attributes { get; }

	/// <summary>
	/// Cached PropertyInfo for the property.
	/// </summary>
	public PropertyInfo PropertyInfo { get; }

	/// <summary>Creates the delegate setter.</summary>
	/// <typeparam name="TTarget">The type of the target object.</typeparam>
	/// <typeparam name="TProperty">The type of the property.</typeparam>
	/// <returns>Func&lt;TTarget, TProperty&gt;.</returns>
	/// <exception cref="System.InvalidOperationException">CanRead is false for property {Name}</exception>
	/// <remarks>This is much faster than calling InvokeGet, but you must provide the correct TTarget and  TProperty arguments.</remarks>
	public Func<TTarget, TProperty> CreateDelegateGetter<TTarget, TProperty>()
	{
		if (CanRead)
		{
			return (Func<TTarget, TProperty>)Delegate.CreateDelegate(typeof(Func<TTarget, TProperty>), m_GetMethod!);
		}
		else
			throw new InvalidOperationException($"CanRead is false for property {Name}");
	}

	/// <summary>Creates the delegate setter for an indexed property.</summary>
	/// <typeparam name="TTarget">The type of the target object.</typeparam>
	/// <typeparam name="TIndex">The type of the index parameter.</typeparam>
	/// <typeparam name="TProperty">The type of the property.</typeparam>
	/// <returns>Func&lt;TTarget, TProperty&gt;.</returns>
	/// <exception cref="System.InvalidOperationException">CanReadIndexed is false for property {Name}</exception>
	/// <remarks>This is much faster than calling InvokeGet, but you must provide the correct TTarget and  TProperty arguments.</remarks>
	public Func<TTarget, TIndex, TProperty> CreateDelegateGetter<TTarget, TIndex, TProperty>()
	{
		if (CanReadIndexed)
		{
			return (Func<TTarget, TIndex, TProperty>)Delegate.CreateDelegate(typeof(Func<TTarget, TIndex, TProperty>), m_GetMethod!);
		}
		else
			throw new InvalidOperationException($"CanReadIndexed is false for property {Name}");
	}

	/// <summary>Creates the delegate setter.</summary>
	/// <typeparam name="TTarget">The type of the target object.</typeparam>
	/// <typeparam name="TProperty">The type of the property.</typeparam>
	/// <returns>Action&lt;TTarget, TProperty&gt;.</returns>
	/// <exception cref="System.InvalidOperationException">CanWrite is false for property {Name}</exception>
	/// <remarks>This is much faster than calling InvokeSet, but you must provide the correct TTarget and  TProperty arguments.</remarks>
	public Action<TTarget, TProperty> CreateDelegateSetter<TTarget, TProperty>()
	{
		if (CanWrite)
		{
			return (Action<TTarget, TProperty>)Delegate.CreateDelegate(typeof(Action<TTarget, TProperty>), m_SetMethod!);
		}
		else
			throw new InvalidOperationException($"CanWrite is false for property {Name}");
	}

	/// <summary>Creates the delegate setter for an indexed property.</summary>
	/// <typeparam name="TTarget">The type of the target object.</typeparam>
	/// <typeparam name="TIndex">The type of the index parameter.</typeparam>
	/// <typeparam name="TProperty">The type of the property.</typeparam>
	/// <returns>Action&lt;TTarget, TProperty&gt;.</returns>
	/// <exception cref="System.InvalidOperationException">CanWriteIndexed is false for property {Name}</exception>
	/// <remarks>This is much faster than calling InvokeSet, but you must provide the correct TTarget and  TProperty arguments.</remarks>
	public Action<TTarget, TIndex, TProperty> CreateDelegateSetter<TTarget, TIndex, TProperty>()
	{
		if (CanWriteIndexed)
		{
			return (Action<TTarget, TIndex, TProperty>)Delegate.CreateDelegate(typeof(Action<TTarget, TIndex, TProperty>), m_SetMethod!);
		}
		else
			throw new InvalidOperationException($"CanWriteIndexed is false for property {Name}");
	}

	/// <summary>
	/// Invokes this property's getter on the supplied object
	/// </summary>
	/// <param name="target">The target.</param>
	/// <returns>System.Object.</returns>
	/// <exception cref="ArgumentException">Error getting property " + Name</exception>
	/// <exception cref="InvalidOperationException">CanRead and CanReadRestricted is false on property {Name}.</exception>
	public object? InvokeGet(object target)
	{
		if (CanRead || CanReadRestricted)
		{
			try
			{
				return m_GetMethod!.Invoke(target, null);
			}
			catch (ArgumentException ex)
			{
				throw new ArgumentException("Error getting property " + Name, ex);
			}
		}
		else
			throw new InvalidOperationException($"CanRead is false on property {Name}.");
	}

	/// <summary>
	/// Invokes this property's getter on the supplied object
	/// </summary>
	/// <param name="target">The target.</param>
	/// <param name="index">The index.</param>
	/// <returns>System.Object.</returns>
	/// <exception cref="System.ArgumentException">Error getting property " + Name</exception>
	/// <exception cref="System.InvalidOperationException">CanReadIndexed and CanReadIndexedRestrictedis false on property {Name}.</exception>
	public object? InvokeGet(object target, object? index)
	{
		if (CanReadIndexed || CanReadIndexedAndRestricted)
		{
			try
			{
				return m_GetMethod!.Invoke(target, new object?[] { index });
			}
			catch (ArgumentException ex)
			{
				throw new ArgumentException("Error getting property " + Name, ex);
			}
		}
		else
			throw new InvalidOperationException($"CanReadIndexed is false on property {Name}.");
	}

	/// <summary>
	/// Invokes this property's setter on the supplied object
	/// </summary>
	/// <param name="target">The target.</param>
	/// <param name="value">The value.</param>
	/// <returns>System.Object.</returns>
	/// <exception cref="ArgumentException">Error setting property " + Name</exception>
	/// <exception cref="InvalidOperationException">CanWrite and CanWriteRestricted is false for property {Name}</exception>
	public void InvokeSet(object target, object? value)
	{
		if (CanWrite || CanWriteRestricted)
		{
			try
			{
				m_SetMethod!.Invoke(target, new object?[] { value });
			}
			catch (ArgumentException ex)
			{
				throw new ArgumentException("Error setting property " + Name, ex);
			}
		}
		else
			throw new InvalidOperationException($"CanWrite is false for property {Name}");
	}

	/// <summary>
	/// Invokes this property's setter on the supplied object
	/// </summary>
	/// <param name="target">The target.</param>
	/// <param name="index">The index.</param>
	/// <param name="value">The value.</param>
	/// <returns>System.Object.</returns>
	/// <exception cref="System.ArgumentException">Error setting property " + Name</exception>
	/// <exception cref="System.InvalidOperationException">CanWriteIndexed and CanWriteIndexedRestricted is false for property {Name}</exception>
	public void InvokeSet(object target, object? index, object? value)
	{
		if (CanWriteIndexed || CanWriteIndexedAndRestricted)
		{
			try
			{
				m_SetMethod!.Invoke(target, new object?[] { index, value });
			}
			catch (ArgumentException ex)
			{
				throw new ArgumentException("Error setting property " + Name, ex);
			}
		}
		else
			throw new InvalidOperationException($"CanWriteIndexed is false for property {Name}");
	}

	/// <summary>
	/// Adds a property to the list of calculated values watching this property.
	/// </summary>
	/// <param name="affectedProperty"></param>
	/// <remarks>This must not be called after EndInit is called.</remarks>
	internal void AddCalculatedField(PropertyMetadata affectedProperty)
	{
		if (CalculatedFields != null)
			throw new InvalidOperationException("EndInit was previosuly called. No further changes are allowed.");

		m_CalculatedFieldsBuilder.Add(affectedProperty);
	}

	/// <summary>
	/// This freezes the object, preventing further changes.
	/// </summary>
	internal void EndInit()
	{
		CalculatedFields = ImmutableArray.CreateRange(m_CalculatedFieldsBuilder);
	}

	/// <summary>
	/// Gets or sets the index of the property.
	/// </summary>
	/// <value>The index of the property.</value>
	/// <remarks>Used by property bags and other things that need to store property values in arrays.</remarks>
	public int PropertyIndex { get; }
}
