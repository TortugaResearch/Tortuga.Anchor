using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Tortuga.Anchor.DataAnnotations;

namespace Tortuga.Anchor.Modeling.Internals;

/// <summary>
/// A base class for models and entities. This is not meant to be used directly by client code.
/// </summary>
/// <typeparam name="TPropertyTracking">The type of property tracking desired.</typeparam>
[DataContract(Namespace = "http://github.com/docevaad/Anchor")]
public abstract partial class AbstractModelBase<TPropertyTracking> : AbstractModelBase, INotifyDataErrorInfo
	where TPropertyTracking : PropertyBagBase
{
	/// <summary>
	/// Creates a model by auto-constructing the property bag defined by TPropertyTracking.
	/// </summary>
	/// <remarks>Requires TPropertyTracking have a public constructor that accepts an Object</remarks>
	protected AbstractModelBase()
	{
		Properties = (TPropertyTracking)Activator.CreateInstance(typeof(TPropertyTracking), this)!;
		Properties.PropertyChanged += Properties_PropertyChanged;
		Properties.RevalidateProperty += (s, e) => ValidateProperty(e.PropertyName);
		Properties.RevalidateObject += (s, e) => ValidateObject();
	}

	/// <summary>
	/// Returns the underlying property bag
	/// </summary>
	[NotMapped]
	protected TPropertyTracking Properties { get; private set; }

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

	/// <summary>
	/// Set the indicated property to the value.
	/// If the value doesn't match the previous value, or if there is no previous value, raise a property changed notification.
	/// </summary>
	/// <typeparam name="T"></typeparam>
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
	public bool Set<T>(T value, PropertyChangedEventHandler propertyChanged, [CallerMemberName] string propertyName = "")
		where T : INotifyPropertyChanged
	{
		if (propertyChanged == null)
			throw new ArgumentNullException(nameof(propertyChanged), $"{nameof(propertyChanged)} is null.");
		if (string.IsNullOrEmpty(propertyName))
			throw new ArgumentException($"{nameof(propertyName)} is null or empty.", nameof(propertyName));

		return Properties.Set(value, propertyChanged, propertyName);
	}

	/// <summary>
	/// This forces the object to be completely revalidated.
	/// </summary>
	/// <returns>
	/// True if the object has no errors
	/// </returns>
	public override sealed bool Validate()
	{
		Properties.RevalidateAll();
		return !HasErrors;
	}

	/// <summary>
	/// Fetches a value, returning Default(T) if it doesn't exist.
	/// </summary>
	/// <typeparam name="T">Expected type</typeparam>
	/// <param name="propertyName">Case-sensitive property name</param>
	/// <returns></returns>
	/// <exception cref="ArgumentNullException">propertyName;propertyName is null.</exception>
	/// <exception cref="ArgumentException">propertyName is empty.;propertyName</exception>
	/// <remarks>
	/// Storing the default value will trigger validation but not a property-changed event.
	/// </remarks>
	protected T Get<T>([CallerMemberName] string propertyName = "")
	{
		if (string.IsNullOrEmpty(propertyName))
			throw new ArgumentException($"{nameof(propertyName)} is null or empty.", nameof(propertyName));

		return Properties.Get<T>(propertyName);
	}

	/// <summary>
	/// Fetches a value, using the default value it if it doesn't exist.
	/// </summary>
	/// <typeparam name="T">Expected type</typeparam>
	/// <param name="defaultValue">Default value to use</param>
	/// <param name="propertyName">Case-sensitive property name</param>
	/// <returns></returns>
	/// <exception cref="ArgumentNullException">propertyName;propertyName is null.</exception>
	/// <exception cref="ArgumentException">propertyName is empty.;propertyName</exception>
	/// <remarks>
	/// Storing the default value will trigger validation but not a property-changed event.
	/// </remarks>
	protected T GetDefault<T>(T defaultValue, [CallerMemberName] string propertyName = "")
	{
		if (string.IsNullOrEmpty(propertyName))
			throw new ArgumentException($"{nameof(propertyName)} is null or empty.", nameof(propertyName));

		return Properties.GetDefault(defaultValue, propertyName);
	}

	/// <summary>
	/// Fetches a value, creating it if it doesn't exist.
	/// </summary>
	/// <typeparam name="T">Expected type that has a parameterless constructor</typeparam>
	/// <param name="propertyName">Case-sensitive property name</param>
	/// <returns></returns>
	/// <exception cref="ArgumentNullException">propertyName;propertyName is null.</exception>
	/// <exception cref="ArgumentException">propertyName is empty.;propertyName</exception>
	/// <remarks>
	/// Executing the constructor will trigger validation but not a property-changed event.
	/// </remarks>
	protected T GetNew<T>([CallerMemberName] string propertyName = "") where T : new()
	{
		if (string.IsNullOrEmpty(propertyName))
			throw new ArgumentException($"{nameof(propertyName)} is null or empty.", nameof(propertyName));

		return Properties.GetNew<T>(propertyName);
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
	/// Fetches a value, creating it if it doesn't exist.
	/// </summary>
	/// <typeparam name="T">Expected type</typeparam>
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
	protected T GetNew<T>(Func<T> creationFunction, [CallerMemberName] string propertyName = "")
	{
		if (creationFunction == null)
			throw new ArgumentNullException(nameof(creationFunction), $"{nameof(creationFunction)} is null.");
		if (string.IsNullOrEmpty(propertyName))
			throw new ArgumentException($"{nameof(propertyName)} is null or empty.", nameof(propertyName));

		return Properties.GetNew(creationFunction, propertyName);
	}

	/// <summary>
	/// Triggers the PropertyChanged event.
	/// </summary>
	/// <param name="propertyName">Name of the property.</param>
	protected void OnPropertyChanged(string propertyName)
	{
		Properties.OnPropertyChanged(propertyName);
	}

	/// <summary>
	/// Triggers the PropertyChanging event.
	/// </summary>
	/// <param name="propertyName">Name of the property.</param>
	protected void OnPropertyChanging(string propertyName)
	{
		Properties.OnPropertyChanging(propertyName);
	}

	/// <summary>
	/// Set the indicated property to the value.
	/// If the value doesn't match the previous value, or if there is no previous value, raise a property changed notification.
	/// </summary>
	/// <param name="value">Value to be saved.</param>
	/// <param name="valueChanged">Delegate that will be invoked if the value of this property changes.</param>
	/// <param name="propertyName">Name of property to be created/updated</param>
	/// <returns>
	/// True is the value actually changed.
	/// </returns>
	/// <exception cref="ArgumentNullException">propertyName;propertyName is null</exception>
	/// <exception cref="ArgumentException">propertyName is null or empty.;propertyName</exception>
	protected bool Set<T>(T value, ValueChanged<T> valueChanged, [CallerMemberName] string propertyName = "")
	{
		if (valueChanged == null)
			throw new ArgumentNullException(nameof(valueChanged), $"{nameof(valueChanged)} is null.");
		if (string.IsNullOrEmpty(propertyName))
			throw new ArgumentException($"{nameof(propertyName)} is null or empty.", nameof(propertyName));

		return Properties.Set(value, valueChanged, propertyName);
	}

	/// <summary>
	/// Set the indicated property to the value.
	/// If the value doesn't match the previous value, or if there is no previous value, raise a property changed notification.
	/// </summary>
	/// <param name="value">Value to be saved.</param>
	/// <param name="propertyName">Name of property to be created/updated</param>
	/// <returns>
	/// True is the value actually changed.
	/// </returns>
	/// <exception cref="ArgumentNullException">propertyName;propertyName is null</exception>
	/// <exception cref="ArgumentException">propertyName is null or empty.;propertyName</exception>
	protected bool Set(object? value, [CallerMemberName] string propertyName = "")
	{
		if (string.IsNullOrEmpty(propertyName))
			throw new ArgumentException($"{nameof(propertyName)} is null or empty.", nameof(propertyName));

		return Properties.Set(value, propertyName);
	}

	/// <summary>
	/// Set the indicated property to the value.
	/// If the value doesn't match the previous value, or if there is no previous value, raise a property changed notification.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="value">Value to be saved.</param>
	/// <param name="collectionChanged">A collection changed event handler to be attached to the new value. If an old value exists, the event handler will be removed from it.</param>
	/// <param name="propertyName">Name of property to be created/updated</param>
	/// <returns></returns>
	/// <exception cref="ArgumentNullException">propertyName;propertyName is null
	/// or
	/// collectionChanged;collectionChanged is null.</exception>
	/// <exception cref="ArgumentException">propertyName is empty.;propertyName</exception>
	protected bool Set<T>(T value, NotifyCollectionChangedEventHandler collectionChanged, [CallerMemberName] string propertyName = "")
where T : INotifyCollectionChanged
	{
		if (collectionChanged == null)
			throw new ArgumentNullException(nameof(collectionChanged), $"{nameof(collectionChanged)} is null.");
		if (string.IsNullOrEmpty(propertyName))
			throw new ArgumentException($"{nameof(propertyName)} is null or empty.", nameof(propertyName));

		return Properties.Set(value, collectionChanged, propertyName);
	}

	partial void AttributeBasedValidation(string propertyName, ValidationResultCollection results);

	partial void AttributeBasedValidation(string propertyName, ValidationResultCollection results)
	{
		var property = Properties.Metadata.Properties[propertyName];

		if (property.CanRead)
		{
			var context = new ValidationContext(this) { MemberName = property.Name };
			Validator.TryValidateProperty(property.InvokeGet(this), context, results);
		}
	}

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

	void Properties_PropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		InvokePropertyChanged(e);
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
}
