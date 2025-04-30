using System.Collections;
using System.ComponentModel;
using System.Dynamic;

namespace Tortuga.Anchor.Dynamic;

#pragma warning disable CA1710 // Identifiers should have correct suffix

/// <summary>
/// The CultureAwareDynamicObject class can support any StringComparer. The default is OrdinalIgnoreCase.
/// Implements the <see cref="DynamicObject" />
/// Implements the <see cref="IDictionary{String, Object}" />
/// Implements the <see cref="INotifyPropertyChanged" />
/// </summary>
/// <seealso cref="DynamicObject" />
/// <seealso cref="IDictionary{String, Object}" />
/// <seealso cref="INotifyPropertyChanged" />
public sealed class CultureAwareDynamicObject : DynamicObject, IDictionary<string, object?>, INotifyPropertyChanged

{
	/// <summary>
	/// The m dictionary
	/// </summary>
	readonly Dictionary<string, object?> m_Dictionary;

	/// <summary>
	/// Initializes a new instance of the <see cref="CultureAwareDynamicObject" /> class.
	/// </summary>
	/// <param name="comparer">The comparer.</param>
	public CultureAwareDynamicObject(StringComparer comparer)
	{
		m_Dictionary = new(comparer);
	}

	/// <summary>
	/// Provides implementation for type conversion operations. Classes derived from the <see cref="DynamicObject" /> class can override this method to specify dynamic behavior for operations that convert an object from one type to another.
	/// </summary>
	/// <param name="binder">Provides information about the conversion operation. The binder.Type property provides the type to which the object must be converted. For example, for the statement (String)sampleObject in C# (CType(sampleObject, Type) in Visual Basic), where sampleObject is an instance of the class derived from the <see cref="DynamicObject" /> class, binder.Type returns the <see cref="String" /> type. The binder.Explicit property provides information about the kind of conversion that occurs. It returns true for explicit conversion and false for implicit conversion.</param>
	/// <param name="result">The result of the type conversion operation.</param>
	/// <returns>true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.)</returns>
	public override bool TryConvert(ConvertBinder binder, out object? result)
	{
		return base.TryConvert(binder, out result);
	}

	/// <summary>
	/// Gets or sets the <see cref="Nullable{Object}"/> with the specified key.
	/// </summary>
	/// <param name="key">The key.</param>
	/// <returns>System.Nullable&lt;System.Object&gt;.</returns>
	public object? this[string key]
	{
		get => ((IDictionary<string, object?>)m_Dictionary)[key];
		set
		{
			((IDictionary<string, object?>)m_Dictionary)[key] = value;
			OnPropertyChanged(key);
		}
	}

	/// <summary>
	/// Gets an <see cref="ICollection{String}" /> containing the keys of the <see cref="IDictionary{String, Object}" />.
	/// </summary>
	/// <value>The keys.</value>
	public ICollection<string> Keys => ((IDictionary<string, object?>)m_Dictionary).Keys;

	/// <summary>
	/// Gets an <see cref="ICollection{Object}" /> containing the values in the <see cref="IDictionary{String, Object}" />.
	/// </summary>
	/// <value>The values.</value>
	public ICollection<object?> Values => ((IDictionary<string, object?>)m_Dictionary).Values;

	/// <summary>
	/// Gets the number of elements contained in the <see cref="ICollection{T}" />.
	/// </summary>
	/// <value>The count.</value>
	public int Count => ((ICollection<KeyValuePair<string, object?>>)m_Dictionary).Count;

	/// <summary>
	/// Gets a value indicating whether the <see cref="ICollection{T}" /> is read-only.
	/// </summary>
	/// <value><c>true</c> if this instance is read only; otherwise, <c>false</c>.</value>
	public bool IsReadOnly => false;

	/// <summary>
	/// Occurs when a property value changes.
	/// </summary>
	public event PropertyChangedEventHandler? PropertyChanged;

	/// <summary>
	/// Adds an element with the provided key and value to the <see cref="IDictionary{String, Object}" />.
	/// </summary>
	/// <param name="key">The object to use as the key of the element to add.</param>
	/// <param name="value">The object to use as the value of the element to add.</param>
	public void Add(string key, object? value)
	{
		((IDictionary<string, object?>)m_Dictionary).Add(key, value);
		OnPropertyChanged(key);
	}

	/// <summary>
	/// Adds an item to the <see cref="ICollection{T}" />.
	/// </summary>
	/// <param name="item">The object to add to the <see cref="ICollection{T}" />.</param>
	public void Add(KeyValuePair<string, object?> item)
	{
		((ICollection<KeyValuePair<string, object?>>)m_Dictionary).Add(item);
		OnPropertyChanged(item.Key);
	}

	/// <summary>
	/// Removes all items from the <see cref="ICollection{T}" />.
	/// </summary>
	public void Clear()
	{
		((ICollection<KeyValuePair<string, object?>>)m_Dictionary).Clear();
		OnPropertyChanged(null);
	}

	/// <summary>
	/// Determines whether the <see cref="ICollection{T}" /> contains a specific value.
	/// </summary>
	/// <param name="item">The object to locate in the <see cref="ICollection{T}" />.</param>
	/// <returns>true if <paramref name="item" /> is found in the <see cref="ICollection{T}" />; otherwise, false.</returns>
	public bool Contains(KeyValuePair<string, object?> item)
	{
		return ((ICollection<KeyValuePair<string, object?>>)m_Dictionary).Contains(item);
	}

