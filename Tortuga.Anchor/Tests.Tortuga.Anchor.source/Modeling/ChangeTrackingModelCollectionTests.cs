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

#if MSTest
using Microsoft.VisualStudio.TestTools.UnitTesting;
#elif WINDOWS_UWP 
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#endif

namespace Tests.Modeling
{
    [TestClass]
    public class ChangeTrackingModelCollectionTests
    {


        [TestMethod]
        public void ChangeTrackingModelCollection_AddRemoveHandlerTest()
        {
            var fired = false;
            var person = new ChangeTrackingPersonCollection();
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
        public void ChangeTrackingModelCollection_AddHandlerNullTest()
        {
            var person = new ChangeTrackingPersonCollection();
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
        public void ChangeTrackingModelCollection_AddHandlerNullTest2()
        {
            var person = new ChangeTrackingPersonCollection();
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
        public void ChangeTrackingModelCollection_AddHandlerNullTest3()
        {
            var person = new ChangeTrackingPersonCollection();
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
        public void ChangeTrackingModelCollection_RemoveHandlerNullTest()
        {
            var person = new ChangeTrackingPersonCollection();
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
        public void ChangeTrackingModelCollection_RemoveHandlerNullTest2()
        {
            var person = new ChangeTrackingPersonCollection();
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
        public void ChangeTrackingModelCollection_RemoveHandlerNullTest3()
        {
            var person = new ChangeTrackingPersonCollection();
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
        public void ChangeTrackingModelCollection_BasicFunctionalityTest()
        {
            using (var verify = new Verify())
            {
                var person = new ChangeTrackingPersonCollection();
                var eventAssert = new PropertyChangedEventTest(verify, person);

                Assert.IsNull(person.FirstName);
                Assert.AreEqual("", person.LastName);

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


            }
        }

        //[TestMethod]
        //public void ChangeTrackingModelCollection_PropertyChangedTest()
        //{
        //	var person = new ChangeTrackingSimplePersonCollection();
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
        public void ChangeTrackingModelCollection_ValidationTest()
        {
            var person = new ChangeTrackingPersonCollection();

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
        public void ChangeTrackingModelCollection_GetNewTest()
        {
            var person = new ChangeTrackingPersonCollection();

            var a = person.Boss;
            Assert.AreEqual("Da", a.FirstName);
            Assert.AreEqual("Boss", a.LastName);
            Assert.AreSame(a, person.Boss);

            var b = person.Partner;
            Assert.AreSame(b, person.Partner);
        }

        [TestMethod]
        public void ChangeTrackingModelCollection_CtrTest()
        {
            var employee = new ChangeTrackingPersonCollection();
        }

        [TestMethod]
        public void ChangeTrackingModelCollection_BasicValidation()
        {
            var person = new ChangeTrackingPersonCollection();
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
        public void ChangeTrackingModelCollection_MultiFieldValidation()
        {

            var person = new ChangeTrackingPersonCollection();
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
        public void ChangeTrackingModelCollection_GetFailedTest()
        {

            var person = new ChangeTrackingPersonCollection();
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
        public void ChangeTrackingModelCollection_GetFailedTest3()
        {

            var person = new ChangeTrackingPersonCollection();
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
        public void ChangeTrackingModelCollection_GetFailedTest2()
        {

            var person = new ChangeTrackingPersonCollection();
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
        public void ChangeTrackingModelCollection_GetFailedTest4()
        {

            var person = new ChangeTrackingPersonCollection();
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
        public void ChangeTrackingModelCollection_GetNewFailedTest1()
        {

            var person = new ChangeTrackingPersonCollection();
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
        public void ChangeTrackingModelCollection_GetNewFailedTest2()
        {

            var person = new ChangeTrackingPersonCollection();
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
        public void ChangeTrackingModelCollection_GetNewFailedTest3()
        {

            var person = new ChangeTrackingPersonCollection();
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
        public void ChangeTrackingModelCollection_GetNewFailedTest4()
        {

            var person = new ChangeTrackingPersonCollection();
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
        public void ChangeTrackingModelCollection_GetNewFailedTest5()
        {

            var person = new ChangeTrackingPersonCollection();
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
        public void ChangeTrackingModelCollection_SetFailedTest1()
        {

            var person = new ChangeTrackingPersonCollection();
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
        public void ChangeTrackingModelCollection_SetFailedTest2()
        {

            var person = new ChangeTrackingPersonCollection();
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
        public void ChangeTrackingModelCollection_CtrTest1()
        {
            var list = new List<ChangeTrackingPerson>() { new ChangeTrackingPerson(), new ChangeTrackingPerson(), new ChangeTrackingPerson() };

            var people = new ChangeTrackingPersonCollection(list);

            CollectionAssert.AreEqual(list, people);
        }


        [TestMethod]
        public void ChangeTrackingModelCollection_CtrTest2()
        {
            var list = new List<ChangeTrackingPerson>() { new ChangeTrackingPerson(), new ChangeTrackingPerson(), new ChangeTrackingPerson() };

            var people = new ChangeTrackingPersonCollection((IEnumerable<ChangeTrackingPerson>)list);

            CollectionAssert.AreEqual(list, people);
        }

        [TestMethod]
        public void ChangeTrackingModelBase_ChangeTrackingTest()
        {

            var people = new ChangeTrackingPersonCollection();
            people.RejectChanges();

            Assert.IsNotNull(people.Boss);
            Assert.IsFalse(people.IsChangedLocal);
            Assert.IsFalse(people.IsChanged);

            people.Age = 100;
            Assert.IsTrue(people.IsChangedLocal);
            Assert.IsTrue(people.IsChanged);
            people.RejectChanges();

            Assert.AreEqual(0, people.Age);
            Assert.IsFalse(people.IsChangedLocal);
            Assert.IsFalse(people.IsChanged);


            people.FirstName = "Tom";
            Assert.IsTrue(people.IsChangedLocal);
            Assert.IsTrue(people.IsChanged);

            people.AcceptChanges();
            Assert.IsFalse(people.IsChangedLocal);
            Assert.IsFalse(people.IsChanged);
            Assert.IsFalse(people.Boss.IsChanged);
            Assert.IsFalse(people.Boss.IsChanged);

            people.Boss.FirstName = "Frank";
            Assert.IsFalse(people.IsChangedLocal);
            Assert.IsTrue(people.IsChanged);
            Assert.IsTrue(people.Boss.IsChanged);
            Assert.IsTrue(people.Boss.IsChanged);
            Assert.AreEqual("Tom", people.FirstName);
            Assert.AreEqual("Frank", people.Boss.FirstName);

            people.AcceptChanges();
            Assert.IsFalse(people.IsChangedLocal);
            Assert.IsFalse(people.IsChanged);
            Assert.IsFalse(people.Boss.IsChanged);
            Assert.IsFalse(people.Boss.IsChanged);
            Assert.AreEqual("Tom", people.FirstName);
            Assert.AreEqual("Frank", people.Boss.FirstName);

            people.FirstName = "Harry";
            people.Boss.FirstName = "Sam";
            Assert.IsTrue(people.IsChangedLocal);
            Assert.IsTrue(people.IsChanged);
            Assert.IsTrue(people.Boss.IsChanged);
            Assert.IsTrue(people.Boss.IsChanged);
            Assert.AreEqual("Harry", people.FirstName);
            Assert.AreEqual("Sam", people.Boss.FirstName);

            people.RejectChanges();
            Assert.IsFalse(people.IsChangedLocal);
            Assert.IsFalse(people.IsChanged);
            Assert.IsFalse(people.Boss.IsChanged);
            Assert.IsFalse(people.Boss.IsChanged);
            Assert.AreEqual("Tom", people.FirstName);
            Assert.AreEqual("Frank", people.Boss.FirstName);

            people.DummyObject.IsChanged = true;
            Assert.IsFalse(people.IsChangedLocal);
            Assert.IsTrue(people.IsChanged);

            people.AcceptChanges();
            Assert.IsFalse(people.IsChangedLocal);
            Assert.IsFalse(people.IsChanged);

            people.DummyObject.IsChanged = true;
            Assert.IsFalse(people.IsChangedLocal);
            Assert.IsTrue(people.IsChanged);

            people.RejectChanges();
            Assert.IsFalse(people.IsChangedLocal);
            Assert.IsFalse(people.IsChanged);


            people.DummyObject.IsChanged = true;
            Assert.IsFalse(people.IsChangedLocal);
            Assert.IsTrue(people.IsChanged);

            people.AcceptChangesLocal();
            Assert.IsFalse(people.IsChangedLocal);
            Assert.IsTrue(people.IsChanged);

        }

        [TestMethod]
        public void ChangeTrackingModelCollection_CollectionTest1()
        {
            var list = new List<ChangeTrackingPerson>() { new ChangeTrackingPerson(), new ChangeTrackingPerson(), new ChangeTrackingPerson() };
            var people = new ChangeTrackingPersonCollection(list);
            CollectionAssert.AreEqual(list, people);

            Assert.IsFalse(people.IsChangedLocal);
            Assert.IsFalse(people.IsChanged);

            people[0].Age = 1;
            Assert.IsFalse(people.IsChangedLocal);
            Assert.IsTrue(people.IsChanged);
            Assert.IsTrue(people[0].IsChanged);
            Assert.AreEqual(1, people[0].Age);

            people.AcceptChanges();
            Assert.IsFalse(people.IsChangedLocal);
            Assert.IsFalse(people.IsChanged);
            Assert.IsFalse(people[0].IsChanged);
            Assert.AreEqual(1, people[0].Age);

            people[0].Age = 2;
            Assert.IsFalse(people.IsChangedLocal);
            Assert.IsTrue(people.IsChanged);
            Assert.IsTrue(people[0].IsChanged);
            Assert.AreEqual(2, people[0].Age);

            people.RejectChanges();
            Assert.IsFalse(people.IsChangedLocal);
            Assert.IsFalse(people.IsChanged);
            Assert.IsFalse(people[0].IsChanged);
            Assert.AreEqual(1, people[0].Age);
        }

        [TestMethod]
        public void ChangeTrackingModelCollection_CollectionTest2()
        {
            var people = new ChangeTrackingPersonCollection();
            var list = new List<ChangeTrackingPerson>();
            Assert.IsFalse(people.IsChangedLocal);
            Assert.IsFalse(people.IsChanged);

            var person1 = new ChangeTrackingPerson();
            var person2 = new ChangeTrackingPerson();
            //var person3 = new ChangeTrackingSimplePerson();

            people.Add(person1);
            list.Add(person1);

            CollectionAssert.AreEqual(list, people);
            Assert.IsTrue(people.IsChangedLocal);

            people.AcceptChanges();
            CollectionAssert.AreEqual(list, people);
            Assert.IsFalse(people.IsChangedLocal);

            people.Add(person2);
            list.Add(person2);

            CollectionAssert.AreEqual(list, people);
            Assert.IsTrue(people.IsChangedLocal);

            people.RejectChanges();
            list.Remove(person2);

            CollectionAssert.AreEqual(list, people);
            Assert.IsFalse(people.IsChangedLocal);

            people.Remove(person1);
            list.Remove(person1);


            CollectionAssert.AreEqual(list, people);
            Assert.IsTrue(people.IsChangedLocal);

            people.AcceptChanges();
            CollectionAssert.AreEqual(list, people);
            Assert.IsFalse(people.IsChangedLocal);


        }

        [TestMethod]
        public void ChangeTrackingModelCollection_SerializationTest1()
        {
            var root = new ChangeTrackingPersonCollectionRoot();
            var people = root.ChangeTrackingPersonCollection;
            people.FirstName = "Tom";
            people.LastName = "Jones";

            people.Add(new ChangeTrackingPerson());
            people.Add(new ChangeTrackingPerson());
            people.Add(new ChangeTrackingPerson());

            people.AcceptChanges();

            var stream = new MemoryStream();
            var serializer = new DataContractSerializer(typeof(ChangeTrackingPersonCollectionRoot));
            serializer.WriteObject(stream, root);
            stream.Position = 0;
            var newRoot = (ChangeTrackingPersonCollectionRoot)serializer.ReadObject(stream);
            var newPeople = newRoot.ChangeTrackingPersonCollection;// (ChangeTrackingPersonCollection)serializer.ReadObject(stream);

            //Property serialization isn't supported by the data contract serializer
            //Assert.AreEqual(people.FirstName, newPeople.FirstName);
            //Assert.AreEqual(people.LastName, newPeople.LastName);
            //Assert.AreEqual(people.FullName, newPeople.FullName);
            Assert.AreEqual(people.IsChangedLocal, newPeople.IsChangedLocal);
            Assert.AreEqual(people.Count, newPeople.Count);

        }
    }
}

