# Property Change Notification

Simple property change notification is handled via a pair of helper methods.

```
public string FirstName
{
    get { return Get<string>(); }
    set { Set(value); }
}
```

Using this model, the actual values are stored in a dictionary that is used for validation, change tracking, and undo support. This is referred to as a "**managed property**" later in this document.

## Defaults

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

When using `GetNew`, the value isn't actually created until the first time it is read. This does not trigger a proeprty change notification or alter the IsChanged flag.

Note that there is a special case for strings.

```
public string FirstName
{
    get { return GetNew(); }
    set { Set(value); }
}
```

Using GetNew without a type will return an empty string. This is used to avoid nulls.


## Calculated Properties

Calculated properties are registered using an attribute that lists the properties that affect the calculated property.

```
[CalculatedField("FirstName,LastName")] 
public string FullName { 
    get { return FirstName + " " + LastName; } 
} 
```

The properties that make up a calculated property must be managed. Otherwise the appropriate property change notification won't be fired.

The calculated property itself is unmanaged and the value is not stored. However, you can cache the value manually if desired.

## Special Handling of Changed Values

If you need to perform additioanl actions when a property is changed, use this syntax:


```
public DateTime DateOfBirth
{
    get { return Get<DateTime>(); }
    set { 
        if (Set(value))
        {
            VerifyAge();
        }
    }
}
```

The `Set` method will only return true if the value has actually been altered.


## Change Notifications on Child Object

There are times when you may want to listen to events on a child object. While you can manually add and remove the event handlers in the property setter, it is easy to make a mistake and leave a dangling event handler or fail to properly handle nulls. So this convinence method takes the guesswork our of the process.

```
public SimplePerson Secretary
{
    get { return Get<SimplePerson>(); }
    set { Set(value, Secretary_Changed); }
}

void Secretary_Changed(object sender, PropertyChangedEventArgs e)
{
    //event handler
}
```

This will also work with the CollectionChanged and ItemPropertyChanged events.


## Performance Notes

Normal properties will not fire property change notifications if the value hasn't actually changed. This is implemented using `System.Object.Equals`.

To avoiding unnecessarily allocating memory when firing events, each `PropertyChangedEventArgs` object is cached at the class level and reused as necessary. Likewise, events are not fired unless something is actually listening to them.

Because values are stored in a dictionary, structs used to represent property values are boxed. In high performance situations, this may cause more frequent GC cycles if the values are rapidly changing. 

## Memory Safety

In addition to the normal interfaces, the Models and Collections in Tortuga Chain implement a set of weak event interfaces: `INotifyPropertyChangedWeak`, `INotifyCollectionChangedWeak`, and `INotifyItemPropertyChangedWeak`. When used instead of the normal interfaces, these will prevent event-based memory leaks.

