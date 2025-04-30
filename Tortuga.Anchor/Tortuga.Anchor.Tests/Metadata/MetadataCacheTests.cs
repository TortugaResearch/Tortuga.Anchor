using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Reflection;
using Tests.Mocks;
using Tortuga.Anchor.Metadata;
using Tortuga.Anchor.Modeling;
using Tortuga.Dragnet;

namespace Tests.Metadata;

public class NullTestsA
{
	public object NotNull { get; set; } = null!;
	public object? Nullable { get; set; }

#nullable disable
	public object NullUnknown { get; set; }
#nullable enable
}

public class EmptyColumn
{
	[Column(TypeName = "money")]
	public int Property0 { get; set; }
}

[Table("TableBase")]
public class Base
{
	[Decompose]
	public ChildA ChildA { get; set; } = null!;

	[Decompose("Bbb")]
	public ChildB ChildB { get; set; } = null!;

	public int Property0 { get; set; }
}

[Table("ChildTable")]
[View("ChildView")]
public class ChildA
{
	[Column("PropertyA2")]
	public int Property { get; set; }

	public int PropertyA1 { get; set; }

	[NotMapped]
	public int PropertyAX { get; set; }
}

public class ChildB
{
	[Column("PropertyB2")]
	public int Property { get; set; }

	public int PropertyB1 { get; set; }

	[NotMapped]
	public int PropertyBX { get; set; }
}

public class Generic<T>
{
	public class GenericNestedInGeneric<T2> { }

	public class NestedInGeneric { }
}

public class OneConstructor
{
	public OneConstructor(int a)
	{
	}
}

public class OneConstructorPerferred
{
	[PreferredConstructor]
	public OneConstructorPerferred(int a) { }
}

public class TwoConstructorsOnePerferred
{
	[PreferredConstructor]
	public TwoConstructorsOnePerferred(int a) { }

	public TwoConstructorsOnePerferred(int a, int b)
	{
	}
}

public class TwoConstructorsNonePerferred
{
	public TwoConstructorsNonePerferred(int a)
	{
	}

	public TwoConstructorsNonePerferred(int a, int b)
	{
	}
}

public class TwoConstructorsTwoPerferred
{
	[PreferredConstructor]
	public TwoConstructorsTwoPerferred(int a) { }

	[PreferredConstructor]
	public TwoConstructorsTwoPerferred(int a, int b) { }
}

[TestClass]
public class MetadataCacheTests
{
	[TestMethod]
	public void OneConstructor()
	{
		var metadata = MetadataCache.GetMetadata<OneConstructor>();
		Assert.IsNull(metadata.Constructors.PreferredConstructor);
	}

	[TestMethod]
	public void OneConstructorPerferred()
	{
		var metadata = MetadataCache.GetMetadata<OneConstructorPerferred>();
		Assert.IsNotNull(metadata.Constructors.PreferredConstructor);
	}

	[TestMethod]
	public void TwoConstructorsOnePerferred()
	{
		var metadata = MetadataCache.GetMetadata<TwoConstructorsOnePerferred>();
		Assert.IsNotNull(metadata.Constructors.PreferredConstructor);
	}

	[TestMethod]
	public void TwoConstructorsNonePerferred()
	{
		var metadata = MetadataCache.GetMetadata<TwoConstructorsNonePerferred>();
		Assert.IsNull(metadata.Constructors.PreferredConstructor);
	}

	[TestMethod]
	public void TwoConstructorsTwoPerferred()
	{
		try
		{
			var metadata = MetadataCache.GetMetadata<TwoConstructorsTwoPerferred>();
			Assert.Fail();
		}
		catch (InvalidOperationException)
		{
			//Expected
		}
	}

	[TestMethod]
	public void AttributeInheritanceForProperties()
	{
		var metaBase = MetadataCache.GetMetadata<BaseClass>();
		var metaSub = MetadataCache.GetMetadata<SubClass>();

		Assert.IsTrue(metaBase.Properties["Name"].Validators.Length == 1, "Base class is missing its validation attribute.");
		Assert.IsTrue(metaSub.Properties["Name"].Validators.Length == 1, "Subclass didn't inherit its validation attribute.");
	}

	[TestMethod]
	public void TableAttribute()
	{
		var meta = MetadataCache.GetMetadata<Base>();
		Assert.AreEqual("TableBase", meta.MappedTableName);
	}

	[TestMethod]
	public void TableAndViewAttribute()
	{
		var meta = MetadataCache.GetMetadata<ChildA>();
		Assert.AreEqual("ChildTable", meta.MappedTableName);
		Assert.AreEqual("ChildView", meta.MappedViewName);
	}

	[TestMethod]
	public void NullableProperty()
	{
		var meta = MetadataCache.GetMetadata<NullTestsA>();
		Assert.AreEqual(false, meta.Properties.Single(x => x.Name == "NotNull").IsReferenceNullable);
	}

	[TestMethod]
	public void NonNullableProperty()
	{
		var meta = MetadataCache.GetMetadata<NullTestsA>();
		Assert.AreEqual(true, meta.Properties.Single(x => x.Name == "Nullable").IsReferenceNullable);
	}

