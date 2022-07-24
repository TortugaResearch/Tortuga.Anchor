using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tortuga.Anchor.Dynamic;
using Tortuga.Dragnet;

namespace Tests.Dynamic;

[TestClass]
public class CultureAwareDynamicObjectTests
{
	[TestMethod]
	public void CultureAwareDynamicObject_Test1()
	{
		using (var verify = new Verify())
		{
			var dictionary = new CultureAwareDynamicObject(StringComparer.OrdinalIgnoreCase);
			var notify = new PropertyChangedEventTest(verify, dictionary);

			dictionary["Foo"] = 5;
			dictionary["Bar"] = "Asaa";
			notify.ExpectEvent("Foo");
			notify.ExpectEvent("Bar");

			verify.IsTrue(dictionary.ContainsKey("Foo"), "Property Foo is missing");
			verify.AreEqual(5, dictionary["Foo"], "Property Foo has the wrong value");

			dictionary["foo"] = 10;
			verify.AreEqual(10, dictionary["foo"], "Property foo has the wrong value");
			verify.AreEqual(10, dictionary["Foo"], "Property Foo has the wrong value");
		}
	}

	[TestMethod]
	public void CultureAwareDynamicObject_Test2()
	{
		using (var verify = new Verify())
		{
			var dictionary = new CultureAwareDynamicObject(StringComparer.Ordinal);
			var notify = new PropertyChangedEventTest(verify, dictionary);

			dictionary["Foo"] = 5;
			dictionary["Bar"] = "Asaa";
			notify.ExpectEvent("Foo");
			notify.ExpectEvent("Bar");

			verify.IsTrue(dictionary.ContainsKey("Foo"), "Property Foo is missing");
			verify.AreEqual(5, dictionary["Foo"], "Property Foo has the wrong value");

			dictionary["foo"] = 10;
			verify.AreEqual(10, dictionary["foo"], "Property foo has the wrong value");
			verify.AreEqual(5, dictionary["Foo"], "Property Foo has incorrectly changed");
		}
	}

	[TestMethod]
	public void CultureAwareDynamicObject_Test3()
	{
		using (var verify = new Verify())
		{
			dynamic dictionary = new CultureAwareDynamicObject(StringComparer.OrdinalIgnoreCase);
			var notify = new PropertyChangedEventTest(verify, dictionary);

			dictionary.Foo = 5;
			dictionary.Bar = "Asaa";
			notify.ExpectEvent("Foo");
			notify.ExpectEvent("Bar");

			verify.IsTrue(dictionary.ContainsKey("Foo"), "Property Foo is missing");
			verify.AreEqual(5, dictionary.Foo, "Property Foo has the wrong value");

			dictionary["foo"] = 10;
			verify.AreEqual(10, dictionary.foo, "Property foo has the wrong value");
			verify.AreEqual(5, dictionary.Foo, "Property Foo has incorrectly changed");
		}
	}

	[TestMethod]
	public void CultureAwareDynamicObject_Test4()
	{
		using (var verify = new Verify())
		{
			dynamic dictionary = new CultureAwareDynamicObject(StringComparer.Ordinal);
			var notify = new PropertyChangedEventTest(verify, dictionary);

			dictionary.Foo = 5;
			dictionary.Bar = "Asaa";
			notify.ExpectEvent("Foo");
			notify.ExpectEvent("Bar");

			verify.IsTrue(dictionary.ContainsKey("Foo"), "Property Foo is missing");
			verify.AreEqual(5, dictionary.Foo, "Property Foo has the wrong value");

			dictionary.foo = 10;
			verify.AreEqual(10, dictionary.foo, "Property foo has the wrong value");
			verify.AreEqual(5, dictionary.Foo, "Property Foo has incorrectly changed");
		}
	}
}
