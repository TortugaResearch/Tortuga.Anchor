using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Tortuga.Anchor.Eventing;

namespace Tortuga.Anchor.Modeling.Internals;

/// <summary>
/// Property bag with basic change tracking capabilities.
/// </summary>
public class ChangeTrackingPropertyBag : PropertyBagBase
{
	readonly IListener<PropertyChangedEventArgs> m_ChildIsChangedPropertyListener;

	//readonly Dictionary<string, object?> m_OriginalValues = new Dictionary<string, object?>(StringComparer.Ordinal);
	readonly object?[] m_OriginalValues;

	readonly BitArray m_ChangedValues;

	bool m_IsChangedLocal;

	/// <summary>
	/// Property bag with basic change tracking capabilities.
	/// </summary>
	/// <param name="owner">Owning model, used to fetch metadata</param>
	public ChangeTrackingPropertyBag(object owner)
: base(owner)
	{
		var count = Metadata.Properties.Count;
		Values = new object?[count];
		m_OriginalValues = new object?[count];
		m_ChangedValues = new BitArray(count);
		for (var i = 0; i < count; i++)
		{
			Values[i] = NotSet.Value;
			m_OriginalValues[i] = NotSet.Value;
		}

		m_ChildIsChangedPropertyListener = new Listener<PropertyChangedEventArgs>(OnChildIsChangedPropertyChanged);
	}

	/// <summary>
	/// Returns True if any fields were modified since the last call to AcceptChanges. This does not walk the object graph.
	/// </summary>
	/// <returns>true if the object’s content has changed since the last call to <see cref="System.ComponentModel.IChangeTracking.AcceptChanges" />; otherwise, false.</returns>
	public bool IsChangedLocal
	{
		get { return m_IsChangedLocal; }
		set
		{
			if (m_IsChangedLocal == value)
				return;
			m_IsChangedLocal = value;

			OnPropertyChanged(Metadata.Properties[nameof(IsChangedLocal)]);
			OnPropertyChanged(Metadata.Properties["IsChanged"]);
		}
	}

	/// <summary>
	/// Access to the values dictionary for sub-classes. Extreme care must be taken when working this dictionary directly, as events will not be automatically fired.
	/// </summary>
	/// <value>
	/// The values.
	/// </value>
	[SuppressMessage("Performance", "CA1819")]
	protected object?[] Values { get; }

	/// <summary>
	/// Marks all fields as unchanged by storing them in the original values collection.
	/// </summary>
	/// <param name="recursive">if set to <c>true</c> [recursive].</param>
	public virtual void AcceptChanges(bool recursive)
	{
		var count = Metadata.Properties.Count;
		for (var i = 0; i < count; i++)
		{
			m_OriginalValues[i] = Values[i];
			if (recursive && Values[i] is IChangeTracking ict)
				ict.AcceptChanges();
		}

		m_ChangedValues.SetAll(false);
		IsChangedLocal = false;
	}

	/// <summary>Implementors need to override this to return the indicated value.</summary>
	/// <returns>The indicated value or System.Reflection.Missing if the value isn't defined.</returns>
	/// <exception cref="ArgumentNullException">propertyName;propertyName is null</exception>
	/// <exception cref="ArgumentException">propertyName is empty.;propertyName</exception>
	public IReadOnlyList<string> ChangedProperties()
	{
		var result = new List<string>();

		var count = Metadata.Properties.Count;
		for (var i = 0; i < count; i++)
		{
			if (m_ChangedValues[i])
				result.Add(Metadata.Properties[i].Name);
		}

		return new ReadOnlyCollection<string>(result);
	}

	/// <summary>
	/// Gets the previous value for the indicated property.
	/// </summary>
	/// <param name="propertyName">
	/// Name of the property.
	/// </param>
	public object? GetPreviousValue(string propertyName)
	{
		if (string.IsNullOrEmpty(propertyName))
			throw new ArgumentException($"{nameof(propertyName)} is null or empty.", nameof(propertyName));

		return m_OriginalValues[GetPropertyIndex(propertyName)];
	}

