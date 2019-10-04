# Metadata

The metadata cache is used to speed up reflection-based operations. It is used by the Modeling framework for property change notifications and validation. In Tortuga Chain, it is used for SQL generation and materializing objects from result sets.

In addition to the basics such as a list of properties and constructors, the metadata cache exposes data extracted from commonly used attributes. This can greatly improve performance when attribute-stored data is frequently used.

.NET Attributes

* Column
* Key
* NotMapped
* Table
* Validation

Tortuga Anchor Attributes

* CalculatedField
* Decompose
* IgnoreOnInsert
* IgnoreOnUpdate

The attribute extraction logic isn't extensible. So if there are other attributes you need, feel free to submit a bug report.

## Code Generation Support

In order to make code generation easier, the `ClassMetadata` also includes a `CSharpFullName` property. This converts the IL name such as `System.Collections.Generic.List`1[System.Int32]` into something that C# can understand. It supports generics, nested classes, and nested generic classes (which are actually harder than you'd think).

There is also a `ColumnsFor` method. This returns the list of database columns that are mapped to properties, honoring the `Column`, `NotMapped`, and `Decompose` attributes.

