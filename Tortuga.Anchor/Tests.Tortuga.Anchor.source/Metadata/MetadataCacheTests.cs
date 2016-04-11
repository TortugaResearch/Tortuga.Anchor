using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tortuga.Dragnet;
using Tortuga.Anchor.Metadata;
using Tortuga.Anchor.Modeling;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Reflection;

#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Tests.Metadata
{

    [TestClass]
    public class MetadataCacheTests
    {
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
        public void MetadataCache_Test2()
        {
            using (var verify = new Verify())
            {
                verify.ArgumentNullException("type", () => MetadataCache.GetMetadata((Type)null));
            }
        }


        [TestMethod]
        public void MetadataCache_Test2b()
        {
            using (var verify = new Verify())
            {
                verify.ArgumentNullException("type", () => MetadataCache.GetMetadata((TypeInfo)null));
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
        public void MetadataCache_Test11()
        {
            using (var verify = new Verify())
            {
                var result = MetadataCache.GetMetadata(typeof(Mock));
                var mock = new Mock();
                var prop = result.Properties["PublicProperty"];
                prop.InvokeSet(mock, 5);
                var value = (int)prop.InvokeGet(mock);
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
                        var x = result.Properties[null];
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
                    var x = result.Properties[null];
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

        class StringIndexedMock
        {
            public bool this[string index]
            {
                get { return true; }
                set { }
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
                verify.ArgumentNullException("item", () => result.Properties.Contains((PropertyMetadata)null));
            }
        }

        [TestMethod]
        public void MetadataCache_Test18()
        {
            using (var verify = new Verify())
            {
                var result = MetadataCache.GetMetadata(typeof(Mock));
                verify.ArgumentException("propertyName", () => result.Properties.Contains((string)null));
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
        public void MetadataCache_Test29()
        {
            using (var verify = new Verify())
            {
                var result = (IEnumerable)MetadataCache.GetMetadata(typeof(Mock)).Properties;
                verify.IsNotNull(result.GetEnumerator(), "basic enumerator check");
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
                PropertyMetadata p;
                verify.IsFalse(result.Properties.TryGetValue("DOES_NOT_EXIST", out p), "TryGet expected to fail here");
                verify.IsNull(p, "TryGet failed, so this should be null.");
            }
        }

        [TestMethod]
        public void MetadataCache_Test26()
        {
            using (var verify = new Verify())
            {
                var result = MetadataCache.GetMetadata(typeof(Mock));
                PropertyMetadata p;
                verify.IsTrue(result.Properties.TryGetValue("PublicProperty", out p), "TryGet should have succeeded");
                verify.IsNotNull(p, "TryGet should have succeeded");
            }
        }

        [TestMethod]
        public void MetadataCache_Test27()
        {
            using (var verify = new Verify())
            {
                var result = MetadataCache.GetMetadata(typeof(Mock));
                PropertyMetadata p;
                verify.ArgumentException("propertyName", () => result.Properties.TryGetValue("", out p), "can't use empty strings for property name");
            }
        }

        [TestMethod]
        public void MetadataCache_Test28()
        {
            using (var verify = new Verify())
            {
                var result = MetadataCache.GetMetadata(typeof(Mock));
                PropertyMetadata p;
                verify.ArgumentException("propertyName", () => result.Properties.TryGetValue(null, out p));
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
                var result = MetadataCache.GetMetadata(typeof(Base)).ColumnsFor;
                verify.AreEqual(5, result.Length, "");
                verify.AreEqual("Property0", result[0], "");
                verify.AreEqual("PropertyA1", result[1], "");
                verify.AreEqual("PropertyA2", result[2], "");
                verify.AreEqual("BbbPropertyB1", result[3], "");
                verify.AreEqual("BbbPropertyB2", result[4], "");
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
    }

    public class Base
    {
        public int Property0 { get; set; }

        [Decompose]
        public ChildA ChildA { get; set; }
        [Decompose("Bbb")]
        public ChildB ChildB { get; set; }
    }

    public class ChildA
    {
        public int PropertyA1 { get; set; }
        [Column("PropertyA2")]
        public int Property { get; set; }
        [NotMapped]
        public int PropertyAX { get; set; }

    }

    public class ChildB
    {
        public int PropertyB1 { get; set; }
        [Column("PropertyB2")]
        public int Property { get; set; }
        [NotMapped]
        public int PropertyBX { get; set; }

    }


}