	[TestMethod]
	public void NullAgnostic()
	{
		var meta = MetadataCache.GetMetadata<NullTestsA>();
		Assert.IsNull(meta.Properties.Single(x => x.Name == "NullUnknown").IsReferenceNullable);
	}

	[TestMethod]
	public void MetadataCache_Constructors_Test()
	{
		using (var verify = new Verify())
		{
			var result = MetadataCache.GetMetadata(typeof(MultiConstructor));

			var a = result.Constructors.Find(typeof(int));
			var b = result.Constructors.Find(typeof(string));
			var c = result.Constructors.Find(typeof(int), typeof(string));
			var d = result.Constructors.Find(typeof(long), typeof(string));

			verify.IsNotNull(a, "int");
			verify.IsNotNull(b, "string");
			verify.IsNotNull(c, "int, string");
			verify.IsNull(d, "long, string shouldn't exist");
		}
	}

	[TestMethod]
	public void MetadataCache_CSharp_Generic()
	{
		using (var verify = new Verify())
		{
			var name = MetadataCache.GetMetadata(typeof(Generic<int>)).CSharpFullName;
			verify.AreEqual("Tests.Metadata.Generic<System.Int32>", name, "C# type name was not correct");
		}
	}

	[TestMethod]
	public void MetadataCache_CSharp_Generic_Dictionary()
	{
		using (var verify = new Verify())
		{
			var name = MetadataCache.GetMetadata(typeof(Dictionary<int, string>)).CSharpFullName;
			verify.AreEqual("System.Collections.Generic.Dictionary<System.Int32, System.String>", name, "C# type name was not correct");
		}
	}

	[TestMethod]
	public void MetadataCache_CSharp_Generic_Of_Generic()
	{
		using (var verify = new Verify())
		{
			var name = MetadataCache.GetMetadata(typeof(Generic<Generic<int>>)).CSharpFullName;
			verify.AreEqual("Tests.Metadata.Generic<Tests.Metadata.Generic<System.Int32>>", name, "C# type name was not correct");
		}
	}

	[TestMethod]
	public void MetadataCache_CSharp_GenericNestedInGeneric()
	{
		using (var verify = new Verify())
		{
			var name = MetadataCache.GetMetadata(typeof(Generic<int>.GenericNestedInGeneric<long>)).CSharpFullName;
			verify.AreEqual("Tests.Metadata.Generic<System.Int32>.GenericNestedInGeneric<System.Int64>", name, "C# type name was not correct");
		}
	}

	[TestMethod]
	public void MetadataCache_CSharp_Nested()
	{
		using (var verify = new Verify())
		{
			var name = MetadataCache.GetMetadata(typeof(Normal.Nested)).CSharpFullName;
			verify.AreEqual("Tests.Metadata.Normal.Nested", name, "C# type name was not correct");
		}
	}

	[TestMethod]
	public void MetadataCache_CSharp_NestedInGeneric()
	{
		using (var verify = new Verify())
		{
			var name = MetadataCache.GetMetadata(typeof(Generic<int>.NestedInGeneric)).CSharpFullName;
			verify.AreEqual("Tests.Metadata.Generic<System.Int32>.NestedInGeneric", name, "C# type name was not correct");
		}
	}

	[TestMethod]
	public void MetadataCache_CSharp_Normal()
	{
		using (var verify = new Verify())
		{
			var name = MetadataCache.GetMetadata(typeof(Normal)).CSharpFullName;
			verify.AreEqual("Tests.Metadata.Normal", name, "C# type name was not correct");
		}
	}

	[TestMethod]
	public void MetadataCache_DefaultConstructor_Test()
	{
		using (var verify = new Verify())
		{
			var a = MetadataCache.GetMetadata(typeof(AutoConstructor));
			verify.IsTrue(a.Constructors.HasDefaultConstructor, "AutoConstructor should have a automatically implemented default constructor");

			var b = MetadataCache.GetMetadata(typeof(DefaultConstructor));
			verify.IsTrue(b.Constructors.HasDefaultConstructor, "DefaultConstructor should have a automatically implemented default constructor");

			var c = MetadataCache.GetMetadata(typeof(DefaultConstructorPrivateClass));
			verify.IsTrue(c.Constructors.HasDefaultConstructor, "DefaultConstructorPrivateClass should have a automatically implemented default constructor");

			var d = MetadataCache.GetMetadata(typeof(PrivateDefaultConstructor));
			verify.IsFalse(d.Constructors.HasDefaultConstructor, "PrivateDefaultConstructor does not have a visible default constructor");

			var e = MetadataCache.GetMetadata(typeof(NonDefaultConstructor));
			verify.IsFalse(e.Constructors.HasDefaultConstructor, "NonDefaultConstructor does not have a default constructor");

			var f = MetadataCache.GetMetadata(typeof(StaticConstructor));
			verify.IsTrue(f.Constructors.HasDefaultConstructor, "NonDefaultConstructor does not have a default constructor");
		}
	}

	[TestMethod]
	public void MetadataCache_FSharp_Generic()
	{
		using (var verify = new Verify())
		{
			var name = MetadataCache.GetMetadata(typeof(Generic<int>)).FSharpFullName;
			verify.AreEqual("Tests.Metadata.Generic<'System.Int32>", name, "F# type name was not correct");
		}
	}

