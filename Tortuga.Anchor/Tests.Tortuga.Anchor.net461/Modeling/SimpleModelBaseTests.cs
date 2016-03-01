using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Tests.Mocks;
using Tortuga.Anchor.Eventing;
using Tortuga.Dragnet;

namespace Tests.Modeling
{
    [TestClass]
    public class SimpleModelBaseTests
    {
        [TestMethod]
        public void SimpleModelBase_AddRemoveHandlerTest()
        {
            var fired = false;
            var person = new SimplePerson();
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
        public void SimpleModelBase_AddHandlerNullTest()
        {
            var person = new SimplePerson();
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
        public void SimpleModelBase_RemoveHandlerNullTest()
        {
            var person = new SimplePerson();
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
        public void SimpleModelBase_BasicFunctionalityTest()
        {
            using (var verify = new Verify())
            {
                var person = new SimplePerson();
                var eventAssert = new PropertyChangedEventTest(verify, person);

                Assert.IsNull(person.FirstName);
                Assert.AreEqual("", person.LastName);

                person.FirstName = "John";
                person.LastName = "Doe";

                eventAssert.ExpectEvent("FirstName");
                eventAssert.ExpectEvent("FullName");
                eventAssert.ExpectEvent("LastName");
                eventAssert.ExpectEvent("FullName");

                person.InvokeGoodPropertyMessage();
                eventAssert.ExpectEvent("FullName");


            }
        }

        [TestMethod]
        public void SimpleModelBase_PropertyChangedTest()
        {
            var person = new SimplePerson();
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
        public void SimpleModelBase_ValidationTest()
        {
            using (var verify = new Verify())
            {
                var person = new SimplePerson();
                var eventAssert = new PropertyChangedEventTest(verify, person);

                person.Validate();
                Assert.IsTrue(person.HasErrors);
                eventAssert.ExpectEvent("HasErrors");
                var errors = person.GetErrors("FirstName");
                Assert.AreEqual(1, errors.Count);

                person.FirstName = "John";
                Assert.IsFalse(person.HasErrors);
                eventAssert.ExpectEvent("FirstName");
                eventAssert.ExpectEvent("FullName");
                eventAssert.ExpectEvent("HasErrors");

                var errors2 = person.GetErrors("FirstName");
                Assert.AreEqual(0, errors2.Count);

            }
        }

        [TestMethod]
        public void SimpleModelBase_ValidationTest2()
        {
            using (var verify = new Verify())
            {
                var person = new SimplePerson();
                var eventAssert = new PropertyChangedEventTest(verify, person);

                person.Validate();
                Assert.IsTrue(person.HasErrors);
                eventAssert.ExpectEvent("HasErrors");
                var errors = person.GetErrors("FirstName");
                Assert.AreEqual(1, errors.Count);

                person.ClearErrors();
                Assert.IsFalse(person.HasErrors);
                var errors2 = person.GetErrors("FirstName");
                Assert.AreEqual(0, errors2.Count);

            }
        }

        [TestMethod]
        public void SimpleModelBase_GetNewTest()
        {
            var person = new SimplePerson();

            var a = person.Boss;
            Assert.AreEqual("Da", a.FirstName);
            Assert.AreEqual("Boss", a.LastName);
            Assert.AreSame(a, person.Boss);

            var b = person.Partner;
            Assert.AreSame(b, person.Partner);
        }

        [TestMethod]
        public void SimpleModelBase_CtrTest()
        {
            var employee = new SimplePerson();
        }

        [TestMethod]
        public void SimpleModelBase_BasicValidation()
        {
            var person = new SimplePerson();
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

            var interfacePerson = (IDataErrorInfo)person;
            Assert.IsFalse(!string.IsNullOrEmpty(interfacePerson.Error));
            Assert.IsTrue(!string.IsNullOrEmpty(interfacePerson["FirstName"]));

            person.FirstName = "Tom";
            Assert.IsFalse(person.HasErrors);
            errors = person.GetErrors();
            Assert.AreEqual(0, errors.Count);

            errors = person.GetErrors("FirstName");
            Assert.AreEqual(0, errors.Count);

            Assert.IsFalse(!string.IsNullOrEmpty(interfacePerson.Error));
            Assert.IsFalse(!string.IsNullOrEmpty(interfacePerson["FirstName"]));

        }

        [TestMethod]
        public void SimpleModelBase_MultiFieldValidation()
        {

            var person = new SimplePerson();
            person.FirstName = "Tom";
            person.LastName = "Tom";
            IList<ValidationResult> errors = person.GetErrors("FirstName");
            Assert.AreEqual(1, errors.Count);
            Assert.IsTrue(errors[0].MemberNames.Contains("FirstName"));
            Assert.IsFalse(string.IsNullOrEmpty(errors[0].ErrorMessage));

            errors = person.GetErrors("LastName");
            Assert.AreEqual(1, errors.Count);
            Assert.IsTrue(errors[0].MemberNames.Contains("LastName"));
            Assert.IsFalse(string.IsNullOrEmpty(errors[0].ErrorMessage));

            errors = ((INotifyDataErrorInfo)person).GetErrors("LastName").Cast<ValidationResult>().ToList();
            Assert.AreEqual(1, errors.Count);
            Assert.IsTrue(errors[0].MemberNames.Contains("LastName"));
            Assert.IsFalse(string.IsNullOrEmpty(errors[0].ErrorMessage));

            errors = person.GetErrors();
            Assert.AreEqual(1, errors.Count);
            Assert.IsTrue(errors[0].MemberNames.Contains("FirstName"));
            Assert.IsTrue(errors[0].MemberNames.Contains("LastName"));
            Assert.IsFalse(string.IsNullOrEmpty(errors[0].ErrorMessage));

            var interfacePerson = (IDataErrorInfo)person;
            Assert.IsTrue(!string.IsNullOrEmpty(interfacePerson.Error));
            Assert.IsTrue(!string.IsNullOrEmpty(interfacePerson["FirstName"]));
            Assert.IsTrue(!string.IsNullOrEmpty(interfacePerson["LastName"]));

        }

        [TestMethod]
        public void SimpleModelBase_GetFailedTest()
        {

            var person = new SimplePerson();
            try
            {
                person.BadGetWithDefault();
                Assert.Fail("Expected an exception");
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("propertyName", ex.ParamName);
            }
        }

        [TestMethod]
        public void SimpleModelBase_GetFailedTest3()
        {

            var person = new SimplePerson();
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
        public void SimpleModelBase_GetFailedTest2()
        {

            var person = new SimplePerson();
            try
            {
                person.BadGet();
                Assert.Fail("Expected an exception");
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("propertyName", ex.ParamName);
            }
        }

        [TestMethod]
        public void SimpleModelBase_GetFailedTest4()
        {

            var person = new SimplePerson();
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
        public void SimpleModelBase_GetNewFailedTest1()
        {

            var person = new SimplePerson();
            try
            {
                person.BadGetNew1();
                Assert.Fail("Expected an exception");
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("propertyName", ex.ParamName);
            }
        }

        [TestMethod]
        public void SimpleModelBase_GetNewFailedTest2()
        {

            var person = new SimplePerson();
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
        public void SimpleModelBase_GetNewFailedTest3()
        {

            var person = new SimplePerson();
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
        public void SimpleModelBase_GetNewFailedTest4()
        {

            var person = new SimplePerson();
            try
            {
                person.BadGetNew4();
                Assert.Fail("Expected an exception");
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("propertyName", ex.ParamName);
            }
        }

        [TestMethod]
        public void SimpleModelBase_GetNewFailedTest5()
        {

            var person = new SimplePerson();
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
        public void SimpleModelBase_GetNewFailedTest6()
        {

            var person = new SimplePerson();
            try
            {
                person.BadGetNew6();
                Assert.Fail("Expected an exception");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("propertyName", ex.ParamName);
            }
        }

        [TestMethod]
        public void SimpleModelBase_GetNewFailedTest7()
        {

            var person = new SimplePerson();
            try
            {
                person.BadGetNew7();
                Assert.Fail("Expected an exception");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("propertyName", ex.ParamName);
            }
        }

        [TestMethod]
        public void SimpleModelBase_SetFailedTest1()
        {

            var person = new SimplePerson();
            try
            {
                person.BadSet1();
                Assert.Fail("Expected an exception");
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("propertyName", ex.ParamName);
            }
        }

        [TestMethod]
        public void SimpleModelBase_SetFailedTest2()
        {

            var person = new SimplePerson();
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
        public void SimpleModelBase_SerializationTest1()
        {
            var person = new SimplePerson();
            person.FirstName = "Tom";
            person.LastName = "Jones";

            var stream = new MemoryStream();
            var serializer = new DataContractSerializer(typeof(SimplePerson));
            serializer.WriteObject(stream, person);
            stream.Position = 0;
            var newPerson = (SimplePerson)serializer.ReadObject(stream);

            Assert.AreEqual(person.FirstName, newPerson.FirstName);
            Assert.AreEqual(person.LastName, newPerson.LastName);
            Assert.AreEqual(person.FullName, newPerson.FullName);

        }

        [TestMethod]
        public void SimpleModelBase_ValueChangedTest()
        {

            var person = new SimplePerson();
            var firstDate = new DateTime(1980, 1, 1);
            var secondDate = new DateTime(2000, 1, 1);

            person.DateOfBirth = firstDate;
            person.DateOfBirth = secondDate;
            Assert.AreEqual(firstDate, person.PreviousDateOfBirth);

        }
    }
}


