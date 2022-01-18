using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Tortuga.Anchor;
using Tortuga.Dragnet;

namespace Tests.Collections
{
	[TestClass()]
	public class CollectionUtilitiesTests
	{
		[TestMethod()]
		public void AddRange_Test1()
		{
			using (var verify = new Verify())
			{
				List<string> target = new List<string>();
				List<string> list = null;
				verify.ArgumentNullException("list", () => CollectionUtilities.AddRange(target, list));
			}
		}

		[TestMethod()]
		public void AddRange_Test10()
		{
			using (var verify = new Verify())
			{
				IEnumerable<int> list = new List<int> { 1, 2, 3 };
				ICollection<int> target = new ReadOnlyCollection<int>(new List<int>());
				verify.ArgumentException("target", () => CollectionUtilities.AddRange(target, list), "read-only list");
				verify.ArgumentException("target", () => CollectionUtilities.AddRange(target, 1, 2, 3), "read-only list");
			}
		}

		[TestMethod()]
		public void AddRange_Test11()
		{
			using (var verify = new Verify())
			{
				var list = (new List<int> { 1, 2, 3 }).Where(x => true);
				ICollection<int> target = new ReadOnlyCollection<int>(new List<int>());
				verify.ArgumentException("target", () => CollectionUtilities.AddRange(target, list), "read-only list");
				verify.ArgumentException("target", () => CollectionUtilities.AddRange(target, 1, 2, 3), "read-only list");
			}
		}

		[TestMethod()]
		public void AddRange_Test2()
		{
			using (var verify = new Verify())
			{
				List<string> target = null;
				List<string> list = null;
				verify.ArgumentNullException("target", () => CollectionUtilities.AddRange(target, list));
			}
		}

		[TestMethod()]
		public void AddRange_Test3()
		{
			using (var verify = new Verify())
			{
				List<string> target = new List<string>();
				List<string> list = new List<string>();
				target.Add("AAA");
				list.Add("BBB");
				list.Add("CCC");

				CollectionUtilities.AddRange(target, list);
				verify.AreEqual(3, target.Count, "AddRange should have added 3 items");
				verify.ItemsAreEqual(new[] { "AAA", "BBB", "CCC" }, target, "AddRange should have added 3 items");
			}
		}

		[TestMethod()]
		public void AddRange_Test3b()
		{
			using (var verify = new Verify())
			{
				List<string> target = new List<string>();
				List<string> list = new List<string>();
				target.Add("AAA");
				list.Add("BBB");
				list.Add("CCC");

				CollectionUtilities.AddRange((ICollection<string>)target, (IEnumerable<string>)list);
				verify.AreEqual(3, target.Count, "AddRange should have added 3 items");
				verify.ItemsAreEqual(new[] { "AAA", "BBB", "CCC" }, target, "AddRange should have added 3 items");
			}
		}

		[TestMethod()]
		public void AddRange_Test4()
		{
			using (var verify = new Verify())
			{
				List<string> target = new List<string>();
				List<string> list = new List<string>();
				target.Add("AAA");
				target.Add("BBB");
				target.Add("CCC");

				CollectionUtilities.AddRange(target, list);
				verify.ItemsAreEqual(new[] { "AAA", "BBB", "CCC" }, target, "AddRange should have added 3 items");
			}
		}

		[TestMethod()]
		public void AddRange_Test5()
		{
			using (var verify = new Verify())
			{
				List<string> target = new List<string>();
				List<string> list = new List<string>();
				list.Add("AAA");
				list.Add("BBB");
				list.Add("CCC");

				CollectionUtilities.AddRange(target, list);
				verify.ItemsAreEqual(new[] { "AAA", "BBB", "CCC" }, target, "AddRange should have added 3 items");
			}
		}

		[TestMethod()]
		public void AddRange_Test6()
		{
			using (var verify = new Verify())
			{
				List<string> target = new List<string>();

				CollectionUtilities.AddRange(target, "AAA", "BBB", "CCC");
				verify.ItemsAreEqual(new[] { "AAA", "BBB", "CCC" }, target, "AddRange should have added 3 items");
			}
		}

		[TestMethod()]
		public void AddRange_Test7()
		{
			using (var verify = new Verify())
			{
				List<string> target = new List<string>();
				string[] list = null;
				verify.ArgumentNullException("list", () => CollectionUtilities.AddRange(target, list));
			}
		}

		[TestMethod()]
		public void AddRange_Test8()
		{
			using (var verify = new Verify())
			{
				List<string> target = null;
				string[] list = null;
				verify.ArgumentNullException("target", () => CollectionUtilities.AddRange(target, list));
			}
		}

