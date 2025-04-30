using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using System.Collections.ObjectModel;
using Tortuga.Anchor;
using Tortuga.Anchor.Collections;

namespace Tests.Collections;

[TestClass()]
public class ReadOnlyMultiValueDictionaryTests
{
	[TestMethod()]
	public void ContainsKey()
	{
		var root = new MultiValueDictionary<string, int>();
		var target = root.ReadOnlyWrapper;

		Assert.IsFalse(target.ContainsKey("A"), "Expected key A to be missing");

		root.Add("A", 5);
		Assert.IsTrue(target.ContainsKey("A"), "Expected key A to be present");
		Assert.IsFalse(target.ContainsKey("B"), "Expected key B to be missing");

		root.Add("A", 10);
		Assert.IsTrue(target.ContainsKey("A"), "Expected key A to still be present when second item is added");

		root.Add("C", 15);
		Assert.IsTrue(target.ContainsKey("A"), "Expected key A to still be present after adding A");
		Assert.IsTrue(target.ContainsKey("C"), "Expected new key C to be present");

		root.Remove("C", 15);
		Assert.IsTrue(target.ContainsKey("A"), "Expected key A to still be present after removing C");
		Assert.IsFalse(target.ContainsKey("C"), "Expected key C to be removed");

		root.Remove("A", 10);
		Assert.IsTrue(target.ContainsKey("A"), "Expected previous key A to still be present after removing one item");

		root.Remove("A", 99);
		Assert.IsTrue(target.ContainsKey("A"), "Expected previous key A to still be present after trying to remove a non-existant value");

		root.Remove("A", 5);
		Assert.IsFalse(target.ContainsKey("A"), "Expected key A to be missing after removing last item");
	}

	[TestMethod()]
	public void Count()
	{
		var root = new MultiValueDictionary<string, int>();
		var target = root.ReadOnlyWrapper;
		var counter = (IReadOnlyCollection<KeyValuePair<string, ReadOnlyCollection<int>>>)target;

		Assert.AreEqual(0, counter.Count, "The collection should have been empty.");

		root.Add("A", 5);
		Assert.AreEqual(1, counter.Count, "There should be 1 after adding A,5");

		root.Add("A", 10);
		Assert.AreEqual(1, counter.Count, "There should be 1 after adding A,10. It is not a new key.");

		root.Add("C", 15);
		Assert.AreEqual(2, counter.Count, "There should be 2 after adding C,15");

		root.Remove("C", 15);
		Assert.AreEqual(1, counter.Count, "There should be 1 after removing C.");

		root.Remove("A", 10);
		Assert.AreEqual(1, counter.Count, "There should be 1, there are A's remaining");

		root.Remove("A", 99);
		Assert.AreEqual(1, counter.Count, "There should be 1, there are A's remaining");

		root.Remove("A", 5);
		Assert.AreEqual(0, counter.Count, "There should be 0, all keys were removed");
	}

	[TestMethod()]
	public void Flatten_Count()
	{
		var root = new MultiValueDictionary<string, int>();
		var target = root.ReadOnlyWrapper;

		Assert.AreEqual(0, target.Flatten.Count, "The collection should have been empty.");

		root.Add("A", 5);
		Assert.AreEqual(1, target.Flatten.Count, "There should be 1 after adding A,5");

		root.Add("A", 10);
		Assert.AreEqual(2, target.Flatten.Count, "There should be 2 after adding A,10");

		root.Add("C", 15);
		Assert.AreEqual(3, target.Flatten.Count, "There should be 3 after adding C,15");

		root.Remove("C", 15);
		Assert.AreEqual(2, target.Flatten.Count, "There should be 2 after removing C.");

		root.Remove("A", 10);
		Assert.AreEqual(1, target.Flatten.Count, "There should be 1, there are A's remaining");

		root.Remove("A", 99);
		Assert.AreEqual(1, target.Flatten.Count, "There should be 1, there are A's remaining");

		root.Remove("A", 5);
		Assert.AreEqual(0, target.Flatten.Count, "There should be 0, all keys were removed");
	}

