using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Tests.Mocks;
using Tortuga.Anchor.Modeling.Internals;

namespace Tests.Modeling.Internals
{
    [TestClass]
    public class EditableObjectPropertyBagTests
    {
        EditableObjectPropertyBag GetPropertyBag()
        {
            return (new EditablePerson()).GetPropertyBag();
        }

        [TestMethod]
        public void EditableObjectPropertyBag_GetFailTest1()
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
        public void EditableObjectPropertyBag_GetFailTest2()
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
        public void EditableObjectPropertyBag_GetValueFailTest1()
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
        public void EditableObjectPropertyBag_GetValueFailTest2()
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
        public void EditableObjectPropertyBag_IsDefinedFailTest1()
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
        public void EditableObjectPropertyBag_IsDefinedFailTest2()
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
        public void EditableObjectPropertyBag_IsDefinedTest()
        {
            var person = new ChangeTrackingPerson();
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
        public void EditableObjectPropertyBag_SetFailTest1()
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
        public void EditableObjectPropertyBag_SetFailTest2()
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
        public void EditableObjectPropertyBag_SetFailTest3()
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
        public void EditableObjectPropertyBag_SetFailTest4()
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
        public void EditableObjectPropertyBag_FixCasingTest()
        {
            var person = new ChangeTrackingPerson();
            var bag = person.GetPropertyBag();

            person.FirstName = "Frank";
            person.FirstName = "Frank";
            bag.Set("Tom", PropertySetModes.FixCasing, "firstname");
            Assert.AreEqual("Tom", person.FirstName);
        }


        [TestMethod]
        public void EditableObjectPropertyBag_ConstructorFailed()
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
        public void EditableObjectPropertyBag_GetNewFailTest1()
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
        public void EditableObjectPropertyBag_GetNewFailTest2()
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
        public void EditableObjectPropertyBag_GetNewFailTest3()
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
        public void EditableObjectPropertyBag_GetNewFailTest4()
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
        public void EditableObjectPropertyBag_GetNewFailTest5()
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
        public void EditableObjectPropertyBag_GetFailTest3()
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
        public void EditableObjectPropertyBag_GetFailTest4()
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
        public void EditableObjectPropertyBag_CorruptionTest1()
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
        public void EditableObjectPropertyBag_CorruptionTest2()
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
        public void EditableObjectPropertyBag_CorruptionTest3()
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
        public void EditableObjectPropertyBag_CorruptionTest4()
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


        [TestMethod]
        public void EditableObjectPropertyBag_BeginEndTests()
        {
            var bag = GetPropertyBag();

            bag.BeginEdit();
            Assert.IsTrue(bag.IsEditing);

            bag.BeginEdit();
            Assert.IsTrue(bag.IsEditing);

            bag.EndEdit();
            Assert.IsFalse(bag.IsEditing);

            bag.EndEdit();
            Assert.IsFalse(bag.IsEditing);


            bag.BeginEdit();
            Assert.IsTrue(bag.IsEditing);

            bag.BeginEdit();
            Assert.IsTrue(bag.IsEditing);

            bag.CancelEdit();
            Assert.IsFalse(bag.IsEditing);

            bag.CancelEdit();
            Assert.IsFalse(bag.IsEditing);
        }
    }


}