		[TestMethod()]
		public void AddRange_Test9()
		{
			using (var verify = new Verify())
			{
				var list = new List<int> { 1, 2, 3 };
				ICollection<int> target = new ReadOnlyCollection<int>(new List<int>());
				verify.ArgumentException("target", () => CollectionUtilities.AddRange(target, list), "read-only list");
				verify.ArgumentException("target", () => CollectionUtilities.AddRange(target, 1, 2, 3), "read-only list");
			}
		}

		[TestMethod]
		public void AsList_Test1()
		{
			var x = (IEnumerable<int>)new List<int>();
			var y = x.AsList();
			Assert.AreEqual(x, y);
		}

		[TestMethod]
		public void AsList_Test2()
		{
			var x = new Collection<int>() { 1, 2, 3, 4, 5 };
			var y = x.AsList();
			Assert.AreEqual(x, y);
		}

		[TestMethod]
		public void AsList_Test3()
		{
			var x = Enumerable.Range(1, 5);
			var y = x.AsList();
			Assert.AreNotEqual(x, y);
			Assert.AreEqual(x.Count(), y.Count);
		}

		[TestMethod]
		public void AsReadOnlyCollection_Test1()
		{
			var x = (IEnumerable<int>)new List<int>();
			var y = x.AsReadOnlyCollection();
			Assert.AreEqual(x, y);
		}

		[TestMethod]
		public void AsReadOnlyCollection_Test2()
		{
			var x = (IEnumerable<int>)new Collection<int>();
			var y = x.AsReadOnlyCollection();
			Assert.AreEqual(x, y);
		}

		[TestMethod]
		public void AsReadOnlyCollection_Test3()
		{
			var x = Enumerable.Range(1, 5);
			var y = x.AsReadOnlyCollection();
			Assert.AreNotEqual(x, y);
			Assert.AreEqual(x.Count(), y.Count);
		}

		[TestMethod]
		public void AsReadOnlyList_Test1()
		{
			var x = (IEnumerable<int>)new List<int>();
			var y = x.AsReadOnlyList();
			Assert.AreEqual(x, y);
		}

		[TestMethod]
		public void AsReadOnlyList_Test2()
		{
			var x = (IEnumerable<int>)new Collection<int>();
			var y = x.AsReadOnlyList();
			Assert.AreEqual(x, y);
		}

		[TestMethod]
		public void AsReadOnlyList_Test3()
		{
			var x = Enumerable.Range(1, 5);
			var y = x.AsReadOnlyList();
			Assert.AreNotEqual(x, y);
			Assert.AreEqual(x.Count(), y.Count);
		}

		[TestMethod()]
		public void BatchAsLists_Test1()
		{
			var sourceList = Enumerable.Range(0, 1000).ToList();

			var batches = sourceList.BatchAsLists(250);
			var offset = 0;
			Assert.AreEqual(4, batches.Count());
			foreach (var batch in batches)
			{
				Assert.AreEqual(250, batch.Count);
				Assert.AreEqual(offset, batch.Min());
				Assert.AreEqual(offset + 249, batch.Max());
				offset += 250;
			}
		}

		[TestMethod()]
		public void BatchAsLists_Test2()
		{
			var sourceList = Enumerable.Range(0, 1002).ToList();

			var batches = sourceList.BatchAsLists(250);
			var offset = 0;
			Assert.AreEqual(5, batches.Count());
			foreach (var batch in batches.Take(4))
			{
				Assert.AreEqual(250, batch.Count);
				Assert.AreEqual(offset, batch.Min());
				Assert.AreEqual(offset + 249, batch.Max());
				offset += 250;
			}
			{
				var batch = batches.Skip(4).Single();
				Assert.AreEqual(2, batch.Count);
				Assert.AreEqual(offset, batch.Min());
				Assert.AreEqual(offset + 1, batch.Max());
				offset += 250;
			}
		}

		[TestMethod()]
		public void BatchAsLists_Test3()
		{
			var sourceList = Enumerable.Range(0, 0).ToList();

			var batches = sourceList.BatchAsLists(250);

			Assert.AreEqual(0, batches.Count());
		}

		[TestMethod()]
		public void BatchAsLists_Test4()
		{
			using (var verify = new Verify())
			{
				IList<int> sourceList = null;
				verify.ArgumentNullException("source", () => sourceList.BatchAsLists(250));
			}
		}

		[TestMethod()]
		public void BatchAsLists_Test5()
		{
			using (var verify = new Verify())
			{
				var sourceList = Enumerable.Range(0, 0).ToList();

				verify.ArgumentOutOfRangeException("batchSize", 0, () => sourceList.BatchAsLists(0));
			}
		}