	[TestMethod()]
	public void Flatten_GetEnumeratorTyped()
	{
		var root = new MultiValueDictionary<string, int>();
		var target = root.ReadOnlyWrapper;

		//Calling ToList will invoke GetEnumerator();
		Assert.AreEqual(0, target.Flatten.ToList().Count, "The collection should have been empty.");

		root.Add("A", 5);
		Assert.AreEqual(1, target.Flatten.ToList().Count, "There should be 1 after adding A,5");

		root.Add("A", 10);
		Assert.AreEqual(2, target.Flatten.ToList().Count, "There should be 2 after adding A,10. It is not a new key.");

		root.Add("C", 15);
		Assert.AreEqual(3, target.Flatten.ToList().Count, "There should be 3 after adding C,15");

		root.Remove("C", 15);
		Assert.AreEqual(2, target.Flatten.ToList().Count, "There should be 2 after removing C.");

		root.Remove("A", 10);
		Assert.AreEqual(1, target.Flatten.ToList().Count, "There should be 1, there are A's remaining");

		root.Remove("A", 99);
		Assert.AreEqual(1, target.Flatten.ToList().Count, "There should be 1, there are A's remaining");

		root.Remove("A", 5);
		Assert.AreEqual(0, target.Flatten.ToList().Count, "There should be 0, all keys were removed");
	}

	[TestMethod()]
	public void GetEnumeratorTyped()
	{
		var root = new MultiValueDictionary<string, int>();
		var target = root.ReadOnlyWrapper;

		//Calling ToList will invoke GetEnumerator();
		Assert.AreEqual(0, target.ToList().Count, "The collection should have been empty.");

		root.Add("A", 5);
		Assert.AreEqual(1, target.ToList().Count, "There should be 1 after adding A,5");

		root.Add("A", 10);
		Assert.AreEqual(1, target.ToList().Count, "There should be 1 after adding A,10. It is not a new key.");

		root.Add("C", 15);
		Assert.AreEqual(2, target.ToList().Count, "There should be 2 after adding C,15");

		root.Remove("C", 15);
		Assert.AreEqual(1, target.ToList().Count, "There should be 2 after removing C.");

		root.Remove("A", 10);
		Assert.AreEqual(1, target.ToList().Count, "There should be 1, there are A's remaining");

		root.Remove("A", 99);
		Assert.AreEqual(1, target.ToList().Count, "There should be 1, there are A's remaining");

		root.Remove("A", 5);
		Assert.AreEqual(0, target.ToList().Count, "There should be 0, all keys were removed");
	}

	[TestMethod()]
	public void GetEnumeratorUntyped()
	{
		var root = new MultiValueDictionary<string, int>();
		var target = root.ReadOnlyWrapper;

		int CountEnumerator()
		{
			var result = 0;
			var source = ((IEnumerable)target!).GetEnumerator();
			while (source.MoveNext())
				result += 1;
			return result;
		}

		//Calling ToList will invoke GetEnumerator();
		Assert.AreEqual(0, CountEnumerator(), "The collection should have been empty.");

		root.Add("A", 5);
		Assert.AreEqual(1, CountEnumerator(), "There should be 1 after adding A,5");

		root.Add("A", 10);
		Assert.AreEqual(1, CountEnumerator(), "There should be 1 after adding A,10. It is not a new key.");

		root.Add("C", 15);
		Assert.AreEqual(2, CountEnumerator(), "There should be 2 after adding C,15");

		root.Remove("C", 15);
		Assert.AreEqual(1, CountEnumerator(), "There should be 1 after removing C.");

		root.Remove("A", 10);
		Assert.AreEqual(1, CountEnumerator(), "There should be 1, there are A's remaining");

		root.Remove("A", 99);
		Assert.AreEqual(1, CountEnumerator(), "There should be 1, there are A's remaining");

		root.Remove("A", 5);
		Assert.AreEqual(0, CountEnumerator(), "There should be 0, all keys were removed");
	}

