using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Tests.Mocks;
using Tortuga.Anchor.Modeling.Internals;

namespace Tests.Modeling.Internals
{
    [TestClass]
    public class PropertyBagTests
    {
        PropertyBag GetPropertyBag()
        {
            return (new SimplePerson()).GetPropertyBag();
        }

        [TestMethod]
        public void PropertyBag_GetFailTest1()
        {
            var bag = GetPropertyBag();
            try
            {
                bag.Get<int>(null);

                Assert.Fail("Expected an exception");
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("propertyName", ex.ParamName);
            }
        }

        [TestMethod]
        public void PropertyBag_GetFailTest2()
        {
            var bag = GetPropertyBag();
            try
            {
                bag.Get<int>("");

                Assert.Fail("Expected an exception");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("propertyName", ex.ParamName);
            }
        }

        [TestMethod]
        public void PropertyBag_GetValueFailTest1()
        {
            var bag = GetPropertyBag();
            try
            {
                bag.GetValue(null);

                Assert.Fail("Expected an exception");
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("propertyName", ex.ParamName);
            }
        }

        [TestMethod]
        public void PropertyBag_GetValueFailTest2()
        {
            var bag = GetPropertyBag();
            try
            {
                bag.GetValue("");

                Assert.Fail("Expected an exception");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("propertyName", ex.ParamName);
            }
        }

        [TestMethod]
        public void PropertyBag_IsDefinedFailTest1()
        {
            var bag = GetPropertyBag();
            try
            {
                bag.IsDefined(null);

                Assert.Fail("Expected an exception");
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("propertyName", ex.ParamName);
            }
        }

        [TestMethod]
        public void PropertyBag_IsDefinedFailTest2()
        {
            var bag = GetPropertyBag();
            try
            {
                bag.IsDefined("");

                Assert.Fail("Expected an exception");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("propertyName", ex.ParamName);
            }
        }

        [TestMethod]
        public void PropertyBag_IsDefinedTest()
        {
            var person = new SimplePerson();
            var bag = person.GetPropertyBag();
            Assert.IsFalse(bag.IsDefined("FirstName"));
            Assert.IsFalse(bag.IsDefined("FullName"));
            Assert.IsFalse(bag.IsDefined("Frank"));

            person.FirstName = "Tom";
            Assert.IsTrue(bag.IsDefined("FirstName"));
            Assert.IsFalse(bag.IsDefined("FullName"));
            Assert.IsFalse(bag.IsDefined("Frank"));

        }

        [TestMethod]
        public void PropertyBag_SetFailTest1()
        {
            var bag = GetPropertyBag();
            try
            {
                bag.Set(0, (string)null);

                Assert.Fail("Expected an exception");
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("propertyName", ex.ParamName);
            }
        }

        [TestMethod]
        public void PropertyBag_SetFailTest2()
        {
            var bag = GetPropertyBag();
            try
            {
                bag.Set(0, "");

                Assert.Fail("Expected an exception");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("propertyName", ex.ParamName);
            }
        }

        [TestMethod]
        public void PropertyBag_SetFailTest3()
        {
            var bag = GetPropertyBag();
            try
            {
                bag.Set(0, PropertySetModes.None, null);

                Assert.Fail("Expected an exception");
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("propertyName", ex.ParamName);
            }
        }

        [TestMethod]
        public void PropertyBag_SetFailTest4()
        {
            var bag = GetPropertyBag();
            try
            {
                bag.Set(0, PropertySetModes.None, "");

                Assert.Fail("Expected an exception");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("propertyName", ex.ParamName);
            }
        }


        [TestMethod]
        public void PropertyBag_FixCasingTest()
        {
            var person = new SimplePerson();
            var bag = person.GetPropertyBag();

            person.FirstName = "Frank";
            person.FirstName = "Frank";
            bag.Set("Tom", PropertySetModes.FixCasing, "firstname");
            Assert.AreEqual("Tom", person.FirstName);
        }


        [TestMethod]
        public void PropertyBag_ConstructorFailed()
        {
            try
            {
                var bag = new PropertyBag(null);

                Assert.Fail("Expected an exception");
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("owner", ex.ParamName);
            }
        }

        [TestMethod]
        public void PropertyBag_GetNewFailTest1()
        {
            var bag = GetPropertyBag();
            try
            {
                bag.GetNew<int>(null);

                Assert.Fail("Expected an exception");
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("propertyName", ex.ParamName);
            }
        }

        [TestMethod]
        public void PropertyBag_GetNewFailTest2()
        {
            var bag = GetPropertyBag();
            try
            {
                bag.GetNew<int>("");

                Assert.Fail("Expected an exception");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("propertyName", ex.ParamName);
            }
        }

        [TestMethod]
        public void PropertyBag_GetNewFailTest3()
        {
            var bag = GetPropertyBag();
            try
            {
                bag.GetNew<int>(() => 0, null);

                Assert.Fail("Expected an exception");
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("propertyName", ex.ParamName);
            }
        }

        [TestMethod]
        public void PropertyBag_GetNewFailTest4()
        {
            var bag = GetPropertyBag();
            try
            {
                bag.GetNew<int>(() => 0, "");

                Assert.Fail("Expected an exception");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("propertyName", ex.ParamName);
            }
        }

        [TestMethod]
        public void PropertyBag_GetNewFailTest5()
        {
            var bag = GetPropertyBag();
            try
            {
                bag.GetNew<int>(null, "FirstName");

                Assert.Fail("Expected an exception");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("creationFunction", ex.ParamName);
            }
        }


        [TestMethod]
        public void PropertyBag_GetFailTest3()
        {
            var bag = GetPropertyBag();
            try
            {
                bag.GetDefault<int>(0, null);

                Assert.Fail("Expected an exception");
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("propertyName", ex.ParamName);
            }
        }

        [TestMethod]
        public void PropertyBag_GetFailTest4()
        {
            var bag = GetPropertyBag();
            try
            {
                bag.GetDefault<int>(0, "");

                Assert.Fail("Expected an exception");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("propertyName", ex.ParamName);
            }
        }

        [TestMethod]
        public void PropertyBag_CorruptionTest1()
        {
            var bag = GetPropertyBag();
            bag.Set(null, "FirstName");
            try
            {
                bag.Get<int>("FirstName");

                Assert.Fail("Expected an exception");
            }
            catch (InvalidOperationException)
            {
                //pass
            }
        }

        [TestMethod]
        public void PropertyBag_CorruptionTest2()
        {
            var bag = GetPropertyBag();
            bag.Set(null, "FirstName");
            try
            {
                bag.GetDefault<int>(10, "FirstName");

                Assert.Fail("Expected an exception");
            }
            catch (InvalidOperationException)
            {
                //pass
            }
        }

        [TestMethod]
        public void PropertyBag_CorruptionTest3()
        {
            var bag = GetPropertyBag();
            bag.Set(null, "FirstName");
            try
            {
                bag.GetNew<int>("FirstName");

                Assert.Fail("Expected an exception");
            }
            catch (InvalidOperationException)
            {
                //pass
            }
        }

        [TestMethod]
        public void PropertyBag_CorruptionTest4()
        {
            var bag = GetPropertyBag();
            bag.Set(null, "FirstName");
            try
            {
                bag.GetNew<int>(() => 0, "FirstName");

                Assert.Fail("Expected an exception");
            }
            catch (InvalidOperationException)
            {
                //pass
            }
        }
    }
}
