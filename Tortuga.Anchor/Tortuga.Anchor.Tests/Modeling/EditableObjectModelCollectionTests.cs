using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.Serialization;
using Tests.Mocks;
using Tortuga.Anchor.Eventing;
using Tortuga.Dragnet;

namespace Tests.Modeling;

[TestClass]
public class EditableObjectModelCollectionTests
{
	[TestMethod]
	public void EditableObjectModelCollection_AddHandlerNullTest()
	{
		var person = new EditablePersonCollection();
		try
		{
			person.AddHandler((IListener<PropertyChangedEventArgs>)null!);
			Assert.Fail("Excepted an ArgumentNullException");
		}
		catch (ArgumentNullException ex)
		{
			Assert.AreEqual("eventHandler", ex.ParamName);
		}
	}

	[TestMethod]
	public void EditableObjectModelCollection_AddHandlerNullTest2()
	{
		var person = new EditablePersonCollection();
		try
		{
			person.AddHandler((IListener<NotifyCollectionChangedEventArgs>)null!);
			Assert.Fail("Excepted an ArgumentNullException");
		}
		catch (ArgumentNullException ex)
		{
			Assert.AreEqual("eventHandler", ex.ParamName);
		}
	}

	[TestMethod]
	public void EditableObjectModelCollection_AddHandlerNullTest3()
	{
		var person = new EditablePersonCollection();
		try
		{
			person.AddHandler((IListener<RelayedEventArgs<PropertyChangedEventArgs>>)null!);
			Assert.Fail("Excepted an ArgumentNullException");
		}
		catch (ArgumentNullException ex)
		{
			Assert.AreEqual("eventHandler", ex.ParamName);
		}
	}