	[TestMethod]
	public void MetadataCache_FSharp_Generic_Dictionary()
	{
		using (var verify = new Verify())
		{
			var name = MetadataCache.GetMetadata(typeof(Dictionary<int, string>)).FSharpFullName;
			verify.AreEqual("System.Collections.Generic.Dictionary<'System.Int32, 'System.String>", name, "F# type name was not correct");
		}
	}

	[TestMethod]
	public void MetadataCache_FSharp_Generic_Of_Generic()
	{
		using (var verify = new Verify())
		{
			var name = MetadataCache.GetMetadata(typeof(Generic<Generic<int>>)).FSharpFullName;
			verify.AreEqual("Tests.Metadata.Generic<'Tests.Metadata.Generic<'System.Int32>>", name, "F# type name was not correct");
		}
	}

	[TestMethod]
	public void MetadataCache_FSharp_GenericNestedInGeneric()
	{
		using (var verify = new Verify())
		{
			var name = MetadataCache.GetMetadata(typeof(Generic<int>.GenericNestedInGeneric<long>)).FSharpFullName;
			verify.AreEqual("Tests.Metadata.Generic<'System.Int32>.GenericNestedInGeneric<'System.Int64>", name, "F# type name was not correct");
		}
	}

	[TestMethod]
	public void MetadataCache_FSharp_Nested()
	{
		using (var verify = new Verify())
		{
			var name = MetadataCache.GetMetadata(typeof(Normal.Nested)).FSharpFullName;
			verify.AreEqual("Tests.Metadata.Normal.Nested", name, "F# type name was not correct");
		}
	}

	[TestMethod]
	public void MetadataCache_FSharp_NestedInGeneric()
	{
		using (var verify = new Verify())
		{
			var name = MetadataCache.GetMetadata(typeof(Generic<int>.NestedInGeneric)).FSharpFullName;
			verify.AreEqual("Tests.Metadata.Generic<'System.Int32>.NestedInGeneric", name, "F# type name was not correct");
		}
	}

	[TestMethod]
	public void MetadataCache_FSharp_Normal()
	{
		using (var verify = new Verify())
		{
			var name = MetadataCache.GetMetadata(typeof(Normal)).FSharpFullName;
			verify.AreEqual("Tests.Metadata.Normal", name, "F# type name was not correct");
		}
	}

	[TestMethod]
	public void MetadataCache_IsNullable_Int()
	{
		var x = MetadataCache.GetMetadata(typeof(int));
		var y = x.IsNullable;
		Assert.AreEqual(false, y);
	}

	[TestMethod]
	public void MetadataCache_IsNullable_Nullable_Int()
	{
		var x = MetadataCache.GetMetadata(typeof(int?));
		var y = x.IsNullable;
		Assert.AreEqual(true, y);
	}

	[TestMethod]
	public void MetadataCache_IsNullable_Object()
	{
		var x = MetadataCache.GetMetadata(typeof(object));
		var y = x.IsNullable;
		Assert.AreEqual(true, y);
	}

	[TestMethod]
	public void MetadataCache_MakeNonNullable_Int()
	{
		var x = MetadataCache.GetMetadata(typeof(int));
		var y = x.MakeNonNullable().TypeInfo;
		Assert.AreEqual(typeof(int), y);
	}

	[TestMethod]
	public void MetadataCache_MakeNonNullable_Nullable_Int()
	{
		var x = MetadataCache.GetMetadata(typeof(int?));
		var y = x.MakeNonNullable().TypeInfo;
		Assert.AreEqual(typeof(int), y);
	}

	[TestMethod]
	public void MetadataCache_MakeNonNullable_Object()
	{
		var x = MetadataCache.GetMetadata(typeof(object));
		var y = x.MakeNonNullable().TypeInfo;
		Assert.AreEqual(typeof(object), y);
	}

	[TestMethod]
	public void MetadataCache_MakeNullable_Int()
	{
		var x = MetadataCache.GetMetadata(typeof(int));
		var y = x.MakeNullable().TypeInfo;
		Assert.AreEqual(typeof(int?), y);
	}

	[TestMethod]
	public void MetadataCache_MakeNullable_Nullable_Int()
	{
		var x = MetadataCache.GetMetadata(typeof(int?));
		var y = x.MakeNullable().TypeInfo;
		Assert.AreEqual(typeof(int?), y);
	}

	[TestMethod]
	public void MetadataCache_MakeNullable_Object()
	{
		var x = MetadataCache.GetMetadata(typeof(object));
		var y = x.MakeNullable().TypeInfo;
		Assert.AreEqual(typeof(object), y);
	}