	/// <summary>
	/// Determines whether the <see cref="IDictionary{String, Object}" /> contains an element with the specified key.
	/// </summary>
	/// <param name="key">The key to locate in the <see cref="IDictionary{String, Object}" />.</param>
	/// <returns>true if the <see cref="IDictionary{String, Object}" /> contains an element with the key; otherwise, false.</returns>
	public bool ContainsKey(string key)
	{
		return ((IDictionary<string, object?>)m_Dictionary).ContainsKey(key);
	}

	/// <summary>
	/// Copies the elements of the <see cref="ICollection{T}" /> to an <see cref="Array" />, starting at a particular <see cref="Array" /> index.
	/// </summary>
	/// <param name="array">The one-dimensional <see cref="Array" /> that is the destination of the elements copied from <see cref="ICollection{T}" />. The <see cref="Array" /> must have zero-based indexing.</param>
	/// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
	public void CopyTo(KeyValuePair<string, object?>[] array, int arrayIndex)
	{
		((ICollection<KeyValuePair<string, object?>>)m_Dictionary).CopyTo(array, arrayIndex);
	}

	/// <summary>
	/// Returns an enumerator that iterates through the collection.
	/// </summary>
	/// <returns>An enumerator that can be used to iterate through the collection.</returns>
	public IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
	{
		return ((IEnumerable<KeyValuePair<string, object?>>)m_Dictionary).GetEnumerator();
	}

	/// <summary>
	/// Removes the element with the specified key from the <see cref="IDictionary{String, Object}" />.
	/// </summary>
	/// <param name="key">The key of the element to remove.</param>
	/// <returns>true if the element is successfully removed; otherwise, false.  This method also returns false if <paramref name="key" /> was not found in the original <see cref="IDictionary{String, Object}" />.</returns>
	public bool Remove(string key)
	{
		bool result = ((IDictionary<string, object?>)m_Dictionary).Remove(key);
		OnPropertyChanged(null);
		return result;
	}

	/// <summary>
	/// Removes the first occurrence of a specific object from the <see cref="ICollection{T}" />.
	/// </summary>
	/// <param name="item">The object to remove from the <see cref="ICollection{T}" />.</param>
	/// <returns>true if <paramref name="item" /> was successfully removed from the <see cref="ICollection{T}" />; otherwise, false. This method also returns false if <paramref name="item" /> is not found in the original <see cref="ICollection{T}" />.</returns>
	public bool Remove(KeyValuePair<string, object?> item)
	{
		bool result = ((ICollection<KeyValuePair<string, object?>>)m_Dictionary).Remove(item);
		OnPropertyChanged(null);
		return result;
	}

	/// <summary>
	/// Provides the implementation for operations that get member values. Classes derived from the <see cref="DynamicObject" /> class can override this method to specify dynamic behavior for operations such as getting a value for a property.
	/// </summary>
	/// <param name="binder">Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member on which the dynamic operation is performed. For example, for the Console.WriteLine(sampleObject.SampleProperty) statement, where sampleObject is an instance of the class derived from the <see cref="DynamicObject" /> class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param>
	/// <param name="result">The result of the get operation. For example, if the method is called for a property, you can assign the property value to <paramref name="result" />.</param>
	/// <returns>true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a run-time exception is thrown.)</returns>
	/// <exception cref="System.ArgumentNullException">binder</exception>
	public override bool TryGetMember(GetMemberBinder binder, out object? result)
	{
		if (binder == null)
			throw new ArgumentNullException(nameof(binder), $"{nameof(binder)} is null.");

		return m_Dictionary.TryGetValue(binder.Name, out result);
	}

	/// <summary>
	/// Gets the value associated with the specified key.
	/// </summary>
	/// <param name="key">The key whose value to get.</param>
	/// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value" /> parameter. This parameter is passed uninitialized.</param>
	/// <returns>true if the object that implements <see cref="IDictionary{String, Object}" /> contains an element with the specified key; otherwise, false.</returns>
	public bool TryGetValue(string key, out object? value)
	{
		return ((IDictionary<string, object?>)m_Dictionary).TryGetValue(key, out value);
	}

	/// <summary>
	/// Provides the implementation for operations that set member values. Classes derived from the <see cref="DynamicObject" /> class can override this method to specify dynamic behavior for operations such as setting a value for a property.
	/// </summary>
	/// <param name="binder">Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member to which the value is being assigned. For example, for the statement sampleObject.SampleProperty = "Test", where sampleObject is an instance of the class derived from the <see cref="DynamicObject" /> class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param>
	/// <param name="value">The value to set to the member. For example, for sampleObject.SampleProperty = "Test", where sampleObject is an instance of the class derived from the <see cref="DynamicObject" /> class, the <paramref name="value" /> is "Test".</param>
	/// <returns>true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.)</returns>
	/// <exception cref="System.ArgumentNullException">binder</exception>
	public override bool TrySetMember(SetMemberBinder binder, object? value)
	{
		if (binder == null)
			throw new ArgumentNullException(nameof(binder), $"{nameof(binder)} is null.");

		m_Dictionary[binder.Name] = value;
		OnPropertyChanged(binder.Name);
		return true;
	}

	/// <summary>
	/// Returns an enumerator that iterates through a collection.
	/// </summary>
	/// <returns>An <see cref="IEnumerator" /> object that can be used to iterate through the collection.</returns>
	IEnumerator IEnumerable.GetEnumerator()
	{
		return ((IEnumerable)m_Dictionary).GetEnumerator();
	}

	void OnPropertyChanged(string? key)
	{
		if (PropertyChanged != null)
			PropertyChanged(this, new PropertyChangedEventArgs(key));
	}
}
