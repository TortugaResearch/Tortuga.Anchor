# Tortuga Anchor
Data Modeling Library in .NET

## Documentation

* [Documentation Website](https://tortugaresearch.github.io/Anchor/)
* [API Reference](http://tortugaresearch.github.io/Anchor/API/Index.html)
* [Anchor Wiki](https://github.com/tortugaresearch/Anchor/wiki)

## What does your entity base class really give you?

Some entity base classes are literally just this.

    public class BaseEntity : IEntity
    {
        public int Id { get; set; }
    }

Besides locking down the name of your primary key (which you probably don't want anyways), that's not very helpful. So what should a base class do for you? Well, here's a short list.

* Validation, both attribute and imperative ([`IDataErrorInfo`](https://msdn.microsoft.com/en-us/library/system.componentmodel.idataerrorinfo(v=vs.110).aspx) / [`INotifyDataErrorInfo`](https://msdn.microsoft.com/en-us/library/system.componentmodel.inotifydataerrorinfo(v=vs.110).aspx))
* Property change notifications ([`INotifyPropertyChanged`](https://msdn.microsoft.com/en-us/library/system.componentmodel.inotifypropertychanged(v=vs.110).aspx))
* Change tracking so you know when records need to be saved ([`IChangeTracking`](https://msdn.microsoft.com/en-us/library/System.ComponentModel.IChangeTracking.aspx))
* Undo ([`IRevertableChangeTracking`](https://msdn.microsoft.com/en-us/library/system.componentmodel.irevertiblechangetracking(v=vs.110).aspx))
* A second undo level for dialogs with ok/cancel buttons. ([`IEditableObject`](https://msdn.microsoft.com/en-us/library/system.componentmodel.ieditableobject(v=vs.110).aspx))

Tortuga Anchor does all of this, and at a very low cost.

**Normal DTO**

    public class Person 
    { 
        public string FirstName 
        { 
             get; 
             set; 
         }
    }

**Tortuga.Anchor Model**

    public class Person : ModelBase     
    { 
        public string FirstName 
        { 
             get { return Get<string>(); } 
             set { Set(value); } 
         }
    }

Just the slight change in how you write your properties is all you need to enable validation and property change notification. To add change tracking, change the base class to `ChangeTrackingModelBase`. If you want a second level of undo, then use `EditableObjectModelBase`. 

And since all of the functionality is exposed through standard .NET interfaces, your existing frameworks and libraries will just work, including serializers and ORMs.