	[TestMethod]
	public void MetadataCache_PrivateProperty_Test()
	{
		using (var verify = new Verify())
		{
			var result = MetadataCache.GetMetadata(typeof(Mock));
			verify.IsNotNull(result, "GetMetadata returned a null");

			var privateProperty = result.Properties["PrivateProperty"];
			verify.IsNotNull(privateProperty, "property wasn't found");
			verify.IsFalse(privateProperty.AffectsCalculatedFields, "not used in calculated fields");
			verify.AreEqual(0, privateProperty.CalculatedFields.Length, "not used in calculated fields");
			verify.IsFalse(privateProperty.CanRead, "CanRead");
			verify.IsFalse(privateProperty.CanWrite, "CanWrite");
			verify.AreEqual("PrivateProperty", privateProperty.Name, "Name");
			verify.AreEqual("PrivateProperty", privateProperty.PropertyChangedEventArgs.PropertyName, "PropertyName");
			verify.AreEqual(typeof(int), privateProperty.PropertyType, "PropertyType");

			verify.AreEqual(0, privateProperty.Validators.Length, "not used in validation");
		}
	}

	[TestMethod]
	public void MetadataCache_ProtectedProperty_Test()
	{
		using (var verify = new Verify())
		{
			var result = MetadataCache.GetMetadata(typeof(Mock));
			verify.IsNotNull(result, "GetMetadata returned a null");
			var protectedProperty = result.Properties["ProtectedProperty"];
			verify.IsNotNull(protectedProperty, "property wasn't found");
			verify.IsFalse(protectedProperty.AffectsCalculatedFields, "not used in calculated fields");
			verify.AreEqual(0, protectedProperty.CalculatedFields.Length, "not used in calculated fields");
			verify.IsFalse(protectedProperty.CanRead, "CanRead");
			verify.IsFalse(protectedProperty.CanWrite, "CanWrite");
			verify.AreEqual("ProtectedProperty", protectedProperty.Name, "Name");
			verify.AreEqual("ProtectedProperty", protectedProperty.PropertyChangedEventArgs.PropertyName, "PropertyName");
			verify.AreEqual(typeof(int), protectedProperty.PropertyType, "PropertyType");

			verify.AreEqual(0, protectedProperty.Validators.Length, "not used in validation");
		}
	}

	[TestMethod]
	public void MetadataCache_PublicPrivateProperty_Test()
	{
		using (var verify = new Verify())
		{
			var result = MetadataCache.GetMetadata(typeof(Mock));
			verify.IsNotNull(result, "GetMetadata returned a null");
			var publicPrivateProperty = result.Properties["PublicPrivateProperty"];
			verify.IsNotNull(publicPrivateProperty, "property wasn't found");
			verify.IsFalse(publicPrivateProperty.AffectsCalculatedFields, "not used in calculated fields");
			verify.AreEqual(0, publicPrivateProperty.CalculatedFields.Length, "not used in calculated fields");
			verify.IsTrue(publicPrivateProperty.CanRead, "CanRead");
			verify.IsFalse(publicPrivateProperty.CanWrite, "CanWrite");
			verify.AreEqual("PublicPrivateProperty", publicPrivateProperty.Name, "Name");
			verify.AreEqual("PublicPrivateProperty", publicPrivateProperty.PropertyChangedEventArgs.PropertyName, "PropertyName");
			verify.AreEqual(typeof(int), publicPrivateProperty.PropertyType, "PropertyType");

			verify.AreEqual(0, publicPrivateProperty.Validators.Length, "not used in validation");
		}
	}

	[TestMethod]
	public void MetadataCache_PublicProperty_Test()
	{
		using (var verify = new Verify())
		{
			var result = MetadataCache.GetMetadata(typeof(Mock));
			verify.IsNotNull(result, "GetMetadata returned a null");

			var publicProperty = result.Properties["PublicProperty"];
			verify.IsNotNull(publicProperty, "property wasn't found");
			verify.IsFalse(publicProperty.AffectsCalculatedFields, "not used in calculated fields");
			verify.AreEqual(0, publicProperty.CalculatedFields.Length, "not used in calculated fields");
			verify.IsTrue(publicProperty.CanRead, "CanRead");
			verify.IsTrue(publicProperty.CanWrite, "CanWrite");
			verify.AreEqual("PublicProperty", publicProperty.Name, "Name");
			verify.AreEqual("PublicProperty", publicProperty.PropertyChangedEventArgs.PropertyName, "PropertyName");
			verify.AreEqual(typeof(int), publicProperty.PropertyType, "PropertyType");

			verify.AreEqual(0, publicProperty.Validators.Length, "not used in validation");

			verify.IsTrue(result.Constructors.HasDefaultConstructor, "this should have a automatically implemented default constructor");
		}
	}

	[TestMethod]
	public void MetadataCache_PublicProtectedProperty_Test()
	{
		using (var verify = new Verify())
		{
			var result = MetadataCache.GetMetadata(typeof(Mock));
			verify.IsNotNull(result, "GetMetadata returned a null");
			var publicProtectedProperty = result.Properties["PublicProtectedProperty"];
			verify.IsNotNull(publicProtectedProperty, "property wasn't found");
			verify.IsFalse(publicProtectedProperty.AffectsCalculatedFields, "not used in calculated fields");
			verify.AreEqual(0, publicProtectedProperty.CalculatedFields.Length, "not used in calculated fields");
			verify.IsTrue(publicProtectedProperty.CanRead, "CanRead");
			verify.IsFalse(publicProtectedProperty.CanWrite, "CanWrite");
			verify.AreEqual("PublicProtectedProperty", publicProtectedProperty.Name, "Name");
			verify.AreEqual("PublicProtectedProperty", publicProtectedProperty.PropertyChangedEventArgs.PropertyName, "PropertyName");
			verify.AreEqual(typeof(int), publicProtectedProperty.PropertyType, "PropertyType");

			verify.AreEqual(0, publicProtectedProperty.Validators.Length, "not used in validation");
		}
	}