		[TestMethod()]
		public void BatchAsSegments_Test1()
		{
			var sourceList = Enumerable.Range(0, 1000).ToList();

			var batches = sourceList.BatchAsSegments(250);
			var offset = 0;
			Assert.AreEqual(4, batches.Count());
			foreach (var batch in batches)
			{
				Assert.AreEqual(250, batch.Count);
				Assert.AreEqual(offset, batch.Min());
				Assert.AreEqual(offset + 249, batch.Max());
				offset += 250;
			}
		}

		[TestMethod()]
		public void BatchAsSegments_Test2()
		{
			var sourceList = Enumerable.Range(0, 1002).ToList();

			var batches = sourceList.BatchAsSegments(250);
			var offset = 0;
			Assert.AreEqual(5, batches.Count());
			foreach (var batch in batches.Take(4))
			{
				Assert.AreEqual(250, batch.Count);
				Assert.AreEqual(offset, batch.Min());
				Assert.AreEqual(offset + 249, batch.Max());
				offset += 250;
			}
			{
				var batch = batches.Skip(4).Single();
				Assert.AreEqual(2, batch.Count);
				Assert.AreEqual(offset, batch.Min());
				Assert.AreEqual(offset + 1, batch.Max());
				offset += 250;
			}
		}

		[TestMethod()]
		public void BatchAsSegments_Test3()
		{
			var sourceList = Enumerable.Range(0, 0).ToList();

			var batches = sourceList.BatchAsSegments(250);

			Assert.AreEqual(0, batches.Count());
		}

		[TestMethod()]
		public void BatchAsSegments_Test4()
		{
			using (var verify = new Verify())
			{
				IReadOnlyList<int> sourceList = null;
				verify.ArgumentNullException("source", () => sourceList.BatchAsSegments(250));
			}
		}

		[TestMethod()]
		public void BatchAsSegments_Test5()
		{
			using (var verify = new Verify())
			{
				var sourceList = Enumerable.Range(0, 0).ToList();

				verify.ArgumentOutOfRangeException("batchSize", 0, () => sourceList.BatchAsSegments(0));
			}
		}


		[TestMethod]
		public void IndexOf_Test1()
		{
			var list = (IReadOnlyList<int>)new List<int> { 1, 2, 3, 4, 5 };

			Assert.AreEqual(0, list.IndexOf(1));
		}

		[TestMethod]
		public void IndexOf_Test2()
		{
			var list = (IReadOnlyList<int>)new List<int> { 1, 2, 3, 4, 5 };

			Assert.AreEqual(-1, list.IndexOf(10));
		}

		[TestMethod]
		public void IndexOf_Test3()
		{
			var list = (IReadOnlyList<int?>)new List<int?> { 1, 2, 3, 4, 5 };

			Assert.AreEqual(0, list.IndexOf(1));
		}

		[TestMethod]
		public void IndexOf_Test4()
		{
			var list = (IReadOnlyList<int?>)new List<int?> { 1, 2, 3, 4, 5 };

			Assert.AreEqual(-1, list.IndexOf(10));
		}

		[TestMethod]
		public void IndexOf_Test5()
		{
			var list = (IReadOnlyList<int?>)new List<int?> { 1, 2, 3, 4, 5, null };

			Assert.AreEqual(5, list.IndexOf(null));
		}

		[TestMethod]
		public void IndexOf_Test6()
		{
			var list = (IReadOnlyList<int?>)new List<int?> { 1, 2, 3, 4, 5 };

			Assert.AreEqual(-1, list.IndexOf(null));
		}

		[TestMethod]
		public void IndexOf_Test7()
		{
			var list = (IReadOnlyList<string?>)new List<string?> { "1", "2", "3" };

			Assert.AreEqual(-1, list.IndexOf(null));
		}

		[TestMethod]
		public void IndexOf_Test8()
		{
			var list = (IReadOnlyList<string?>)new List<string?> { "1", "2", "3", null };

			Assert.AreEqual(3, list.IndexOf(null));
		}

		[TestMethod()]
		public void InsertRange_Test1()
		{
			using (var verify = new Verify())
			{
				List<string> target = new List<string>();
				List<string> list = null;
				verify.ArgumentNullException("list", () => CollectionUtilities.InsertRange(target, 0, list));
			}
		}

		[TestMethod()]
		public void InsertRange_Test2()
		{
			using (var verify = new Verify())
			{
				List<string> target = null;
				List<string> list = null;
				verify.ArgumentNullException("target", () => CollectionUtilities.InsertRange(target, 0, list));
			}
		}

		[TestMethod()]
		public void InsertRange_Test3()
		{
			using (var verify = new Verify())
			{
				List<string> target = new List<string>();
				List<string> list = new List<string>();
				target.Add("AAA");
				list.Add("BBB");
				list.Add("CCC");

				CollectionUtilities.InsertRange(target, 0, list);
				verify.ItemsAreEqual(new[] { "BBB", "CCC", "AAA" }, target, "InsertRange should have added 3 items");
			}
		}