	/// <summary>Implementors need to override this to return the indicated value.</summary>
	/// <param name="propertyName">Name of the property to fetch.</param>
	/// <returns>The indicated value or NotSet.Value if the value isn't defined.</returns>
	/// <exception cref="ArgumentException">propertyName</exception>
	public override object? GetValue([CallerMemberName] string propertyName = "")
	{
		if (string.IsNullOrEmpty(propertyName))
			throw new ArgumentException($"{nameof(propertyName)} is null or empty.", nameof(propertyName));

		return Values[GetPropertyIndex(propertyName)];
	}

	/// <summary>
	/// Determines whether any objects have unsaved changed in the graph.
	/// </summary>
	/// <returns></returns>
	public bool IsChangedGraph()
	{
		foreach (var item in Values.OfType<IChangeTracking>())
			if (item.IsChanged)
				return true;

		return false;
	}

	/// <summary>
	/// This property indicates whether or not the associated property was created.
	/// </summary>
	/// <param name="propertyName"></param>
	/// <returns></returns>
	/// <exception cref="ArgumentNullException">propertyName;propertyName is null</exception>
	/// <exception cref="ArgumentException">propertyName is empty.;propertyName</exception>
	public override bool IsDefined([CallerMemberName] string propertyName = "")
	{
		if (string.IsNullOrEmpty(propertyName))
			throw new ArgumentException($"{nameof(propertyName)} is null or empty.", nameof(propertyName));

		return Values[GetPropertyIndex(propertyName)] != NotSet.Value;
	}

	/// <summary>
	/// Replaces the current values collection with the original values collection.
	/// </summary>
	/// <param name="recursive">if set to <c>true</c> [recursive].</param>
	public virtual void RejectChanges(bool recursive)
	{
		var needsIsChangedEvent = IsChangedGraph() && !IsChangedLocal;

		var count = Metadata.Properties.Count;

		////remove properties that no longer exist
		//for (int i = 0; i < count; i++)
		//{
		//    var currentValue = Values[i];
		//    if (m_OriginalValues[i] == NotSet.Value)
		//    {
		//        var property = Metadata.Properties[i];
		//        OnPropertyChanging(property);

		//        Values[i] = NotSet.Value;

		//        UpdateChangeTrackingEventHandlers(currentValue, null);

		//        OnPropertyChanged(property);
		//        OnRevalidateProperty(property);
		//    }
		//}

		////update remaining properties

		for (int i = 0; i < count; i++)
		{
			var originalValue = m_OriginalValues[i];
			var currentValue = Values[i];
			if (!Equals(currentValue, originalValue))
			{
				var property = Metadata.Properties[i];
				OnPropertyChanging(property);

				Values[i] = originalValue;

				UpdateChangeTrackingEventHandlers(currentValue, originalValue);

				OnPropertyChanged(property);
				OnRevalidateProperty(property);
			}
		}

		if (recursive)
		{
			for (int i = 0; i < count; i++)
			{
				if (Values[i] is IRevertibleChangeTracking ict)
					ict.RejectChanges();
			}
		}

		m_ChangedValues.SetAll(false);
		IsChangedLocal = false;

		if (needsIsChangedEvent)
			OnPropertyChanged(Metadata.Properties["IsChanged"]);

		OnRevalidateObject();
	}