	[TestMethod]
	public void MetadataCache_ReflexiveTest1()
	{
		using (var verify = new Verify())
		{
			var fromType = MetadataCache.GetMetadata(typeof(Mock));
			var fromTypeInfo = MetadataCache.GetMetadata(typeof(Mock).GetTypeInfo());

			verify.AreSame(fromType, fromTypeInfo, "From Type was not cached");
		}
	}

	[TestMethod]
	public void MetadataCache_ReflexiveTest2()
	{
		using (var verify = new Verify())
		{
			var fromTypeInfo = MetadataCache.GetMetadata(typeof(Mock).GetTypeInfo());
			var fromType = MetadataCache.GetMetadata(typeof(Mock));

			verify.AreSame(fromType, fromTypeInfo, "From TypeInfo was not cached");
		}
	}

	[TestMethod]
	public void MetadataCache_SetOnlyProperty_Test()
	{
		using (var verify = new Verify())
		{
			var result = MetadataCache.GetMetadata(typeof(Mock));
			verify.IsNotNull(result, "GetMetadata returned a null");

			var setOnlyProperty = result.Properties["SetOnlyProperty"];
			verify.IsNotNull(setOnlyProperty, "property wasn't found");
			verify.IsFalse(setOnlyProperty.AffectsCalculatedFields, "not used in calculated fields");
			verify.AreEqual(0, setOnlyProperty.CalculatedFields.Length, "not used in calculated fields");
			verify.IsFalse(setOnlyProperty.CanRead, "CanRead");
			verify.IsTrue(setOnlyProperty.CanWrite, "CanWrite");
			verify.AreEqual("SetOnlyProperty", setOnlyProperty.Name, "Name");
			verify.AreEqual("SetOnlyProperty", setOnlyProperty.PropertyChangedEventArgs.PropertyName, "PropertyName");
			verify.AreEqual(typeof(int), setOnlyProperty.PropertyType, "PropertyType");

			verify.AreEqual(0, setOnlyProperty.Validators.Length, "not used in validation");
		}
	}

	[TestMethod]
	public void MetadataCache_Test11()
	{
		using (var verify = new Verify())
		{
			var result = MetadataCache.GetMetadata(typeof(Mock));
			var mock = new Mock();
			var prop = result.Properties["PublicProperty"];
			prop.InvokeSet(mock, 5);
			var value = (int)prop.InvokeGet(mock)!;
			verify.AreEqual(5, value, "InvokeGet");
		}
	}

	[TestMethod]
	public void MetadataCache_Test12()
	{
		using (var verify = new Verify())
		{
			var result = MetadataCache.GetMetadata(typeof(Mock));
			verify.ArgumentException("propertyName", () =>
				{
					var x = result.Properties[null!];
				});
		}
	}

	[TestMethod]
	public void MetadataCache_Test13()
	{
		using (var verify = new Verify())
		{
			var result = MetadataCache.GetMetadata(typeof(Mock));
			verify.ArgumentException("propertyName", () =>
			{
				var x = result.Properties[null!];
			}, "empty strings are not allowed");
		}
	}

	[TestMethod]
	public void MetadataCache_Test14()
	{
		using (var verify = new Verify())
		{
			var result = MetadataCache.GetMetadata(typeof(List<int>));

			foreach (var item in result.Properties)
			{
				Debug.WriteLine(item.Name);
			}

			var x = result.Properties["Item [Int32]"];
			verify.IsNotNull(x, "Item [Int32] property is missing");

			var y = result.Properties["Item[]"];
			verify.IsNotNull(y, "Item[] property is missing");
			verify.AreSame(x, y, "Item[] and Item [Int32] don't match");
		}
	}

	[TestMethod]
	public void MetadataCache_Test14B()
	{
		using (var verify = new Verify())
		{
			var result = MetadataCache.GetMetadata(typeof(StringIndexedMock));
			var x = result.Properties["Item [System.String]"];
			verify.IsNotNull(x, "Item [System.String] property is missing");

			var y = result.Properties["Item[]"];
			verify.IsNotNull(y, "Item[] property is missing");
			verify.AreSame(x, y, "Item[] and Item [System.String] don't match");
		}
	}

	[TestMethod]
	public void MetadataCache_Test15()
	{
		using (var verify = new Verify())
		{
			var result = MetadataCache.GetMetadata(typeof(Mock));
			verify.IsTrueForAll(result.Properties.PropertyNames, p => result.Properties.Contains(p), "Failed the contains test");
		}
	}

	[TestMethod]
	public void MetadataCache_Test16()
	{
		using (var verify = new Verify())
		{
			var result = MetadataCache.GetMetadata(typeof(Mock));
			verify.IsTrueForAll(result.Properties, p => result.Properties.Contains(p), "failed the contains test");
		}
	}

