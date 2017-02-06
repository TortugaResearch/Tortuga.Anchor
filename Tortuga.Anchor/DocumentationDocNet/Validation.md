# Validation

Validation for models and collections can be implemented either programmatically or using attributes. Either way, the same interfaces can be used to check the validation status of the object.

When an object is first created, it is in the 'clear' state. This means that even if it wouldn't pass validation, it will appear to have no validation errors. This may seem odd, but it avoids the situation where the user sees a blank form full of errors.

As each property is modified, validation will be triggered on the property. At the same time, object-level validation will occur. This is known as the 'partially validated' state.

To force the object to be fully validated, use the `Validate()` method.

To return to the clear state, invoke `ClearErrors()`. 

## Programmatic Validation

Programmatic validation can occur at the property or object level.


At the property level this is usually expressed as a switch block in the `OnValidateProperty` method.

```
protected override void OnValidateProperty(string propertyName, ValidationResultCollection results)
{
    switch (propertyName)
    {
        case nameof(Age):
            if (Age < 0)
                results.Add(new ValidationResult($"{nameof(Age)} cannot be a negative number", new[] { nameof(Age) }));
            break;
    }
}
```


Object level validation occurs every time any property changes, so it should be kept simple and fast.


```
protected override void OnValidateObject(ValidationResultCollection results)
{
    if (FirstName == LastName && FirstName != "")
        results.Add(new ValidationResult("First and last names cannot match", new[] { "FirstName", "LastName" }));
}
```

## Attribute Based Validation

Attribute based validation, where available, uses the subclasses of `ValidationAttribute` in the `System.ComponentModel.DataAnnotations` namespace.

## IValidatable

Normal code that users Anchor's validation framework will interact with it via the `IValidatable` interface. This interface is significantly easier to use than the validation interfaces that come standard with .NET. It exposes the following methods:

* bool Validate(); //forces all object and property level validators to run. This puts the object into the 'fully validated' state.
* void ClearErrors(); //resets the object to the 'clear' state

The accuracy of the following is depend on whether the object is in the 'clear', 'partially validated', or 'fully validated' state.

* bool HasErrors { get; }
* ReadOnlyCollection<ValidationResult> GetErrors(); //object level errors only
* ReadOnlyCollection<ValidationResult> GetErrors(string propertyName); //property level errors
* ReadOnlyCollection<ValidationResult> GetAllErrors(); //all object and property level errors
* event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged; //fired whenever validation occurs and something changes. This uses `System.ComponentModel.DataErrorsChangedEventArgs`.


# IDataErrorInfo/INotifyDataErrorInfo

These interfaces are automatically implemented when you use Tortuga Anchor's validation framework. 

