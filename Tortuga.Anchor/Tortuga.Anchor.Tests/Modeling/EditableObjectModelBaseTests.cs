using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel;
using System.Runtime.Serialization;
using Tests.Mocks;
using Tortuga.Anchor.Eventing;
using Tortuga.Dragnet;

namespace Tests.Modeling;

[TestClass]
public class EditableObjectModelBaseTests
{
	[TestMethod]
	public void EditableObjectModelBase_AddHandlerNullTest()
	{
		var person = new EditablePerson();
		try
		{
			person.AddHandler(null!);
			Assert.Fail("Excepted an ArgumentNullException");
		}
		catch (ArgumentNullException ex)
		{
			Assert.AreEqual("eventHandler", ex.ParamName);
		}
	}

	[TestMethod]
	public void EditableObjectModelBase_AddRemoveHandlerTest()
	{
		var fired = false;
		var person = new EditablePerson();
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
	public void EditableObjectModelBase_BasicFunctionalityTest()
	{
		using (var verify = new Verify())
		{
			var person = new EditablePerson();
			var eventAssert = new PropertyChangedEventTest(verify, person);

			Assert.IsNull(person.FirstName);
			Assert.AreEqual("", person.LastName);

			person.FirstName = "John";

			eventAssert.ExpectUnordered("FirstName", "IsChangedLocal", "IsChanged", "FullName");

			person.LastName = "Doe";
			eventAssert.ExpectUnordered("LastName", "FullName");

			person.InvokeGoodPropertyMessage();
			eventAssert.ExpectEvent("FullName");
		}
	}

	[TestMethod]
	public void EditableObjectModelBase_BasicValidation()
	{
		var person = new EditablePerson();
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
	public void EditableObjectModelBase_ChangeTrackingTest()
	{
		var person = new EditablePerson();
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

		person.AcceptChanges();
		Assert.IsFalse(person.IsChangedLocal);
		Assert.IsFalse(person.IsChanged);

		person.FirstName = "Jimmy";
		Assert.IsTrue(person.IsChangedLocal);
		Assert.IsTrue(person.IsChanged);

		person.FirstName = "Tom";
		Assert.IsFalse(person.IsChangedLocal);
		Assert.IsFalse(person.IsChanged);
	}

	[TestMethod]
	public void EditableObjectModelBase_CtrTest()
	{
		_ = new EditablePerson();
	}

	[TestMethod]
	public void EditableObjectModelBase_EditableObjectTest()
	{
		var person = new EditablePerson
		{
			FirstName = "Albert",
			Age = 10
		};
		person.Boss.Age = 99;
		person.AcceptChanges();
		Assert.AreEqual(0, person.ChangedProperties().Count);

		Assert.IsFalse(person.IsChangedLocal);

		person.BeginEdit();
		Assert.IsFalse(person.IsChangedLocal);
		person.FirstName = "Bob";
		Assert.IsTrue(person.IsChangedLocal);
		Assert.AreEqual("Bob", person.FirstName);
		person.CancelEdit();

		Assert.AreEqual("Albert", person.FirstName);
		Assert.AreEqual(10, person.Age);
		Assert.IsFalse(person.IsChangedLocal);

		person.BeginEdit();
		Assert.IsFalse(person.IsChangedLocal);
		person.FirstName = "Chris";
		Assert.IsTrue(person.IsChangedLocal);
		Assert.AreEqual("Chris", person.FirstName);

		person.EndEdit();
		Assert.IsTrue(person.IsChangedLocal);
		Assert.AreEqual("Chris", person.FirstName);

		person.BeginEdit();
		Assert.IsTrue(person.IsChangedLocal);
		person.FirstName = "David";
		Assert.IsTrue(person.IsChangedLocal);
		Assert.AreEqual("David", person.FirstName);

		person.EndEdit();
		Assert.IsTrue(person.IsChangedLocal);
		Assert.AreEqual("David", person.FirstName);

		person.BeginEdit();
		person.BeginEdit();

		person.EndEdit();
		person.EndEdit();

		person.BeginEdit();
		person.BeginEdit();

		person.CancelEdit();
		person.CancelEdit();
	}

	[TestMethod]
	public void EditableObjectModelBase_GetFailedTest()
	{
		var person = new EditablePerson();
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
	public void EditableObjectModelBase_GetFailedTest2()
	{
		var person = new EditablePerson();
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
	public void EditableObjectModelBase_GetFailedTest3()
	{
		var person = new EditablePerson();
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
	public void EditableObjectModelBase_GetFailedTest4()
	{
		var person = new EditablePerson();
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
	public void EditableObjectModelBase_GetNewFailedTest1()
	{
		var person = new EditablePerson();
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
	public void EditableObjectModelBase_GetNewFailedTest2()
	{
		var person = new EditablePerson();
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
	public void EditableObjectModelBase_GetNewFailedTest3()
	{
		var person = new EditablePerson();
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
	public void EditableObjectModelBase_GetNewFailedTest4()
	{
		var person = new EditablePerson();
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
	public void EditableObjectModelBase_GetNewFailedTest5()
	{
		var person = new EditablePerson();
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
	public void EditableObjectModelBase_GetNewTest()
	{
		var person = new EditablePerson();

		var a = person.Boss;
		Assert.AreEqual("Da", a.FirstName);
		Assert.AreEqual("Boss", a.LastName);
		Assert.AreSame(a, person.Boss);

		var b = person.Partner;
		Assert.AreSame(b, person.Partner);
	}

	[TestMethod]
	public void EditableObjectModelBase_IsChanged_GraphTest()
	{
		using (var verify = new Verify())
		{
			var person = new EditablePerson();

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
	public void EditableObjectModelBase_MissingOriginalTest()
	{
		var person = new EditablePerson();
		person.GetPreviousValue("FirstName");
	}

	[TestMethod]
	public void EditableObjectModelBase_MultiFieldValidation()
	{
		var person = new EditablePerson
		{
			FirstName = "Tom",
			LastName = "Tom"
		};
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
	public void EditableObjectModelBase_PropertyChangedTest()
	{
		var person = new EditablePerson();
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
	public void EditableObjectModelBase_RemoveHandlerNullTest()
	{
		var person = new EditablePerson();
		try
		{
			person.RemoveHandler(null!);
			Assert.Fail("Excepted an ArgumentNullException");
		}
		catch (ArgumentNullException ex)
		{
			Assert.AreEqual("eventHandler", ex.ParamName);
		}
	}

	[TestMethod]
	public void EditableObjectModelBase_SerializationTest1()
	{
		var person = new EditablePerson() { FirstName = "Tom", LastName = "Jones" };
		person.AcceptChanges();

		var stream = new MemoryStream();
		var serializer = new DataContractSerializer(typeof(EditablePerson));
		serializer.WriteObject(stream, person);
		stream.Position = 0;
		var newPerson = (EditablePerson)serializer.ReadObject(stream)!;

		Assert.AreEqual(person.FirstName, newPerson.FirstName);
		Assert.AreEqual(person.LastName, newPerson.LastName);
		Assert.AreEqual(person.FullName, newPerson.FullName);
		Assert.AreEqual(person.IsChangedLocal, newPerson.IsChangedLocal);
	}

	[TestMethod]
	public void EditableObjectModelBase_SetFailedTest1()
	{
		var person = new EditablePerson();
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
	public void EditableObjectModelBase_SetFailedTest2()
	{
		var person = new EditablePerson();
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
	public void EditableObjectModelBase_ValidationTest()
	{
		var person = new EditablePerson();

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
	public void SimpleModelBase_ValueChangedTest()
	{
		var person = new EditablePerson();
		var firstDate = new DateTime(1980, 1, 1);
		var secondDate = new DateTime(2000, 1, 1);

		person.DateOfBirth = firstDate;
		person.DateOfBirth = secondDate;
		Assert.AreEqual(firstDate, person.PreviousDateOfBirth);
	}
}