	[TestMethod]
	public void MetadataCache_Test17()
	{
		using (var verify = new Verify())
		{
			var result = MetadataCache.GetMetadata(typeof(Mock));
			verify.ArgumentNullException("item", () => result.Properties.Contains((PropertyMetadata)null!));
		}
	}

	[TestMethod]
	public void MetadataCache_Test18()
	{
		using (var verify = new Verify())
		{
			var result = MetadataCache.GetMetadata(typeof(Mock));
			verify.ArgumentException("propertyName", () => result.Properties.Contains((string)null!));
		}
	}

	[TestMethod]
	public void MetadataCache_Test2()
	{
		using (var verify = new Verify())
		{
			verify.ArgumentNullException("type", () => MetadataCache.GetMetadata((Type)null!));
		}
	}

	[TestMethod]
	public void MetadataCache_Test20()
	{
		using (var verify = new Verify())
		{
			var result = (ICollection<PropertyMetadata>)MetadataCache.GetMetadata(typeof(Mock)).Properties;
			verify.NotSupportedException(() => result.Add(MetadataCache.GetMetadata(typeof(String)).Properties.First()));
		}
	}

	[TestMethod]
	public void MetadataCache_Test21()
	{
		using (var verify = new Verify())
		{
			var result = (ICollection<PropertyMetadata>)MetadataCache.GetMetadata(typeof(Mock)).Properties;
			verify.NotSupportedException(() => result.Remove(MetadataCache.GetMetadata(typeof(String)).Properties.First()));
		}
	}

	[TestMethod]
	public void MetadataCache_Test22()
	{
		using (var verify = new Verify())
		{
			var result = (ICollection<PropertyMetadata>)MetadataCache.GetMetadata(typeof(Mock)).Properties;
			verify.NotSupportedException(() => result.Clear());
		}
	}

	[TestMethod]
	public void MetadataCache_Test23()
	{
		using (var verify = new Verify())
		{
			var result = (ICollection<PropertyMetadata>)MetadataCache.GetMetadata(typeof(Mock)).Properties;
			verify.IsTrue(result.IsReadOnly, "Collection should be read only");
		}
	}

	[TestMethod]
	public void MetadataCache_Test24()
	{
		using (var verify = new Verify())
		{
			var result = MetadataCache.GetMetadata(typeof(Mock));
			verify.ArgumentOutOfRangeException("propertyName", "DOES_NOT_EXIST", () =>
			{
				var p = result.Properties["DOES_NOT_EXIST"];
			});
		}
	}

	[TestMethod]
	public void MetadataCache_Test25()
	{
		using (var verify = new Verify())
		{
			var result = MetadataCache.GetMetadata(typeof(Mock));
			verify.IsFalse(result.Properties.TryGetValue("DOES_NOT_EXIST", out var p), "TryGet expected to fail here");
			verify.IsNull(p, "TryGet failed, so this should be null.");
		}
	}

	[TestMethod]
	public void MetadataCache_Test26()
	{
		using (var verify = new Verify())
		{
			var result = MetadataCache.GetMetadata(typeof(Mock));
			verify.IsTrue(result.Properties.TryGetValue("PublicProperty", out var p), "TryGet should have succeeded");
			verify.IsNotNull(p, "TryGet should have succeeded");
		}
	}

	[TestMethod]
	public void MetadataCache_Test27()
	{
		using (var verify = new Verify())
		{
			var result = MetadataCache.GetMetadata(typeof(Mock));
			verify.ArgumentException("propertyName", () => result.Properties.TryGetValue("", out var p), "can't use empty strings for property name");
		}
	}

	[TestMethod]
	public void MetadataCache_Test28()
	{
		using (var verify = new Verify())
		{
			var result = MetadataCache.GetMetadata(typeof(Mock));
			verify.ArgumentException("propertyName", () => result.Properties.TryGetValue(null!, out var p));
		}
	}

	[TestMethod]
	public void MetadataCache_Test29()
	{
		using (var verify = new Verify())
		{
			var result = (IEnumerable)MetadataCache.GetMetadata(typeof(Mock)).Properties;
			verify.IsNotNull(result.GetEnumerator(), "basic enumerator check");
		}
	}

	[TestMethod]
	public void MetadataCache_Test2b()
	{
		using (var verify = new Verify())
		{
			verify.ArgumentNullException("type", () => MetadataCache.GetMetadata((TypeInfo)null!));
		}
	}

	[TestMethod]
	public void MetadataCache_Test30()
	{
		using (var verify = new Verify())
		{
			var result = MetadataCache.GetMetadata(typeof(object));
			var mockProperty = MetadataCache.GetMetadata(typeof(Mock)).Properties.First();
			verify.AreEqual(0, result.Properties.Count, "the type System.Object has no properties");
			verify.IsFalse(result.Properties.Contains(mockProperty), "contains test");
		}
	}

	[TestMethod]
	public void MetadataCache_Test31()
	{
		using (var verify = new Verify())
		{
			var result = MetadataCache.GetMetadata(typeof(ShadowedMock));
			verify.IsNotNull(result, "cache should never return null");
		}
	}