		[TestMethod()]
		public void InsertRange_Test4()
		{
			using (var verify = new Verify())
			{
				List<string> target = new List<string>();
				List<string> list = new List<string>();
				target.Add("AAA");
				target.Add("BBB");
				target.Add("CCC");

				CollectionUtilities.InsertRange(target, 0, list);
				verify.ItemsAreEqual(new[] { "AAA", "BBB", "CCC" }, target, "InsertRange should have added 3 items");
			}
		}

		[TestMethod()]
		public void InsertRange_Test5()
		{
			using (var verify = new Verify())
			{
				List<string> target = new List<string>();
				List<string> list = new List<string>();
				list.Add("AAA");
				list.Add("BBB");
				list.Add("CCC");

				CollectionUtilities.InsertRange(target, 0, list);
				verify.ItemsAreEqual(new[] { "AAA", "BBB", "CCC" }, target, "InsertRange should have added 3 items");
			}
		}

		[TestMethod()]
		public void InsertRange_Test6()
		{
			using (var verify = new Verify())
			{
				List<string> target = new List<string>() { "A", "B", "C" };
				List<string> list = new List<string>();
				verify.ArgumentOutOfRangeException("startingIndex", -1, () => CollectionUtilities.InsertRange(target, -1, list));
			}
		}

		[TestMethod()]
		public void InsertRange_Test7()
		{
			using (var verify = new Verify())
			{
				List<string> target = new List<string>() { "A", "B", "C" };
				List<string> list = new List<string>();
				verify.ArgumentOutOfRangeException("startingIndex", 4, () => CollectionUtilities.InsertRange(target, 4, list));
			}
		}

		[TestMethod()]
		public void InsertRange_Test8()
		{
			using (var verify = new Verify())
			{
				var list = new List<int> { 1, 2, 3 };
				IList<int> target = new ReadOnlyCollection<int>(new List<int> { 4, 5, 6 });
				verify.ArgumentException("target", () => CollectionUtilities.InsertRange(target, 1, list), "read-only list");
			}
		}

		[TestMethod()]
		public void RemoveRange_Test1()
		{
			using (var verify = new Verify())
			{
				List<string> list = null;
				verify.ArgumentNullException("list", () => CollectionUtilities.RemoveRange(list, 0, 1));
			}
		}

		[TestMethod()]
		public void RemoveRange_Test2()
		{
			using (var verify = new Verify())
			{
				List<string> list = new List<string>();
				list.Add("AAA");
				list.Add("BBB");
				list.Add("CCC");
				CollectionUtilities.RemoveRange(list, 0, 1);

				verify.ItemsAreEqual(new[] { "BBB", "CCC" }, list, "RemoveRange should have left 2 items");
			}
		}

		[TestMethod()]
		public void RemoveRange_Test3()
		{
			using (var verify = new Verify())
			{
				List<string> list = new List<string>();
				list.Add("AAA");
				list.Add("BBB");
				list.Add("CCC");
				CollectionUtilities.RemoveRange(list, 0, 2);

				verify.ItemsAreEqual(new[] { "CCC" }, list, "RemoveRange should have left 1 item");
			}
		}

		[TestMethod()]
		public void RemoveRange_Test4()
		{
			using (var verify = new Verify())
			{
				List<string> list = new List<string>();
				list.Add("AAA");
				list.Add("BBB");
				list.Add("CCC");
				CollectionUtilities.RemoveRange(list, 1, 2);

				verify.ItemsAreEqual(new[] { "AAA" }, list, "RemoveRange should have left 1 item");
			}
		}

		[TestMethod()]
		public void RemoveRange_Test5()
		{
			using (var verify = new Verify())
			{
				List<string> target = new List<string>() { "A", "B", "C" };
				verify.ArgumentOutOfRangeException("startingIndex", 3, () => CollectionUtilities.RemoveRange(target, 3, 1));
			}
		}

		[TestMethod()]
		public void RemoveRange_Test6()
		{
			using (var verify = new Verify())
			{
				List<string> target = new List<string>() { "A", "B", "C" };
				verify.ArgumentOutOfRangeException("startingIndex", -1, () => CollectionUtilities.RemoveRange(target, -1, 1));
			}
		}

		[TestMethod()]
		public void RemoveRange_Test7()
		{
			using (var verify = new Verify())
			{
				IList<int> target = new ReadOnlyCollection<int>(new List<int> { 4, 5, 6 });
				verify.ArgumentException("list", () => CollectionUtilities.RemoveRange(target, 1, 1), "read-only list");
			}
		}
	}
}