	[TestMethod()]
	public void Keys()
	{
		var root = new MultiValueDictionary<string, int>();
		var target = root.ReadOnlyWrapper;
		//var x = target.SelectMany(xx => xx.Value, (a, b) => KeyValuePair.Create(a.Key, b));

		Assert.AreEqual(0, target.Keys.Count(), "No keys should have been found");
		Assert.IsFalse(target.Keys.Contains("A"), "Expected key A to be missing");

		root.Add("A", 5);
		Assert.IsTrue(target.Keys.Contains("A"), "Expected key A to be present");
		Assert.IsFalse(target.Keys.Contains("B"), "Expected key B to be missing");

		root.Add("A", 10);
		Assert.IsTrue(target.Keys.Contains("A"), "Expected key A to still be present when second item is added");

		root.Add("C", 15);
		Assert.IsTrue(target.Keys.Contains("A"), "Expected key A to still be present after adding A");
		Assert.IsTrue(target.Keys.Contains("C"), "Expected key C to be present");

		root.Remove("C", 15);
		Assert.IsTrue(target.Keys.Contains("A"), "Expected key A to still be present after removing C");
		Assert.IsFalse(target.Keys.Contains("C"), "Expected key C to be removed");

		root.Remove("A", 10);
		Assert.IsTrue(target.Keys.Contains("A"), "Expected previous key A to still be present after removing one item");

		root.Remove("A", 99);
		Assert.IsTrue(target.Keys.Contains("A"), "Expected previous key A to still be present after trying to remove a non-existant value");

		root.Remove("A", 5);
		Assert.IsFalse(target.Keys.Contains("A"), "Expected key A to be missing after removing last item");
	}

	[TestMethod()]
	public void ThisGet()
	{
		var root = new MultiValueDictionary<string, int>();
		var target = root.ReadOnlyWrapper;

		try
		{
			var test1 = target["A"];
			Assert.Fail("No keys should have been found");
		}
		catch (KeyNotFoundException) { }

		root.Add("A", 5);
		var test2 = target["A"];
		Assert.IsTrue(test2.ContainsOnly(5), "Expected to find a collection containing a [5]");

		root.Add("A", 10);
		var test3 = target["A"];
		Assert.IsTrue(test3.ContainsOnly(5, 10), "Expected to find a collection containing a [5,10]");

		root.Add("C", 15);
		var test4 = target["A"];
		var test5 = target["C"];
		Assert.AreEqual(2, target.Values.Count(), "Expected 2 collection");
		Assert.IsTrue(test4.ContainsOnly(5, 10), "Expected to find a collection containing a [5,10]");
		Assert.IsTrue(test5.ContainsOnly(15), "Expected to find a collection containing a [15]");

		root.Remove("C", 15);
		var test6 = target["A"];
		try
		{
			var test7 = target["C"];
			Assert.Fail("C should have been removed");
		}
		catch (KeyNotFoundException) { }
		Assert.IsTrue(test6.ContainsOnly(5, 10), "Expected to find a collection containing a [5,10]");

		root.Remove("A", 10);
		var test8 = target["A"];
		Assert.IsTrue(test8.ContainsOnly(5), "Expected to find a collection containing a [5]");

		root.Remove("A", 99);
		var test9 = target["A"];
		Assert.IsTrue(test9.ContainsOnly(5), "Expected to find a collection containing a [5]");

		root.Remove("A", 5);
		try
		{
			var test10 = target["A"];
			Assert.Fail("A should have been removed");
		}
		catch (KeyNotFoundException) { }
	}