	[TestMethod]
	public void MetadataCache_Test32()
	{
		using (var verify = new Verify())
		{
			var result = MetadataCache.GetMetadata(typeof(Base)).ColumnsFor.OrderBy(x => x).ToList();
			verify.AreEqual(5, result.Count, "");
			verify.AreEqual("BbbPropertyB1", result[0], "");
			verify.AreEqual("BbbPropertyB2", result[1], "");
			verify.AreEqual("Property0", result[2], "");
			verify.AreEqual("PropertyA1", result[3], "");
			verify.AreEqual("PropertyA2", result[4], "");
		}
	}

	[TestMethod]
	public void MetadataCache_Test33()
	{
		using (var verify = new Verify())
		{
			var result = MetadataCache.GetMetadata(typeof(EmptyColumn));
			verify.AreEqual("Property0", result.Properties["Property0"].MappedColumnName, "Column wasn't mapped correctly.");
		}
	}

	[TestMethod]
	public void MetadataCache_Test4()
	{
		using (var verify = new Verify())
		{
			var result = MetadataCache.GetMetadata(typeof(Mock));

			var source1 = result.Properties["CalculatedSource1"];
			var target = result.Properties["CalculatedTarget"];

			verify.IsTrue(source1.AffectsCalculatedFields, "AffectsCalculatedFields");
			CollectionAssert.Contains(source1.CalculatedFields, target);
		}
	}

	[TestMethod]
	public void MetadataCache_Test5()
	{
		using (var verify = new Verify())
		{
			try
			{
				MetadataCache.GetMetadata(typeof(BadMock));
				verify.Fail("This should have thrown an exception because of the unmatched calculated field.");
			}
			catch (InvalidOperationException)
			{
			}
		}
	}

	[TestMethod]
	public void MetadataCache_Test7()
	{
		using (var verify = new Verify())
		{
			var result = MetadataCache.GetMetadata(typeof(Mock));
			var mock = new Mock();
			var prop = result.Properties["PrivateProperty"];

			try
			{
				prop.InvokeGet(mock);
			}
			catch (InvalidOperationException)
			{
			}
		}
	}

	[TestMethod]
	public void MetadataCache_Test8()
	{
		using (var verify = new Verify())
		{
			var result = MetadataCache.GetMetadata(typeof(Mock));
			var mock = new Mock();
			var prop = result.Properties["PrivateProperty"];

			try
			{
				prop.InvokeSet(mock, 5);
			}
			catch (InvalidOperationException)
			{
			}
		}
	}

	[TestMethod]
	public void MetadataCache_VisualBasic_Generic()
	{
		using (var verify = new Verify())
		{
			var name = MetadataCache.GetMetadata(typeof(Generic<int>)).VisualBasicFullName;
			verify.AreEqual("Tests.Metadata.Generic(Of System.Int32)", name, "VB type name was not correct");
		}
	}

	[TestMethod]
	public void MetadataCache_VisualBasic_Generic_Dictionary()
	{
		using (var verify = new Verify())
		{
			var name = MetadataCache.GetMetadata(typeof(Dictionary<int, string>)).VisualBasicFullName;
			verify.AreEqual("System.Collections.Generic.Dictionary(Of System.Int32, System.String)", name, "VB type name was not correct");
		}
	}

	[TestMethod]
	public void MetadataCache_VisualBasic_Generic_Of_Generic()
	{
		using (var verify = new Verify())
		{
			var name = MetadataCache.GetMetadata(typeof(Generic<Generic<int>>)).VisualBasicFullName;
			verify.AreEqual("Tests.Metadata.Generic(Of Tests.Metadata.Generic(Of System.Int32))", name, "VB type name was not correct");
		}
	}

	[TestMethod]
	public void MetadataCache_VisualBasic_GenericNestedInGeneric()
	{
		using (var verify = new Verify())
		{
			var name = MetadataCache.GetMetadata(typeof(Generic<int>.GenericNestedInGeneric<long>)).VisualBasicFullName;
			verify.AreEqual("Tests.Metadata.Generic(Of System.Int32).GenericNestedInGeneric(Of System.Int64)", name, "VB type name was not correct");
		}
	}

	[TestMethod]
	public void MetadataCache_VisualBasic_Nested()
	{
		using (var verify = new Verify())
		{
			var name = MetadataCache.GetMetadata(typeof(Normal.Nested)).VisualBasicFullName;
			verify.AreEqual("Tests.Metadata.Normal.Nested", name, "VB type name was not correct");
		}
	}

	[TestMethod]
	public void MetadataCache_VisualBasic_NestedInGeneric()
	{
		using (var verify = new Verify())
		{
			var name = MetadataCache.GetMetadata(typeof(Generic<int>.NestedInGeneric)).VisualBasicFullName;
			verify.AreEqual("Tests.Metadata.Generic(Of System.Int32).NestedInGeneric", name, "VB type name was not correct");
		}
	}

	[TestMethod]
	public void MetadataCache_VisualBasic_Normal()
	{
		using (var verify = new Verify())
		{
			var name = MetadataCache.GetMetadata(typeof(Normal)).VisualBasicFullName;
			verify.AreEqual("Tests.Metadata.Normal", name, "VB type name was not correct");
		}
	}