	/// <summary>
	/// Implementors need to override this to save the indicated value.
	/// </summary>
	/// <param name="value">The value to be saved. A null will set the value to null.</param>
	/// <param name="mode">Indicates special handling for the action.</param>
	/// <param name="propertyName">Name of property to update</param>
	/// <param name="oldValue">The previously stored value. If the property was uninitialized, this will return NotSet.Value</param>
	/// <returns>
	/// True if the value actually changed
	/// </returns>
	/// <exception cref="ArgumentNullException">propertyName;propertyName is null</exception>
	/// <exception cref="ArgumentException">propertyName is empty.;propertyName</exception>
	/// <remarks>
	/// This will create the property if it doesn't already exist
	/// </remarks>
	public override bool Set(object? value, PropertySetModes mode, string propertyName, out object? oldValue)
	{
		if (string.IsNullOrEmpty(propertyName))
			throw new ArgumentException($"{nameof(propertyName)} is null or empty.", nameof(propertyName));

		var property = Metadata.Properties[propertyName];

		if ((mode & PropertySetModes.FixCasing) != 0)
			propertyName = property.Name;

		oldValue = GetValue(propertyName);

		if (Equals(oldValue, value))
			return false;

		if ((mode & PropertySetModes.SetAsOriginal) != 0)
			m_OriginalValues[property.PropertyIndex] = value;

		if ((mode & PropertySetModes.RaiseChangedEvent) != 0)
			OnPropertyChanging(property);

		Values[property.PropertyIndex] = value;

		UpdateChangeTrackingEventHandlers(oldValue, value);

		if ((mode & PropertySetModes.RaiseChangedEvent) != 0)
			OnPropertyChanged(property);

		if ((mode & PropertySetModes.UpdateIsChangedProperty) != 0)
			UpdateIsChangedLocal(property.PropertyIndex);

		if ((mode & PropertySetModes.ValidateProperty) != 0)
			OnRevalidateProperty(property);

		if ((mode & PropertySetModes.ValidateObject) != 0)
			OnRevalidateObject();

		return true;
	}

	void UpdateIsChangedLocal(int propertyIndex)
	{
		var currentFlag = m_ChangedValues[propertyIndex];
		var newFlag = !Equals(m_OriginalValues[propertyIndex], Values[propertyIndex]);

		if (currentFlag == newFlag) //nothing changed
			return;

		m_ChangedValues[propertyIndex] = newFlag;

		if (IsChangedLocal == newFlag) //No need to recalculate IsChangedLocal
			return;

		var count = Metadata.Properties.Count;
		for (var i = 0; i < count; i++)
		{
			if (m_ChangedValues[i])
			{
				IsChangedLocal = true;
				return;
			}
		}
		IsChangedLocal = false;
	}

	/// <summary>
	/// Updates the IsChangedLocal flag by recomparing all properties.
	/// </summary>
	protected void UpdateIsChangedLocal()
	{
		var count = Metadata.Properties.Count;

		for (var i = 0; i < count; i++)
			m_ChangedValues[i] = !Equals(m_OriginalValues[i], Values[i]);

		for (var i = 0; i < count; i++)
		{
			if (m_ChangedValues[i])
			{
				IsChangedLocal = true;
				return;
			}
		}
		IsChangedLocal = false;
	}

	void OnChildIsChangedPropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		if (string.IsNullOrEmpty(e.PropertyName) || e.PropertyName == "IsChanged")
			OnPropertyChanged(Metadata.Properties["IsChanged"]);
	}

	void UpdateChangeTrackingEventHandlers(object? oldValue, object? newValue)
	{
		if (oldValue is IChangeTracking && oldValue is INotifyPropertyChanged changedOld)
			if (oldValue is INotifyPropertyChangedWeak notifyPropertyChangedWeak)
				notifyPropertyChangedWeak.RemoveHandler(m_ChildIsChangedPropertyListener);
			else
				changedOld.PropertyChanged -= OnChildIsChangedPropertyChanged;

		if (newValue is IChangeTracking && newValue is INotifyPropertyChanged changedNew)
			if (newValue is INotifyPropertyChangedWeak notifyPropertyChangedWeak2)
				(notifyPropertyChangedWeak2).AddHandler(m_ChildIsChangedPropertyListener);
			else
				changedNew.PropertyChanged += OnChildIsChangedPropertyChanged;
	}

	/// <inheritdoc/>
	protected internal override object?[] GetInternalValues()
	{
		return Values;
	}

	/// <inheritdoc/>
	protected internal override void SetInternalValues(object?[] valuesArray)
	{
		if (valuesArray == null)
			throw new ArgumentNullException(nameof(valuesArray));
		valuesArray.CopyTo(Values, 0);
		valuesArray.CopyTo(m_OriginalValues, 0);
	}
}
