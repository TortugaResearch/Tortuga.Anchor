# Modeling

The base class for data models is `ModelBase`. This will provide you with the following capabilities:

* [Property Change Notification](PropertyChangeNotification.htm)
* [Programmatic Validation](Validation.htm)
* [Attribute Based Validation](Validation.htm)

If revertible change tracking is desired, use `ChangeTrackingModelBase` as your base class. This includes the following interfaces

* `IChangeTracking`: Indicates whether an object has unsaved changes.
* `IRevertibleChangeTracking`: Allows the unsaved changes to be reverted.
* `IPropertyChangeTracking`: Provides a list of changed properties. Used for logging changes and by the Chain ORM for generating more efficient SQL.
* `IDetailedPropertyChangeTracking`: Adds the ability to retrieve the previous value, which is useful when logging changes.

The `EditableObjectModelBase` further extends this capability by added a second level of undo support via the `IEditableObject` interface. This is used in WinForms/WPF data grids and should be considered for any modal dialog.

For collections, create a subclass of `ModelCollection`, `ChangeTrackingModelCollection`, or `EditableObjectModelCollection` as appropriate.

## Property Change Notification

Simple property change notification is handled via a pair of helper methods.

```
public string FirstName
{
    get { return Get<string>(); }
    set { Set(value); }
}
```
Using this model, the actual values are stored in a dictionary that is used for validation, change tracking, and undo support. 

If you need to specify a non-standard default, use this syntax to avoid accidentally setting the IsChanged flag.

```
public int HitPoints
{
    get { return GetDefault<int>(50); }
    set { Set(value); }
}
```
If your default is an object, you’ll need to use `GetNew<T>` instead. This can call the default constructor or invoke a `Func<T>` to avoid unnecessarily allocating memory. This is most often used for collection properties.

```
//C# 6
public SimplePersonCollection Childern => GetNew<SimplePersonCollection>();

//C# 5
public SimplePersonCollection Childern
{
    get { return GetNew<SimplePersonCollection>(); }
}
```

For advanced scenarios, see [Property Change Notification](PropertyChangeNotification.htm).
