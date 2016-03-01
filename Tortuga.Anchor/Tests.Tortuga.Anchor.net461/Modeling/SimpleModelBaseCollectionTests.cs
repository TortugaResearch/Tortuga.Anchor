using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Tests.Mocks;
using Tortuga.Anchor.Eventing;
using Tortuga.Dragnet;

namespace Tests.Modeling
{
    [TestClass]
    public class SimpleModelBaseCollectionTests
    {

        [TestMethod]
        public void SimpleModelBaseCollection_AddRemoveHandlerTest()
        {
            var fired = false;
            var person = new SimplePersonCollection();
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
        public void SimpleModelBaseCollection_AddHandlerNullTest()
        {
            var person = new SimplePersonCollection();
            try
            {
                person.AddHandler((IListener<PropertyChangedEventArgs>)null);
                Assert.Fail("Excepted an ArgumentNullException");
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("eventHandler", ex.ParamName);
            }
        }

        [TestMethod]
        public void SimpleModelBaseCollection_AddHandlerNullTest2()
        {
            var person = new SimplePersonCollection();
            try
            {
                person.AddHandler((IListener<NotifyCollectionChangedEventArgs>)null);
                Assert.Fail("Excepted an ArgumentNullException");
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("eventHandler", ex.ParamName);
            }
        }

        [TestMethod]
        public void SimpleModelBaseCollection_AddHandlerNullTest3()
        {
            var person = new SimplePersonCollection();
            try
            {
                person.AddHandler((IListener<RelayedEventArgs<PropertyChangedEventArgs>>)null);
                Assert.Fail("Excepted an ArgumentNullException");
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("eventHandler", ex.ParamName);
            }
        }



        [TestMethod]
        public void SimpleModelBaseCollection_RemoveHandlerNullTest()
        {
            var person = new SimplePersonCollection();
            try
            {
                person.RemoveHandler((IListener<PropertyChangedEventArgs>)null);
                Assert.Fail("Excepted an ArgumentNullException");
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("eventHandler", ex.ParamName);
            }
        }

        [TestMethod]
        public void SimpleModelBaseCollection_RemoveHandlerNullTest2()
        {
            var person = new SimplePersonCollection();
            try
            {
                person.RemoveHandler((IListener<NotifyCollectionChangedEventArgs>)null);
                Assert.Fail("Excepted an ArgumentNullException");
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("eventHandler", ex.ParamName);
            }
        }

        [TestMethod]
        public void SimpleModelBaseCollection_RemoveHandlerNullTest3()
        {
            var person = new SimplePersonCollection();
            try
            {
                person.RemoveHandler((IListener<RelayedEventArgs<PropertyChangedEventArgs>>)null);
                Assert.Fail("Excepted an ArgumentNullException");
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("eventHandler", ex.ParamName);
            }
        }



        [TestMethod]
        public void SimpleModelBaseCollection_BasicFunctionalityTest()
        {
            using (var verify = new Verify())
            {
                var person = new SimplePersonCollection();
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

        //[TestMethod]
        //public void SimpleModelBaseCollection_PropertyChangedTest()
        //{
        //	var person = new SimplePersonCollection();
        //	try
        //	{
        //		person.InvokeBadPropertyMessage();
        //		Assert.Fail("Expected an exception");
        //	}
        //	catch (ArgumentOutOfRangeException ex)
        //	{
        //		Assert.AreEqual("propertyName", ex.ParamName);
        //		Assert.AreEqual("Boom", ex.ActualValue);
        //	}
        //}

        [TestMethod]
        public void SimpleModelBaseCollection_ValidationTest()
        {
            using (var verify = new Verify())
            {
                var person = new SimplePersonCollection();
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
        public void SimpleModelBaseCollection_ValidationTest2()
        {
            using (var verify = new Verify())
            {
                var person = new SimplePersonCollection();
                var eventAssert = new PropertyChangedEventTest(verify, person);

                person.Validate();
                Assert.IsTrue(person.HasErrors);
                eventAssert.ExpectEvent("HasErrors");
                var errors = person.GetErrors("FirstName");
                Assert.AreEqual(1, errors.Count);

                person.ClearErrors();
                Assert.IsFalse(person.HasErrors);
                eventAssert.ExpectEvent("HasErrors");
                var errors2 = person.GetErrors("FirstName");
                Assert.AreEqual(0, errors2.Count);

                person.ClearErrors();
                Assert.IsFalse(person.HasErrors);
                eventAssert.ExpectNothing();


            }
        }

        [TestMethod]
        public void SimpleModelBaseCollection_GetNewTest()
        {
            var person = new SimplePersonCollection();

            var a = person.Boss;
            Assert.AreEqual("Da", a.FirstName);
            Assert.AreEqual("Boss", a.LastName);
            Assert.AreSame(a, person.Boss);

            var b = person.Partner;
            Assert.AreSame(b, person.Partner);
        }

        [TestMethod]
        public void SimpleModelBaseCollection_CtrTest()
        {
            var employee = new SimplePersonCollection();
        }

        [TestMethod]
        public void SimpleModelBaseCollection_BasicValidation()
        {
            var person = new SimplePersonCollection();
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
        public void SimpleModelBaseCollection_MultiFieldValidation()
        {

            var person = new SimplePersonCollection();
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

            var interfacePerson = (IDataErrorInfo)person;
            Assert.IsTrue(!string.IsNullOrEmpty(interfacePerson.Error));
            Assert.IsTrue(!string.IsNullOrEmpty(interfacePerson["FirstName"]));
            Assert.IsTrue(!string.IsNullOrEmpty(interfacePerson["LastName"]));

        }

        [TestMethod]
        public void SimpleModelBaseCollection_GetFailedTest()
        {

            var person = new SimplePersonCollection();
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
        public void SimpleModelBaseCollection_GetFailedTest3()
        {

            var person = new SimplePersonCollection();
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
        public void SimpleModelBaseCollection_GetFailedTest2()
        {

            var person = new SimplePersonCollection();
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
        public void SimpleModelBaseCollection_GetFailedTest4()
        {

            var person = new SimplePersonCollection();
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
        public void SimpleModelBaseCollection_GetNewFailedTest1()
        {

            var person = new SimplePersonCollection();
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
        public void SimpleModelBaseCollection_GetNewFailedTest2()
        {

            var person = new SimplePersonCollection();
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
        public void SimpleModelBaseCollection_GetNewFailedTest3()
        {

            var person = new SimplePersonCollection();
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
        public void SimpleModelBaseCollection_GetNewFailedTest4()
        {

            var person = new SimplePersonCollection();
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
        public void SimpleModelBaseCollection_GetNewFailedTest5()
        {

            var person = new SimplePersonCollection();
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
        public void SimpleModelBaseCollection_SetFailedTest1()
        {

            var person = new SimplePersonCollection();
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
        public void SimpleModelBaseCollection_SetFailedTest2()
        {

            var person = new SimplePersonCollection();
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
        public void SimpleModelBaseCollection_CtrTest1()
        {
            var list = new List<SimplePerson>() { new SimplePerson(), new SimplePerson(), new SimplePerson() };

            var people = new SimplePersonCollection(list);

            CollectionAssert.AreEqual(list, people);
        }


        [TestMethod]
        public void SimpleModelBaseCollection_CtrTest2()
        {
            var list = new List<SimplePerson>() { new SimplePerson(), new SimplePerson(), new SimplePerson() };

            var people = new SimplePersonCollection((IEnumerable<SimplePerson>)list);

            CollectionAssert.AreEqual(list, people);
        }



        [TestMethod]
        public void SimpleModelBaseCollection_SerializationTest1()
        {
            var people = new SimplePersonCollection();
            people.FirstName = "Tom";
            people.LastName = "Jones";

            people.Add(new SimplePerson());
            people.Add(new SimplePerson());
            people.Add(new SimplePerson());

            var stream = new MemoryStream();
            var serializer = new DataContractSerializer(typeof(SimplePersonCollection));
            serializer.WriteObject(stream, people);
            stream.Position = 0;
            var newPeople = (SimplePersonCollection)serializer.ReadObject(stream);

            //Property serialization isn't supported by the data contract serializer
            //Assert.AreEqual(people.FirstName, newPeople.FirstName);
            //Assert.AreEqual(people.LastName, newPeople.LastName);
            //Assert.AreEqual(people.FullName, newPeople.FullName);
            Assert.AreEqual(people.Count, newPeople.Count);

        }

    }
}
