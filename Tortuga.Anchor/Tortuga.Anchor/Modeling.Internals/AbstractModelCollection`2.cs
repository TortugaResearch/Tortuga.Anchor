using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Tortuga.Anchor.Collections;
using Tortuga.Anchor.ComponentModel;
using Tortuga.Anchor.DataAnnotations;

namespace Tortuga.Anchor.Modeling.Internals;

/// <summary>
/// A base class for collections of models and entities. This is not meant to be used directly by client code.
/// </summary>
/// <typeparam name="T">The type of object being stored</typeparam>
/// <typeparam name="TPropertyTracking">The type of property tracking desired.</typeparam>

[DataContract(Namespace = "http://github.com/docevaad/Anchor")]
public abstract partial class AbstractModelCollection<T, TPropertyTracking> : ObservableCollectionExtended<T>, INotifyDataErrorInfo, IValidatable, IDataErrorInfo
	where TPropertyTracking : PropertyBagBase
{
	/// <summary>
	/// Raised when the the errors collection has changed.
	/// </summary>
	/// <remarks>
	/// This may be fired even when no actual change has occurred.
	/// </remarks>
	public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[OnDeserializing]
	void OnDeserializing(StreamingContext context)
	{
		Properties = (TPropertyTracking)Activator.CreateInstance(typeof(TPropertyTracking), this)!;
		Properties.PropertyChanged += Properties_PropertyChanged;
		Properties.RevalidateProperty += (s, e) => ValidateProperty(e.PropertyName);
		Properties.RevalidateObject += (s, e) => ValidateObject();
	}

	/// <summary>
	/// Creates a model by auto-constructing the property bag defined by TPropertyTracking.
	/// </summary>
	/// <remarks>
	/// Requires TPropertyTracking have a public constructor that accepts an Object
	/// </remarks>
	protected AbstractModelCollection()
	{
		Properties = (TPropertyTracking)Activator.CreateInstance(typeof(TPropertyTracking), this)!;
		Properties.PropertyChanged += Properties_PropertyChanged;
		Properties.RevalidateProperty += (s, e) => ValidateProperty(e.PropertyName);
		Properties.RevalidateObject += (s, e) => ValidateObject();
	}

	/// <summary>
	/// Creates a model by auto-constructing the property bag defined by TPropertyTracking and populates it using the supplied list
	/// </summary>
	/// <param name="list">The list from which the elements are copied.</param>
	protected AbstractModelCollection(List<T> list)
		: base(list)
	{
		Properties = (TPropertyTracking)Activator.CreateInstance(typeof(TPropertyTracking), this)!;
		Properties.PropertyChanged += Properties_PropertyChanged;
		Properties.RevalidateProperty += (s, e) => ValidateProperty(e.PropertyName);
		Properties.RevalidateObject += (s, e) => ValidateObject();
	}

	/// <summary>
	/// Creates a model by auto-constructing the property bag defined by TPropertyTracking and populates it using the supplied collection
	/// </summary>
	/// <param name="collection">The collection from which the elements are copied.</param>
	protected AbstractModelCollection(IEnumerable<T> collection)
		: base(collection)
	{
		Properties = (TPropertyTracking)Activator.CreateInstance(typeof(TPropertyTracking), this)!;
		Properties.PropertyChanged += Properties_PropertyChanged;
		Properties.RevalidateProperty += (s, e) => ValidateProperty(e.PropertyName);
		Properties.RevalidateObject += (s, e) => ValidateObject();
	}

	/// <summary>
	/// Returns True if there are any errors.
	/// </summary>
	/// <value>
	///   <c>true</c> if there are any errors; otherwise, <c>false</c>.
	/// </value>
	/// <remarks>
	/// Call Validate() to refresh this property
	/// </remarks>
	[NotMapped]
	public bool HasErrors
	{
		get { return ErrorsDictionary.HasErrors(); }
	}

	/// <summary>
	/// Returns the underlying property bag
	/// </summary>
	/// <value>
	/// The property bag.
	/// </value>
	[NotMapped]
	protected TPropertyTracking Properties { get; private set; }

	/// <summary>
	/// Returns a collection of all errors (object and property level).
	/// </summary>
	/// <returns></returns>
	/// <remarks>
	/// Call Validate() to refresh this property.
	/// </remarks>
	public ReadOnlyCollection<ValidationResult> GetAllErrors()
	{
		return ErrorsDictionary.GetAllErrors();
	}

	/// <summary>
	/// Returns a collection of object-level errors.
	/// </summary>
	/// <returns></returns>
	/// <remarks>
	/// Call Validate() to refresh this property.
	/// </remarks>

	public ReadOnlyCollection<ValidationResult> GetErrors()
	{
		return GetErrors("");
	}

	/// <summary>
	/// Returns a collection of property-level errors.
	/// </summary>
	/// <param name="propertyName">Null or String.Empty will return the object-level errors</param>
	/// <returns></returns>
	/// <remarks>
	/// Call Validate() to refresh this property.
	/// </remarks>
	public ReadOnlyCollection<ValidationResult> GetErrors(string? propertyName)
	{
		return ErrorsDictionary.GetErrors(propertyName);
	}

	/// <summary>
	/// Invoke this method to signal the events associated with changing the errors dictionary. The parameter updateType is returned by the methods on ErrorsDictionary.
	/// </summary>
	/// <param name="propertyName">Name of the property.</param>
	/// <param name="updateType">Type of the update.</param>
	internal void OnErrorsChanged(string propertyName, ErrorsDictionaryUpdateType updateType)
	{
		if (updateType == ErrorsDictionaryUpdateType.NoChange)
			return;

		OnErrorsChanged(propertyName);

		if (updateType == ErrorsDictionaryUpdateType.HasErrorsIsFalse || updateType == ErrorsDictionaryUpdateType.HasErrorsIsTrue)
			OnPropertyChanged(CommonProperties.HasErrorsProperty);
	}

	/// <summary>
	/// Used to manually invoke the ErrorsChanged event.
	/// </summary>
	/// <param name="propertyName">Name of the property.</param>
	protected void OnErrorsChanged(string propertyName)
	{
		ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
	}

	/// <summary>
	/// Override this method to add imperative validation at the object level.
	/// </summary>
	/// <param name="results">A collection of the declarative validation errors. You may add and remove errors from this collection.</param>

	protected virtual void OnValidateObject(ValidationResultCollection results)
	{
	}

	/// <summary>
	/// Override this method to add imperative validation at the property level.
	/// </summary>
	/// <param name="propertyName">The name of the property being validated.</param>
	/// <param name="results">A collection of the declarative validation errors. You may add and remove errors from this collection.</param>
	protected virtual void OnValidateProperty(string propertyName, ValidationResultCollection results)
	{
	}

	/// <summary>
	/// This forces the object to be completely revalidated.
	/// </summary>
	/// <returns>
	/// True if the object has no errors
	/// </returns>
	public bool Validate()
	{
		Properties.RevalidateAll();
		return !HasErrors;
	}

	/// <summary>
	/// Clears the error collections and the HasErrors property
	/// </summary>
	public void ClearErrors()
	{
		OnErrorsChanged("", ErrorsDictionary.Clear());
	}

	/// <summary>
	/// Fetches a value, creating it if it doesn't exist.
	/// </summary>
	/// <typeparam name="TValue">Expected type that has a parameterless constructor</typeparam>
	/// <param name="propertyName">Case-sensitive property name</param>
	/// <returns></returns>
	/// <exception cref="ArgumentNullException">propertyName;propertyName is null.</exception>
	/// <exception cref="ArgumentException">propertyName is empty.;propertyName</exception>
	/// <remarks>
	/// Executing the constructor will trigger validation but not a property-changed event.
	/// </remarks>
	protected TValue GetNew<TValue>([CallerMemberName] string propertyName = "") where TValue : new()
	{
		if (string.IsNullOrEmpty(propertyName))
			throw new ArgumentException($"{nameof(propertyName)} is null or empty.", nameof(propertyName));

		return Properties.GetNew<TValue>(propertyName);
	}

	/// <summary>
	/// Fetches a value, creating it if it doesn't exist.
	/// </summary>
	/// <typeparam name="TValue">Expected type</typeparam>
	/// <param name="creationFunction">Function to execute if the property doesn't already exist.</param>
	/// <param name="propertyName">Case-sensitive property name</param>
	/// <returns></returns>
	/// <exception cref="ArgumentNullException">
	/// creationFunction;creationFunction is null.
	/// or
	/// propertyName;propertyName is null.
	/// </exception>
	/// <exception cref="ArgumentException">propertyName is empty.;propertyName</exception>
	/// <remarks>
	/// Executing the default function will trigger validation but not a property-changed event.
	/// </remarks>
	protected TValue GetNew<TValue>(Func<TValue> creationFunction, [CallerMemberName] string propertyName = "")
	{
		if (creationFunction == null)
			throw new ArgumentNullException(nameof(creationFunction), $"{nameof(creationFunction)} is null.");
		if (string.IsNullOrEmpty(propertyName))
			throw new ArgumentException($"{nameof(propertyName)} is null or empty.", nameof(propertyName));

		return Properties.GetNew(creationFunction, propertyName);
	}

	/// <summary>
	/// Fetches a string value, returning String.Empty if it doesn't exist.
	/// </summary>
	/// <param name="propertyName">Case-sensitive property name</param>
	/// <returns></returns>
	/// <exception cref="ArgumentNullException">propertyName;propertyName is null.</exception>
	/// <exception cref="ArgumentException">propertyName is empty.;propertyName</exception>
	/// <remarks>
	/// This is a special care for strings, as we usually don't want them to default to null. This is the equivalent of calling GetDefault<string>("", propertyName) </string>
	/// </remarks>
	protected string GetNew([CallerMemberName] string propertyName = "")
	{
		if (string.IsNullOrEmpty(propertyName))
			throw new ArgumentException($"{nameof(propertyName)} is null or empty.", nameof(propertyName));

		return Properties.GetDefault(string.Empty, propertyName);
	}

	/// <summary>
	/// Fetches a value, using the default value it if it doesn't exist.
	/// </summary>
	/// <typeparam name="TValue">Expected type</typeparam>
	/// <param name="defaultValue">Default value to use</param>
	/// <param name="propertyName">Case-sensitive property name</param>
	/// <returns></returns>
	/// <exception cref="ArgumentNullException">propertyName;propertyName is null.</exception>
	/// <exception cref="ArgumentException">propertyName is empty.;propertyName</exception>
	/// <remarks>
	/// Storing the default value will trigger validation but not a property-changed event.
	/// </remarks>
	protected TValue GetDefault<TValue>(TValue defaultValue, [CallerMemberName] string propertyName = "")
	{
		if (string.IsNullOrEmpty(propertyName))
			throw new ArgumentException($"{nameof(propertyName)} is null or empty.", nameof(propertyName));

		return Properties.GetDefault(defaultValue, propertyName);
	}

	/// <summary>
	/// Fetches a value, returning Default(TValue) if it doesn't exist.
	/// </summary>
	/// <typeparam name="TValue">Expected type</typeparam>
	/// <param name="propertyName">Case-sensitive property name</param>
	/// <returns></returns>
	/// <exception cref="ArgumentNullException">propertyName;propertyName is null.</exception>
	/// <exception cref="ArgumentException">propertyName is empty.;propertyName</exception>
	/// <remarks>
	/// Storing the default value will trigger validation but not a property-changed event.
	/// </remarks>
	protected TValue Get<TValue>([CallerMemberName] string propertyName = "")
	{
		if (string.IsNullOrEmpty(propertyName))
			throw new ArgumentException($"{nameof(propertyName)} is null or empty.", nameof(propertyName));

		return Properties.Get<TValue>(propertyName);
	}

	/// <summary>
	/// Set the indicated property to the value.
	/// If the value doesn't match the previous value, or if there is no previous value, raise a property changed notification.
	/// </summary>
	/// <param name="value">Value to be saved.</param>
	/// <param name="propertyName">Name of property to be created/updated</param>
	/// <exception cref="ArgumentNullException">propertyName;propertyName is null</exception>
	/// <exception cref="ArgumentException">propertyName is null or empty.;propertyName</exception>
	protected bool Set(object value, [CallerMemberName] string propertyName = "")
	{
		if (string.IsNullOrEmpty(propertyName))
			throw new ArgumentException($"{nameof(propertyName)} is null or empty.", nameof(propertyName));

		return Properties.Set(value, propertyName);
	}

	/// <summary>
	/// Set the indicated property to the value.
	/// If the value doesn't match the previous value, or if there is no previous value, raise a property changed notification.
	/// </summary>
	/// <typeparam name="TValue"></typeparam>
	/// <param name="value">Value to be saved.</param>
	/// <param name="propertyChanged">A property changed event handler to be attached to the new value. If an old value exists, the event handler will be removed from it.</param>
	/// <param name="propertyName">Name of property to be created/updated</param>
	/// <returns></returns>
	/// <exception cref="ArgumentNullException">
	/// propertyName;propertyName is null
	/// or
	/// propertyChanged;propertyChanged is null.
	/// </exception>
	/// <exception cref="ArgumentException">propertyName is empty.;propertyName</exception>
	public bool Set<TValue>(TValue value, PropertyChangedEventHandler propertyChanged, [CallerMemberName] string propertyName = "")
		where TValue : INotifyPropertyChanged
	{
		if (propertyChanged == null)
			throw new ArgumentNullException(nameof(propertyChanged), $"{nameof(propertyChanged)} is null.");
		if (string.IsNullOrEmpty(propertyName))
			throw new ArgumentException($"{nameof(propertyName)} is null or empty.", nameof(propertyName));

		return Properties.Set(value, propertyChanged, propertyName);
	}

	/// <summary>
	/// Set the indicated property to the value.
	/// If the value doesn't match the previous value, or if there is no previous value, raise a property changed notification.
	/// </summary>
	/// <typeparam name="TValue"></typeparam>
	/// <param name="value">Value to be saved.</param>
	/// <param name="collectionChanged">A collection changed event handler to be attached to the new value. If an old value exists, the event handler will be removed from it.</param>
	/// <param name="propertyName">Name of property to be created/updated</param>
	/// <returns></returns>
	/// <exception cref="ArgumentNullException">propertyName;propertyName is null
	/// or
	/// collectionChanged;collectionChanged is null.</exception>
	/// <exception cref="ArgumentException">propertyName is empty.;propertyName</exception>
	protected bool Set<TValue>(TValue value, NotifyCollectionChangedEventHandler collectionChanged, [CallerMemberName] string propertyName = "")
where TValue : INotifyCollectionChanged
	{
		if (collectionChanged == null)
			throw new ArgumentNullException(nameof(collectionChanged), $"{nameof(collectionChanged)} is null.");
		if (string.IsNullOrEmpty(propertyName))
			throw new ArgumentException($"{nameof(propertyName)} is null or empty.", nameof(propertyName));

		return Properties.Set(value, collectionChanged, propertyName);
	}

	partial void AttributeBasedValidation(string propertyName, ValidationResultCollection results);

	void Properties_PropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		OnPropertyChanged(e);
	}

	void ValidateObject()
	{
		var results = new ValidationResultCollection();

		OnValidateObject(results);
		OnErrorsChanged("", ErrorsDictionary.SetErrors(results, out var affectedProperties));

		foreach (var p in affectedProperties)
			OnErrorsChanged(p);
	}

	void ValidateProperty(string? propertyName)
	{
		if (string.IsNullOrWhiteSpace(propertyName))
			return;

		var results = new ValidationResultCollection();

		AttributeBasedValidation(propertyName!, results);

		OnValidateProperty(propertyName!, results);

		OnErrorsChanged(propertyName!, ErrorsDictionary.SetErrors(propertyName!, results));
	}

	/// <summary>
	/// Gets the errors dictionary.
	/// </summary>
	/// <value>
	/// The errors dictionary.
	/// </value>
	internal ErrorsDictionary ErrorsDictionary { get; } = new ErrorsDictionary();

	/// <summary>
	/// Gets the validation errors for a specified property or for the entire entity.
	/// </summary>
	/// <param name="propertyName">The name of the property to retrieve validation errors for; or null or <see cref="System.String.Empty" />, to retrieve entity-level errors.</param>
	/// <returns>
	/// The validation errors for the property or entity.
	/// </returns>
	IEnumerable INotifyDataErrorInfo.GetErrors(string? propertyName)
	{
		return GetErrors(propertyName);
	}
}

partial class AbstractModelCollection<T, TPropertyTracking>
{
	partial void AttributeBasedValidation(string propertyName, ValidationResultCollection results)
	{
		var property = Properties.Metadata.Properties[propertyName];
		if (property.CanRead)
		{
			var context = new ValidationContext(this) { MemberName = property.Name };
			Validator.TryValidateProperty(property.InvokeGet(this), context, results);
		}
	}

	/// <summary>
	/// Gets an error message indicating what is wrong with this object.
	/// </summary>
	/// <returns>An error message indicating what is wrong with this object. The default is an empty string ("").</returns>
	string IDataErrorInfo.Error
	{
#pragma warning disable CA1033 // Interface methods should be callable by child types
		get
#pragma warning restore CA1033 // Interface methods should be callable by child types
		{
			var errors = from e in GetErrors("") select e.ToString();
			return string.Join("\n", errors.ToArray());
		}
	}

	/// <summary>
	/// Gets the error message for the property with the given name.
	/// </summary>
	/// <param name="columnName">Name of the column.</param>
	/// <returns></returns>
	string IDataErrorInfo.this[string columnName]
	{
		get
		{
			var errors = from e in GetErrors(columnName) select e.ToString();
			return string.Join("\n", errors.ToArray());
		}
	}
}
