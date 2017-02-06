# Change Tracking

Using the `ChangeTrackingModelBase` and `ChangeTrackingModelCollection` base classes enable the change tracking features in Tortuga Anchor.

To fully take advantage of these capabilities, the following rules must be followed:

* All tracked properties are managed (i.e. they use the `Get`/`Set` helper methods).
* All tracked model properties are subclasses of `ChangeTrackingModelBase` or `EditableObjectModelBase`
* All tracked collection properties are subclasses of `ChangeTrackingModelCollection` or `EditableObjectModelCollection`
* There are no circular graphs among tracked properties. This means that if A is the child of B, then B can't also be the child of A. 

## IChangeTracking

Tortuga Anchor implements IChangeTracking at the object graph level. This means that `IsChanged` will return true if the object has changes or if any of its managed children have changes. 

If you want to know if the current object is changed, without considering its children, use the `IsChangedLocal` property.

Calling `AcceptChanges()` will mark the current object as unchanged and call `AcceptChanges()` and all of its managed children. Theoretically the children will then call their children, recursively walking the entire tree. This is why circular graphs are problematic.

One way to avoid a circular graph is to use an unmanaged property to refer to a parent object.

```
public EmployeeCollection Employees => GetNew<EmployeeCollection>();

public Employee Manager {get; set; } //back-reference 
```

The typical use of this interface is to track records that have unsaved changes.

## IRevertibleChangeTracking

Like IChangeTracking, this works at the object graph level. Invoking `RejectChanges()` will call `RejectChanges()`.


## IPropertyChangeTracking/IDetailedPropertyChangeTracking

These interfaces are specific to Tortuga Anchor.

The `ChangedProperties()` method will return a readonly list of modified property names. To get the previous value of any property, use the `GetPreviousValue(string propertyName)` method. Retrieving the current value can be done normally or via reflection.

These interfaces are meant to be used for logging changes to an object. They are also used by Tortuga Chain when generating SQL.

## IEditableObject 

The `IEditableObject` is used in .NET to deal with short-term, revertible changes. If you imagine a dialog window with Ok and Cancel buttons, that would normally be implemented using `IEditableObject`. Data grids in WinForms and WPF also use this interface.

In Tortuga Anchor, this interface is exposed by the `EditableObjectModel` and `EditableObjectModelCollection` base classes. As with `ChangeTrackingModelBase`, the `BeginEdit()`, `EndEdit()`, and `CancelEdit()` methods are implemented at the object graph level. 

Since `EditableObjectModel` and `EditableObjectModelCollection` also implement `IRevertibleChangeTracking`, you effective have two levels of undo possible.