	[TestMethod]
	public void CloneTest_Shallow()
	{
		var original = new Base();
		original.Property0 = 10;
		original.ChildA = new() { Property = 100, PropertyA1 = 110, PropertyAX = 120 };
		original.ChildB = new() { Property = 200, PropertyB1 = 220, PropertyBX = 230 };

		var copy = MetadataCache.Clone(original, CloneOptions.None);
		Assert.IsNotNull(copy);
		Assert.AreEqual(original.Property0, copy.Property0);
		Assert.AreSame(original.ChildA, copy.ChildA);
		Assert.AreSame(original.ChildB, copy.ChildB);
	}

	[TestMethod]
	public void CloneTest_Deep()
	{
		var original = new Base
		{
			Property0 = 10,
			ChildA = new() { Property = 100, PropertyA1 = 110, PropertyAX = 120 },
			ChildB = new() { Property = 200, PropertyB1 = 220, PropertyBX = 230 }
		};

		var copy = MetadataCache.Clone(original, CloneOptions.DeepClone);
		Assert.IsNotNull(copy);
		Assert.AreEqual(original.Property0, copy.Property0);

		Assert.AreNotSame(original.ChildA, copy.ChildA);
		Assert.AreEqual(original.ChildA.Property, copy.ChildA.Property);
		Assert.AreEqual(original.ChildA.PropertyA1, copy.ChildA.PropertyA1);
		Assert.AreEqual(original.ChildA.PropertyAX, copy.ChildA.PropertyAX);

		Assert.AreNotSame(original.ChildB, copy.ChildB);
		Assert.AreEqual(original.ChildB.Property, copy.ChildB.Property);
		Assert.AreEqual(original.ChildB.PropertyB1, copy.ChildB.PropertyB1);
		Assert.AreEqual(original.ChildB.PropertyBX, copy.ChildB.PropertyBX);
	}

	[TestMethod]
	public void CloneTest_BypassProperties()
	{
		var original = new SimplePerson()
		{
			FirstName = "A",
			LastName = "B"
		};
		original.Boss.FirstName = "Tom";
		var copy = MetadataCache.Clone(original, CloneOptions.BypassProperties);
		Assert.AreEqual(original.Boss.FirstName, copy.Boss.FirstName);
		Assert.AreSame(original.Boss, copy.Boss);
	}

	[TestMethod]
	public void CloneTest_BypassProperties_DeepClone()
	{
		var original = new SimplePerson()
		{
			FirstName = "A",
			LastName = "B"
		};
		original.Boss.FirstName = "Tom";
		var copy = MetadataCache.Clone(original, CloneOptions.BypassProperties | CloneOptions.DeepClone);
		Assert.AreEqual(original.Boss.FirstName, copy.Boss.FirstName);
		Assert.AreNotSame(original.Boss, copy.Boss);
	}

	[TestMethod]
	public void CloneTest_DeepClone_ReadonlyProperties()
	{
		var original = new SimplePerson()
		{
			FirstName = "A",
			LastName = "B"
		};
		original.Boss.FirstName = "Tom";
		var copy = MetadataCache.Clone(original, CloneOptions.DeepClone, 1);
		Assert.AreEqual(original.Boss.FirstName, copy.Boss.FirstName);
	}

	[TestMethod]
	public void SkipRecordSpecialProperties()
	{
		using (var verify = new Verify())
		{
			var result = MetadataCache.GetMetadata(typeof(BarRecord));

			verify.AreEqual(2, result.Properties.Where(p => p.CanRead).Count(), "CanRead");
			verify.AreEqual(2, result.Properties.Where(p => p.CanWrite).Count(), "CanWrite");
			verify.AreEqual(2, result.ColumnsFor.Count(), "ColumnsFor");

			verify.AreEqual(3, result.Properties.Where(p => p.CanReadRestricted).Count(), "CanReadRestricted");
			verify.AreEqual(2, result.Properties.Where(p => p.CanWriteRestricted).Count(), "CanWriteRestricted");
		}
	}

	public class AutoConstructor { }

	public class DefaultConstructor
	{
		public DefaultConstructor()
		{
		}
	}

	public class MultiConstructor
	{
		public MultiConstructor()
		{
		}

#pragma warning disable IDE0060 // Remove unused parameter

		public MultiConstructor(int x)
		{
		}

		public MultiConstructor(string x)
		{
		}

		public MultiConstructor(int a, string b)
		{
		}

#pragma warning restore IDE0060 // Remove unused parameter
	}

	public class NonDefaultConstructor
	{
#pragma warning disable IDE0060 // Remove unused parameter

		public NonDefaultConstructor(int x)
		{
		}

#pragma warning restore IDE0060 // Remove unused parameter
	}

	public class PrivateDefaultConstructor
	{
		private PrivateDefaultConstructor()
		{
		}
	}

	public class StaticConstructor
	{
		static StaticConstructor()
		{
		}
	}

	private class DefaultConstructorPrivateClass
	{
		public DefaultConstructorPrivateClass()
		{
		}
	}

	class StringIndexedMock
	{
		public bool this[string index]
		{
			get { return true; }
			set { }
		}
	}
}

public class Normal
{
	public class Nested { }
}
