using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Tests.Mocks;
using Tortuga.Anchor.Eventing;
using Tortuga.Dragnet;

#if MSTest
using Microsoft.VisualStudio.TestTools.UnitTesting;
#elif WINDOWS_UWP 
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#endif

namespace Tests.Modeling
{


    /// <summary>
    ///This is a test class for ModelBaseTest and is intended
    ///to contain all ModelBaseTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ChangeTrackingModelBaseTests
    {


        [TestMethod]
        public void ChangeTrackingModelBase_AddRemoveHandlerTest()
        {
            var fired = false;
            var person = new ChangeTrackingPerson();
            var listener = new Listener<PropertyChangedEventArgs>((sender, e) => { fired = true; });
            person.ErrorsChanged += (sender, e) => { };
            person.AddHandler(listener);
            person.FirstName = "Tom";
            Assert.IsTrue(fired);
            fired = false;
            person.RemoveHandler(listener);
            person.FirstName = "Sam";
            Assert.IsFalse(fired);
        }

        [TestMethod]
        public void ChangeTrackingModelBase_AddHandlerNullTest()
        {
            var person = new ChangeTrackingPerson();
            try
            {
                person.AddHandler(null);
                Assert.Fail("Excepted an ArgumentNullException");
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("eventHandler", ex.ParamName);
            }
        }

        [TestMethod]
        public void ChangeTrackingModelBase_RemoveHandlerNullTest()
        {
            var person = new ChangeTrackingPerson();
            try
            {
                person.RemoveHandler(null);
                Assert.Fail("Excepted an ArgumentNullException");
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("eventHandler", ex.ParamName);
            }
        }


        [TestMethod]
        public void ChangeTrackingModelBase_BasicFunctionalityTest()
        {
            using (var verify = new Verify())
            {
                var person = new ChangeTrackingPerson();
                var eventAssert = new PropertyChangedEventTest(verify, person);

                Assert.IsNull(person.FirstName);
                Assert.AreEqual("", person.LastName);

                eventAssert.ExpectNothing();

                person.FirstName = "John";

                eventAssert.ExpectEvent("FirstName");
                eventAssert.ExpectEvent("FullName");
                eventAssert.ExpectEvent("IsChangedLocal");
                eventAssert.ExpectEvent("IsChanged");

                person.LastName = "Doe";

                eventAssert.ExpectEvent("LastName");
                eventAssert.ExpectEvent("FullName");

                person.InvokeGoodPropertyMessage();
                eventAssert.ExpectEvent("FullName");

                person.InvokeAllPropertyMessage();
                eventAssert.ExpectEvent("");

            }
        }

        [TestMethod]
        public void ChangeTrackingModelBase_PropertyChangedTest()
        {
            var person = new ChangeTrackingPerson();
            try
            {
                person.InvokeBadPropertyMessage();
                Assert.Fail("Expected an exception");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Assert.AreEqual("propertyName", ex.ParamName);
                Assert.AreEqual("Boom", ex.ActualValue);
            }
        }

        [TestMethod]
        public void ChangeTrackingModelBase_ValidationTest()
        {
            var person = new ChangeTrackingPerson();

            person.Validate();
            Assert.IsTrue(person.HasErrors);
            var errors = person.GetErrors("FirstName");
            Assert.AreEqual(1, errors.Count);

            person.FirstName = "John";
            Assert.IsFalse(person.HasErrors);
            var errors2 = person.GetErrors("FirstName");
            Assert.AreEqual(0, errors2.Count);

        }

        [TestMethod]
        public void ChangeTrackingModelBase_GetNewTest()
        {
            var person = new ChangeTrackingPerson();

            var a = person.Boss;
            Assert.AreEqual("Da", a.FirstName);
            Assert.AreEqual("Boss", a.LastName);
            Assert.AreSame(a, person.Boss);

            var b = person.Partner;
            Assert.AreSame(b, person.Partner);
        }

        [TestMethod]
        public void ChangeTrackingModelBase_CtrTest()
        {
            var employee = new ChangeTrackingPerson();
        }

        [TestMethod]
        public void ChangeTrackingModelBase_BasicValidation()
        {
            var person = new ChangeTrackingPerson();
            Assert.IsFalse(person.HasErrors);
            var errors = person.GetErrors();
            Assert.AreEqual(0, errors.Count);
            errors = person.GetErrors("");
            Assert.AreEqual(0, errors.Count);
            errors = person.GetErrors(null);
            Assert.AreEqual(0, errors.Count);

            person.Validate();

            Assert.IsTrue(person.HasErrors);
            errors = person.GetErrors();
            Assert.AreEqual(0, errors.Count);

            errors = person.GetErrors("FirstName");
            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual("FirstName", errors[0].MemberNames.First());
            Assert.IsFalse(string.IsNullOrEmpty(errors[0].ErrorMessage));

#if !WINDOWS_UWP 
            var interfacePerson = (IDataErrorInfo)person;
            Assert.IsFalse(!string.IsNullOrEmpty(interfacePerson.Error));
            Assert.IsTrue(!string.IsNullOrEmpty(interfacePerson["FirstName"]));
#endif
            person.FirstName = "Tom";
            Assert.IsFalse(person.HasErrors);
            errors = person.GetErrors();
            Assert.AreEqual(0, errors.Count);

            errors = person.GetErrors("FirstName");
            Assert.AreEqual(0, errors.Count);

#if !WINDOWS_UWP 
            Assert.IsFalse(!string.IsNullOrEmpty(interfacePerson.Error));
            Assert.IsFalse(!string.IsNullOrEmpty(interfacePerson["FirstName"]));
#endif
        }

        [TestMethod]
        public void ChangeTrackingModelBase_MultiFieldValidation()
        {

            var person = new ChangeTrackingPerson();
            person.FirstName = "Tom";
            person.LastName = "Tom";
            var errors = person.GetErrors("FirstName");
            Assert.AreEqual(1, errors.Count);
            Assert.IsTrue(errors[0].MemberNames.Contains("FirstName"));
            Assert.IsFalse(string.IsNullOrEmpty(errors[0].ErrorMessage));

            errors = person.GetErrors("LastName");
            Assert.AreEqual(1, errors.Count);
            Assert.IsTrue(errors[0].MemberNames.Contains("LastName"));
            Assert.IsFalse(string.IsNullOrEmpty(errors[0].ErrorMessage));

            errors = person.GetErrors();
            Assert.AreEqual(1, errors.Count);
            Assert.IsTrue(errors[0].MemberNames.Contains("FirstName"));
            Assert.IsTrue(errors[0].MemberNames.Contains("LastName"));
            Assert.IsFalse(string.IsNullOrEmpty(errors[0].ErrorMessage));

#if !WINDOWS_UWP 
            var interfacePerson = (IDataErrorInfo)person;
            Assert.IsTrue(!string.IsNullOrEmpty(interfacePerson.Error));
            Assert.IsTrue(!string.IsNullOrEmpty(interfacePerson["FirstName"]));
            Assert.IsTrue(!string.IsNullOrEmpty(interfacePerson["LastName"]));
#endif
        }

        [TestMethod]
        public void ChangeTrackingModelBase_GetFailedTest()
        {

            var person = new ChangeTrackingPerson();
            try
            {
                person.BadGetWithDefault();
                Assert.Fail("Expected an exception");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("propertyName", ex.ParamName);
            }
        }

        [TestMethod]
        public void ChangeTrackingModelBase_GetFailedTest3()
        {

            var person = new ChangeTrackingPerson();
            try
            {
                person.BadGetWithDefault2();
                Assert.Fail("Expected an exception");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("propertyName", ex.ParamName);
            }
        }

        [TestMethod]
        public void ChangeTrackingModelBase_GetFailedTest2()
        {

            var person = new ChangeTrackingPerson();
            try
            {
                person.BadGet();
                Assert.Fail("Expected an exception");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("propertyName", ex.ParamName);
            }
        }

        [TestMethod]
        public void ChangeTrackingModelBase_GetFailedTest4()
        {

            var person = new ChangeTrackingPerson();
            try
            {
                person.BadGet2();
                Assert.Fail("Expected an exception");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("propertyName", ex.ParamName);
            }
        }

        [TestMethod]
        public void ChangeTrackingModelBase_GetNewFailedTest1()
        {

            var person = new ChangeTrackingPerson();
            try
            {
                person.BadGetNew1();
                Assert.Fail("Expected an exception");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("propertyName", ex.ParamName);
            }
        }

        [TestMethod]
        public void ChangeTrackingModelBase_GetNewFailedTest2()
        {

            var person = new ChangeTrackingPerson();
            try
            {
                person.BadGetNew2();
                Assert.Fail("Expected an exception");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("propertyName", ex.ParamName);
            }
        }

        [TestMethod]
        public void ChangeTrackingModelBase_GetNewFailedTest3()
        {

            var person = new ChangeTrackingPerson();
            try
            {
                person.BadGetNew3();
                Assert.Fail("Expected an exception");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("creationFunction", ex.ParamName);
            }
        }

        [TestMethod]
        public void ChangeTrackingModelBase_GetNewFailedTest4()
        {

            var person = new ChangeTrackingPerson();
            try
            {
                person.BadGetNew4();
                Assert.Fail("Expected an exception");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("propertyName", ex.ParamName);
            }
        }

        [TestMethod]
        public void ChangeTrackingModelBase_GetNewFailedTest5()
        {

            var person = new ChangeTrackingPerson();
            try
            {
                person.BadGetNew5();
                Assert.Fail("Expected an exception");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("propertyName", ex.ParamName);
            }
        }

        [TestMethod]
        public void ChangeTrackingModelBase_SetFailedTest1()
        {

            var person = new ChangeTrackingPerson();
            try
            {
                person.BadSet1();
                Assert.Fail("Expected an exception");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("propertyName", ex.ParamName);
            }
        }

        [TestMethod]
        public void ChangeTrackingModelBase_SetFailedTest2()
        {

            var person = new ChangeTrackingPerson();
            try
            {
                person.BadSet2();
                Assert.Fail("Expected an exception");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("propertyName", ex.ParamName);
            }
        }


        [TestMethod]
        public void ChangeTrackingModelBase_ChangeTrackingTest()
        {

            var person = new ChangeTrackingPerson();
            person.RejectChanges();

            Assert.IsNotNull(person.Boss);
            Assert.IsFalse(person.IsChangedLocal);
            Assert.IsFalse(person.IsChanged);

            person.Age = 100;
            Assert.IsTrue(person.IsChangedLocal);
            Assert.IsTrue(person.IsChanged);
            person.RejectChanges();

            Assert.AreEqual(0, person.Age);
            Assert.IsFalse(person.IsChangedLocal);
            Assert.IsFalse(person.IsChanged);


            person.FirstName = "Tom";
            Assert.IsTrue(person.IsChangedLocal);
            Assert.IsTrue(person.IsChanged);

            person.AcceptChanges();
            Assert.IsFalse(person.IsChangedLocal);
            Assert.IsFalse(person.IsChanged);
            Assert.IsFalse(person.Boss.IsChangedLocal);
            Assert.IsFalse(person.Boss.IsChanged);

            person.Boss.FirstName = "Frank";
            Assert.IsFalse(person.IsChangedLocal);
            Assert.IsTrue(person.IsChanged);
            Assert.IsTrue(person.Boss.IsChangedLocal);
            Assert.IsTrue(person.Boss.IsChanged);
            Assert.AreEqual("Tom", person.FirstName);
            Assert.AreEqual("Frank", person.Boss.FirstName);

            person.AcceptChanges();
            Assert.IsFalse(person.IsChangedLocal);
            Assert.IsFalse(person.IsChanged);
            Assert.IsFalse(person.Boss.IsChangedLocal);
            Assert.IsFalse(person.Boss.IsChanged);
            Assert.AreEqual("Tom", person.FirstName);
            Assert.AreEqual("Frank", person.Boss.FirstName);

            person.FirstName = "Harry";
            person.Boss.FirstName = "Sam";
            Assert.IsTrue(person.IsChangedLocal);
            Assert.IsTrue(person.IsChanged);
            Assert.IsTrue(person.Boss.IsChangedLocal);
            Assert.IsTrue(person.Boss.IsChanged);
            Assert.AreEqual("Harry", person.FirstName);
            Assert.AreEqual("Sam", person.Boss.FirstName);

            person.RejectChanges();
            Assert.IsFalse(person.IsChangedLocal);
            Assert.IsFalse(person.IsChanged);
            Assert.IsFalse(person.Boss.IsChangedLocal);
            Assert.IsFalse(person.Boss.IsChanged);
            Assert.AreEqual("Tom", person.FirstName);
            Assert.AreEqual("Frank", person.Boss.FirstName);

            person.DummyObject.IsChanged = true;
            Assert.IsFalse(person.IsChangedLocal);
            Assert.IsTrue(person.IsChanged);

            person.AcceptChanges();
            Assert.IsFalse(person.IsChangedLocal);
            Assert.IsFalse(person.IsChanged);

            person.DummyObject.IsChanged = true;
            Assert.IsFalse(person.IsChangedLocal);
            Assert.IsTrue(person.IsChanged);

            person.RejectChanges();
            Assert.IsFalse(person.IsChangedLocal);
            Assert.IsFalse(person.IsChanged);


            person.DummyObject.IsChanged = true;
            Assert.IsFalse(person.IsChangedLocal);
            Assert.IsTrue(person.IsChanged);

            person.AcceptChangesLocal();
            Assert.IsFalse(person.IsChangedLocal);
            Assert.IsTrue(person.IsChanged);

        }


        [TestMethod]
        public void ChangeTrackingModelBase_SerializationTest1()
        {
            var person = new ChangeTrackingPerson();
            person.FirstName = "Tom";
            person.LastName = "Jones";
            person.AcceptChanges();

            var stream = new MemoryStream();
            var serializer = new DataContractSerializer(typeof(ChangeTrackingPerson));
            serializer.WriteObject(stream, person);
            stream.Position = 0;
            var newPerson = (ChangeTrackingPerson)serializer.ReadObject(stream);

            Assert.AreEqual(person.FirstName, newPerson.FirstName);
            Assert.AreEqual(person.LastName, newPerson.LastName);
            Assert.AreEqual(person.FullName, newPerson.FullName);
            Assert.AreEqual(person.IsChangedLocal, newPerson.IsChangedLocal);
        }

        [TestMethod]
        public void ChangeTrackingModelBase_IsChanged_GraphTest()
        {
            using (var verify = new Verify())
            {
                var person = new ChangeTrackingPerson();

                Assert.IsNotNull(person.Boss);

                var eventAssert = new PropertyChangedEventTest(verify, person);
                var eventAssert2 = new PropertyChangedEventTest(verify, person.Boss);

                person.Boss.FirstName = "Tom";
                eventAssert2.ExpectEvent("FirstName");
                eventAssert2.ExpectEvent("FullName");
                eventAssert2.ExpectEvent("IsChangedLocal");
                eventAssert2.ExpectEvent("IsChanged");

                eventAssert.ExpectEvent("IsChanged");

                person.Boss.FirstName = "Tim";
                eventAssert2.ExpectEvent("FirstName");
                eventAssert2.ExpectEvent("FullName");
                eventAssert2.ExpectNothing();
                eventAssert.ExpectNothing();


            }
        }

        [TestMethod]
        public void SimpleModelBase_ValueChangedTest()
        {

            var person = new ChangeTrackingPerson();
            var firstDate = new DateTime(1980, 1, 1);
            var secondDate = new DateTime(2000, 1, 1);

            person.DateOfBirth = firstDate;
            person.DateOfBirth = secondDate;
            Assert.AreEqual(firstDate, person.PreviousDateOfBirth);

        }

        [TestMethod]
        public void ChangeTrackingModelBase_MissingOriginalTest()
        {
            var person = new ChangeTrackingPerson();
            var value = person.GetPreviousValue("FirstName");
        }

    }
}
