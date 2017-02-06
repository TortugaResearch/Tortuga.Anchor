# Change Tracking

Using the `ChangeTrackingModelBase` and `ChangeTrackingModelCollection` base classes enable the change tracking features in Tortuga Anchor.

To fully take advantage of these capabilities, the following rules must be followed:

* All tracked properties are managed (i.e. they use the `Get`/`Set` helper methods).
* All tracked model properties are subclasses of `ChangeTrackingModelBase`
* All tracked collection properties are subclasses of `ChangeTrackingModelCollection`
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

## IRevertibleChangeTracking

Like IChangeTracking, this works at the object graph level. Invoking `RejectChanges()` will call `RejectChanges()`.


## IPropertyChangeTracking/IDetailedPropertyChangeTracking

These interfaces are specific to Tortuga Anchor.

The `ChangedProperties()` method will return a readonly list of modified property names. To get the previous value of any property, use the `GetPreviousValue(string propertyName)` method. Retrieving the current value can be done normally or via reflection.