	[TestMethod]
	public void EditableObjectModelCollection_AddRemoveHandlerTest()
	{
		var fired = false;
		var person = new EditablePersonCollection();
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
	public void EditableObjectModelCollection_BasicFunctionalityTest()
	{
		using (var verify = new Verify())
		{
			var person = new EditablePersonCollection();
			var eventAssert = new PropertyChangedEventTest(verify, person);

			Assert.IsNull(person.FirstName);
			Assert.AreEqual("", person.LastName);

			person.FirstName = "John";
			eventAssert.ExpectUnordered("FirstName", "FullName", "IsChanged", "IsChangedLocal");

			person.LastName = "Doe";
			eventAssert.ExpectUnordered("LastName", "FullName");

			person.InvokeGoodPropertyMessage();
			eventAssert.ExpectEvent("FullName");
		}
	}

	[TestMethod]
	public void EditableObjectModelCollection_BasicValidation()
	{
		var person = new EditablePersonCollection();
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
	public void EditableObjectModelCollection_ChangeTrackingTest()
	{
		var people = new EditablePersonCollection();
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
		Assert.IsFalse(people.Boss.IsChangedLocal);
		Assert.IsFalse(people.Boss.IsChangedLocal);

		people.Boss.FirstName = "Frank";
		Assert.IsFalse(people.IsChangedLocal);
		Assert.IsTrue(people.IsChanged);
		Assert.IsTrue(people.Boss.IsChangedLocal);
		Assert.IsTrue(people.Boss.IsChangedLocal);
		Assert.AreEqual("Tom", people.FirstName);
		Assert.AreEqual("Frank", people.Boss.FirstName);

		people.AcceptChanges();
		Assert.IsFalse(people.IsChangedLocal);
		Assert.IsFalse(people.IsChanged);
		Assert.IsFalse(people.Boss.IsChangedLocal);
		Assert.IsFalse(people.Boss.IsChangedLocal);
		Assert.AreEqual("Tom", people.FirstName);
		Assert.AreEqual("Frank", people.Boss.FirstName);

		people.FirstName = "Harry";
		people.Boss.FirstName = "Sam";
		Assert.IsTrue(people.IsChangedLocal);
		Assert.IsTrue(people.IsChanged);
		Assert.IsTrue(people.Boss.IsChangedLocal);
		Assert.IsTrue(people.Boss.IsChangedLocal);
		Assert.AreEqual("Harry", people.FirstName);
		Assert.AreEqual("Sam", people.Boss.FirstName);

		people.RejectChanges();
		Assert.IsFalse(people.IsChangedLocal);
		Assert.IsFalse(people.IsChanged);
		Assert.IsFalse(people.Boss.IsChangedLocal);
		Assert.IsFalse(people.Boss.IsChangedLocal);
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

		people.AcceptChanges();
		Assert.IsFalse(people.IsChangedLocal);
		Assert.IsFalse(people.IsChanged);

		people.FirstName = "Jimmy";
		Assert.IsTrue(people.IsChangedLocal);
		Assert.IsTrue(people.IsChanged);

		people.FirstName = "Tom";
		Assert.IsFalse(people.IsChangedLocal);
		Assert.IsFalse(people.IsChanged);

		people.AcceptChanges();
		var last = new EditablePerson();
		people.Add(last);
		Assert.IsTrue(people.IsChangedLocal);
		Assert.IsTrue(people.IsChanged);
		people.Remove(last);
		Assert.IsFalse(people.IsChangedLocal);
		Assert.IsFalse(people.IsChanged);
	}

	[TestMethod]
	public void EditableObjectModelCollection_CollectionTest1()
	{
		var list = new List<EditablePerson>() { new EditablePerson(), new EditablePerson(), new EditablePerson() };
		var people = new EditablePersonCollection(list);
		CollectionAssert.AreEqual(list, people);

		Assert.IsFalse(people.IsChangedLocal);
		Assert.IsFalse(people.IsChanged);

		people[0].Age = 1;
		Assert.IsFalse(people.IsChangedLocal);
		Assert.IsTrue(people.IsChanged);
		Assert.IsTrue(people[0].IsChangedLocal);
		Assert.AreEqual(1, people[0].Age);

		people.AcceptChanges();
		Assert.IsFalse(people.IsChangedLocal);
		Assert.IsFalse(people.IsChanged);
		Assert.IsFalse(people[0].IsChangedLocal);
		Assert.AreEqual(1, people[0].Age);

		people[0].Age = 2;
		Assert.IsFalse(people.IsChangedLocal);
		Assert.IsTrue(people.IsChanged);
		Assert.IsTrue(people[0].IsChangedLocal);
		Assert.AreEqual(2, people[0].Age);

		people.RejectChanges();
		Assert.IsFalse(people.IsChangedLocal);
		Assert.IsFalse(people.IsChanged);
		Assert.IsFalse(people[0].IsChangedLocal);
		Assert.AreEqual(1, people[0].Age);
	}

	[TestMethod]
	public void EditableObjectModelCollection_CollectionTest2()
	{
		var people = new EditablePersonCollection();
		var list = new List<EditablePerson>();
		Assert.IsFalse(people.IsChangedLocal);
		Assert.IsFalse(people.IsChanged);

		var person1 = new EditablePerson();
		var person2 = new EditablePerson();
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
	public void EditableObjectModelCollection_CtrTest()
	{
		_ = new EditablePersonCollection();
	}

	[TestMethod]
	public void EditableObjectModelCollection_CtrTest1()
	{
		var list = new List<EditablePerson>() { new EditablePerson(), new EditablePerson(), new EditablePerson() };

		var people = new EditablePersonCollection(list);

		CollectionAssert.AreEqual(list, people);
	}

	[TestMethod]
	public void EditableObjectModelCollection_CtrTest2()
	{
		var list = new List<EditablePerson>() { new EditablePerson(), new EditablePerson(), new EditablePerson() };

		var people = new EditablePersonCollection((IEnumerable<EditablePerson>)list);

		CollectionAssert.AreEqual(list, people);
	}

	[TestMethod]
	public void EditableObjectModelCollection_EditableObjectTest()
	{
		var people = new EditablePersonCollection() { FirstName = "Albert" };
		people.Boss.Age = 99;
		people.AcceptChanges();

		Assert.IsFalse(people.IsChangedLocal);

		people.BeginEdit();
		Assert.IsFalse(people.IsChangedLocal);
		people.FirstName = "Bob";
		people.Age = 10;
		Assert.IsTrue(people.IsChangedLocal);
		Assert.AreEqual("Bob", people.FirstName);
		people.CancelEdit();

		Assert.AreEqual("Albert", people.FirstName);
		Assert.AreEqual(0, people.Age);
		Assert.IsFalse(people.IsChangedLocal);

		people.BeginEdit();
		Assert.IsFalse(people.IsChangedLocal);
		people.FirstName = "Chris";
		Assert.IsTrue(people.IsChangedLocal);
		Assert.AreEqual("Chris", people.FirstName);

		people.EndEdit();
		Assert.IsTrue(people.IsChangedLocal);
		Assert.AreEqual("Chris", people.FirstName);

		people.BeginEdit();
		Assert.IsTrue(people.IsChangedLocal);
		people.FirstName = "David";
		Assert.IsTrue(people.IsChangedLocal);
		Assert.AreEqual("David", people.FirstName);

		people.EndEdit();
		Assert.IsTrue(people.IsChangedLocal);
		Assert.AreEqual("David", people.FirstName);

		people.AcceptChanges();

		people.Add(new EditablePerson());
		Assert.AreEqual(1, people.Count);

		people.BeginEdit();
		people.Add(new EditablePerson());
		Assert.AreEqual(2, people.Count);

		people.CancelEdit();
		Assert.AreEqual(1, people.Count);

		people.BeginEdit();
		people.Add(new EditablePerson());
		Assert.AreEqual(2, people.Count);
		people.EndEdit();
		Assert.AreEqual(2, people.Count);

		people.BeginEdit();
		people.BeginEdit();

		people.EndEdit();
		people.EndEdit();

		people.BeginEdit();
		people.BeginEdit();

		people.CancelEdit();
		people.CancelEdit();
	}

	[TestMethod]
	public void EditableObjectModelCollection_GetFailedTest()
	{
		var person = new EditablePersonCollection();
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
	public void EditableObjectModelCollection_GetFailedTest2()
	{
		var person = new EditablePersonCollection();
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
	public void EditableObjectModelCollection_GetFailedTest3()
	{
		var person = new EditablePersonCollection();
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
	public void EditableObjectModelCollection_GetFailedTest4()
	{
		var person = new EditablePersonCollection();
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
	public void EditableObjectModelCollection_GetNewFailedTest1()
	{
		var person = new EditablePersonCollection();
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
	public void EditableObjectModelCollection_GetNewFailedTest2()
	{
		var person = new EditablePersonCollection();
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
	public void EditableObjectModelCollection_GetNewFailedTest3()
	{
		var person = new EditablePersonCollection();
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
	public void EditableObjectModelCollection_GetNewFailedTest4()
	{
		var person = new EditablePersonCollection();
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
	public void EditableObjectModelCollection_GetNewFailedTest5()
	{
		var person = new EditablePersonCollection();
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
	public void EditableObjectModelCollection_GetNewTest()
	{
		var person = new EditablePersonCollection();

		var a = person.Boss;
		Assert.AreEqual("Da", a.FirstName);
		Assert.AreEqual("Boss", a.LastName);
		Assert.AreSame(a, person.Boss);

		var b = person.Partner;
		Assert.AreSame(b, person.Partner);
	}

	[TestMethod]
	public void EditableObjectModelCollection_MultiFieldValidation()
	{
		var person = new EditablePersonCollection() { FirstName = "Tom", LastName = "Tom" };
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
	public void EditableObjectModelCollection_RemoveHandlerNullTest()
	{
		var person = new EditablePersonCollection();
		try
		{
			person.RemoveHandler((IListener<PropertyChangedEventArgs>)null!);
			Assert.Fail("Excepted an ArgumentNullException");
		}
		catch (ArgumentNullException ex)
		{
			Assert.AreEqual("eventHandler", ex.ParamName);
		}
	}

	[TestMethod]
	public void EditableObjectModelCollection_RemoveHandlerNullTest2()
	{
		var person = new EditablePersonCollection();
		try
		{
			person.RemoveHandler((IListener<NotifyCollectionChangedEventArgs>)null!);
			Assert.Fail("Excepted an ArgumentNullException");
		}
		catch (ArgumentNullException ex)
		{
			Assert.AreEqual("eventHandler", ex.ParamName);
		}
	}

	[TestMethod]
	public void EditableObjectModelCollection_RemoveHandlerNullTest3()
	{
		var person = new EditablePersonCollection();
		try
		{
			person.RemoveHandler((IListener<RelayedEventArgs<PropertyChangedEventArgs>>)null!);
			Assert.Fail("Excepted an ArgumentNullException");
		}
		catch (ArgumentNullException ex)
		{
			Assert.AreEqual("eventHandler", ex.ParamName);
		}
	}


	[TestMethod]
	public void EditableObjectModelCollection_SerializationTest1()
	{
		var root = new EditablePersonCollectionRoot();
		var people = root.EditablePersonCollection;
		people.FirstName = "Tom";
		people.LastName = "Jones";

		people.Add(new EditablePerson());
		people.Add(new EditablePerson());
		people.Add(new EditablePerson());

		people.AcceptChanges();

		var stream = new MemoryStream();
		var serializer = new DataContractSerializer(typeof(EditablePersonCollectionRoot));
		serializer.WriteObject(stream, root);
		stream.Position = 0;
		var newRoot = (EditablePersonCollectionRoot)serializer.ReadObject(stream)!;
		var newPeople = newRoot.EditablePersonCollection;

		//Property serialization isn't supported by the data contract serializer
		//Assert.AreEqual(people.FirstName, newPeople.FirstName);
		//Assert.AreEqual(people.LastName, newPeople.LastName);
		//Assert.AreEqual(people.FullName, newPeople.FullName);
		Assert.AreEqual(people.IsChangedLocal, newPeople.IsChangedLocal);
		Assert.AreEqual(people.Count, newPeople.Count);
	}

	[TestMethod]
	public void EditableObjectModelCollection_SetFailedTest1()
	{
		var person = new EditablePersonCollection();
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
	public void EditableObjectModelCollection_SetFailedTest2()
	{
		var person = new EditablePersonCollection();
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
	public void EditableObjectModelCollection_ValidationTest()
	{
		var person = new EditablePersonCollection();

		person.Validate();
		Assert.IsTrue(person.HasErrors);
		var errors = person.GetErrors("FirstName");
		Assert.AreEqual(1, errors.Count);

		person.FirstName = "John";
		Assert.IsFalse(person.HasErrors);
		var errors2 = person.GetErrors("FirstName");
		Assert.AreEqual(0, errors2.Count);
	}
}