	[TestMethod()]
	public void TryGetValue()
	{
		var root = new MultiValueDictionary<string, int>();
		var target = root.ReadOnlyWrapper;

		Assert.IsFalse(target.TryGetValue("A", out var test1));
		Assert.IsNull(test1);

		root.Add("A", 5);
		Assert.IsTrue(target.TryGetValue("A", out var test2));
		Assert.IsTrue(test2.ContainsOnly(5), "Expected to find a collection containing a [5]");

		root.Add("A", 10);
		Assert.IsTrue(target.TryGetValue("A", out var test3));
		Assert.IsTrue(test3.ContainsOnly(5, 10), "Expected to find a collection containing a [5,10]");

		root.Add("C", 15);
		Assert.IsTrue(target.TryGetValue("A", out var test4));
		Assert.IsTrue(target.TryGetValue("C", out var test5));
		Assert.AreEqual(2, target.Values.Count(), "Expected 2 collection");
		Assert.IsTrue(test4.ContainsOnly(5, 10), "Expected to find a collection containing a [5,10]");
		Assert.IsTrue(test5.ContainsOnly(15), "Expected to find a collection containing a [15]");

		root.Remove("C", 15);
		Assert.IsTrue(target.TryGetValue("A", out var test6));
		Assert.IsFalse(target.TryGetValue("C", out var test7));
		Assert.IsNull(test7);
		Assert.IsTrue(test6.ContainsOnly(5, 10), "Expected to find a collection containing a [5,10]");

		root.Remove("A", 10);
		Assert.IsTrue(target.TryGetValue("A", out var test8));
		Assert.IsTrue(test8.ContainsOnly(5), "Expected to find a collection containing a [5]");

		root.Remove("A", 99);
		Assert.IsTrue(target.TryGetValue("A", out var test9));
		Assert.IsTrue(test9.ContainsOnly(5), "Expected to find a collection containing a [5]");

		root.Remove("A", 5);
		Assert.IsFalse(target.TryGetValue("A", out var test10));
		Assert.IsNull(test10);
	}

	[TestMethod()]
	public void Values()
	{
		var root = new MultiValueDictionary<string, int>();
		var target = root.ReadOnlyWrapper;

		Assert.AreEqual(0, target.Keys.Count(), "No keys should have been found");

		root.Add("A", 5);
		Assert.AreEqual(1, target.Values.Count(), "Expected 1 collection");
		Assert.IsTrue(target.Values.Any(x => x.ContainsOnly(5)), "Expected to find a collection containing a [5]");

		root.Add("A", 10);
		Assert.AreEqual(1, target.Values.Count(), "Expected 1 collection with 2 items");
		Assert.IsTrue(target.Values.Any(x => x.ContainsOnly(5, 10)), "Expected to find a collection containing a [5,10]");

		root.Add("C", 15);
		Assert.AreEqual(2, target.Values.Count(), "Expected 2 collection");
		Assert.IsTrue(target.Values.Any(x => x.ContainsOnly(5, 10)), "Expected to find a collection containing a [5,10]");
		Assert.IsTrue(target.Values.Any(x => x.ContainsOnly(15)), "Expected to find a collection containing a [15]");

		root.Remove("C", 15);
		Assert.AreEqual(1, target.Values.Count(), "Expected 1 collection");
		Assert.IsTrue(target.Values.Any(x => x.ContainsOnly(5, 10)), "Expected to find a collection containing a [5,10]");
		Assert.IsFalse(target.Values.Any(x => x.ContainsOnly(15)), "Expected to not find a collection containing a [15]");

		root.Remove("A", 10);
		Assert.AreEqual(1, target.Values.Count(), "Expected 1 collection");
		Assert.IsTrue(target.Values.Any(x => x.ContainsOnly(5)), "Expected to find a collection containing a [5]");

		root.Remove("A", 99);
		Assert.AreEqual(1, target.Values.Count(), "Expected 1 collection");
		Assert.IsTrue(target.Values.Any(x => x.ContainsOnly(5)), "Expected to find a collection containing a [5]");

		root.Remove("A", 5);
		Assert.AreEqual(0, target.Values.Count(), "Expected 0 collections");
	}
}
