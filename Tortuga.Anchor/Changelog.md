## Version 4.2

### Features

[#83 Culture Aware (including case insensitive) dynamic objects](https://github.com/TortugaResearch/Tortuga.Anchor/issues/83)

The new `CultureAwareDynamicObject` allows you to supply culture information using a `StringComparer`. This allows for case-insensitive property names.

### Technical Debt

Removed unnecessary compiler constants.


## Version 4.1


### Features

[#77 PropertyMetadata should cache attributes](https://github.com/TortugaResearch/Tortuga.Anchor/issues/77)

All attributes are now being cached by the `PropertyMetadata`.

We still have dedicated attribute lists for performance senstive areas like validation.

[#78 Expose the PropertyInfo class from the PropertyMetadata class](https://github.com/TortugaResearch/Tortuga.Anchor/issues/78)

This is basically an 'esacpe hatch' for when someone needs additional information not exposed via the `PropertyMetadata`.

[#74 ConstructorMetadataCollection needs a DefaultConstructor property](https://github.com/TortugaResearch/Tortuga.Anchor/issues/74)

We often need to grab the default constructor. A lot of code in Chain duplicates this search, which is silly because the `ConstructorMetadataCollection` already knows that the constructor exists.


### Bug Fixes

[#76 Test attribute inheritance on properties](https://github.com/TortugaResearch/Tortuga.Anchor/issues/76)

If a property is virtual, and overriden by a subclass, the attributes on the base class's property were not being acknowledged up by the subclass.

[#70 Column attribute doesn't handle missing column name](https://github.com/TortugaResearch/Tortuga.Anchor/issues/70)

It is possible to construct a `Column` attribute in a way where it doesn't have a column name. This happens when you need to override another column-related value.



## Version 4.0

**Breaking Change** `PropertyMetadata.CanRead` and `PropertyMetadata.CanWrite` were incorrectly returning true for protected properties. It has been fixed to only return true for properties that are actually public. #61 

To determine if you can read or write to protected/private properties, use one of:

 * `CanReadRestricted`
 * `CanReadIndexedAndRestricted`
 * `CanWriteRestricted`
 * `CanWriteIndexedAndRestricted`

To actually perform the read or write, use `InvokeGet` and `InvokeSet`.

`ClassMetadata.ColumnsFor` has been affected by this change.

**Feature** A `MetadataCache.Clone` method has been added. The purpose of this is to make it easier to implement a clone method on a class. #28 

**Feature** CollectionUtilities.BatchAsSegments is a lightweight LINQ batcher. #36

In order to avoid memory allocations, this uses a `ReadOnlyListSegment` struct. 

For more information on these changes, see these blog posts.

* [Anchor 4 Breaking Changes](https://tortugaresearch.com/anchor-4-breaking-changes/)
* [Allocation-free Batching in Anchor 4](https://tortugaresearch.com/allocation-free-batching-in-anchor-4/)
* [Supporting Clone with Anchor 4](https://tortugaresearch.com/supporting-clone-with-anchor-4/)