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
        public void CollectionUtilities_AddRange_Test1()
        {
            using (var verify = new Verify())
            {
                List<string> target = new List<string>();
                List<string> list = null;
                verify.ArgumentNullException("list", () => CollectionUtilities.AddRange(target, list));
            }
        }

        [TestMethod()]
        public void CollectionUtilities_AddRange_Test10()
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
        public void CollectionUtilities_AddRange_Test11()
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
        public void CollectionUtilities_AddRange_Test2()
        {
            using (var verify = new Verify())
            {
                List<string> target = null;
                List<string> list = null;
                verify.ArgumentNullException("target", () => CollectionUtilities.AddRange(target, list));
            }
        }

        [TestMethod()]
        public void CollectionUtilities_AddRange_Test3()
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
        public void CollectionUtilities_AddRange_Test3b()
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
        public void CollectionUtilities_AddRange_Test4()
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
        public void CollectionUtilities_AddRange_Test5()
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
        public void CollectionUtilities_AddRange_Test6()
        {
            using (var verify = new Verify())
            {
                List<string> target = new List<string>();

                CollectionUtilities.AddRange(target, "AAA", "BBB", "CCC");
                verify.ItemsAreEqual(new[] { "AAA", "BBB", "CCC" }, target, "AddRange should have added 3 items");
            }
        }

        [TestMethod()]
        public void CollectionUtilities_AddRange_Test7()
        {
            using (var verify = new Verify())
            {
                List<string> target = new List<string>();
                string[] list = null;
                verify.ArgumentNullException("list", () => CollectionUtilities.AddRange(target, list));
            }
        }

        [TestMethod()]
        public void CollectionUtilities_AddRange_Test8()
        {
            using (var verify = new Verify())
            {
                List<string> target = null;
                string[] list = null;
                verify.ArgumentNullException("target", () => CollectionUtilities.AddRange(target, list));
            }
        }

        [TestMethod()]
        public void CollectionUtilities_AddRange_Test9()
        {
            using (var verify = new Verify())
            {
                var list = new List<int> { 1, 2, 3 };
                ICollection<int> target = new ReadOnlyCollection<int>(new List<int>());
                verify.ArgumentException("target", () => CollectionUtilities.AddRange(target, list), "read-only list");
                verify.ArgumentException("target", () => CollectionUtilities.AddRange(target, 1, 2, 3), "read-only list");
            }
        }

        [TestMethod()]
        public void CollectionUtilities_InsertRange_Test1()
        {
            using (var verify = new Verify())
            {
                List<string> target = new List<string>();
                List<string> list = null;
                verify.ArgumentNullException("list", () => CollectionUtilities.InsertRange(target, 0, list));
            }
        }

        [TestMethod()]
        public void CollectionUtilities_InsertRange_Test2()
        {
            using (var verify = new Verify())
            {
                List<string> target = null;
                List<string> list = null;
                verify.ArgumentNullException("target", () => CollectionUtilities.InsertRange(target, 0, list));
            }
        }

        [TestMethod()]
        public void CollectionUtilities_InsertRange_Test3()
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
        public void CollectionUtilities_InsertRange_Test4()
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
        public void CollectionUtilities_InsertRange_Test5()
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
        public void CollectionUtilities_InsertRange_Test6()
        {
            using (var verify = new Verify())
            {
                List<string> target = new List<string>() { "A", "B", "C" };
                List<string> list = new List<string>();
                verify.ArgumentOutOfRangeException("startingIndex", -1, () => CollectionUtilities.InsertRange(target, -1, list));
            }
        }

        [TestMethod()]
        public void CollectionUtilities_InsertRange_Test7()
        {
            using (var verify = new Verify())
            {
                List<string> target = new List<string>() { "A", "B", "C" };
                List<string> list = new List<string>();
                verify.ArgumentOutOfRangeException("startingIndex", 4, () => CollectionUtilities.InsertRange(target, 4, list));
            }
        }

        [TestMethod()]
        public void CollectionUtilities_InsertRange_Test8()
        {
            using (var verify = new Verify())
            {
                var list = new List<int> { 1, 2, 3 };
                IList<int> target = new ReadOnlyCollection<int>(new List<int> { 4, 5, 6 });
                verify.ArgumentException("target", () => CollectionUtilities.InsertRange(target, 1, list), "read-only list");
            }
        }

        [TestMethod()]
        public void CollectionUtilities_RemoveRange_Test1()
        {
            using (var verify = new Verify())
            {
                List<string> list = null;
                verify.ArgumentNullException("list", () => CollectionUtilities.RemoveRange(list, 0, 1));
            }
        }

        [TestMethod()]
        public void CollectionUtilities_RemoveRange_Test2()
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
        public void CollectionUtilities_RemoveRange_Test3()
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
        public void CollectionUtilities_RemoveRange_Test4()
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
        public void CollectionUtilities_RemoveRange_Test5()
        {
            using (var verify = new Verify())
            {
                List<string> target = new List<string>() { "A", "B", "C" };
                verify.ArgumentOutOfRangeException("startingIndex", 3, () => CollectionUtilities.RemoveRange(target, 3, 1));
            }
        }

        [TestMethod()]
        public void CollectionUtilities_RemoveRange_Test6()
        {
            using (var verify = new Verify())
            {
                List<string> target = new List<string>() { "A", "B", "C" };
                verify.ArgumentOutOfRangeException("startingIndex", -1, () => CollectionUtilities.RemoveRange(target, -1, 1));
            }
        }

        [TestMethod()]
        public void CollectionUtilities_RemoveRange_Test7()
        {
            using (var verify = new Verify())
            {
                IList<int> target = new ReadOnlyCollection<int>(new List<int> { 4, 5, 6 });
                verify.ArgumentException("list", () => CollectionUtilities.RemoveRange(target, 1, 1), "read-only list");
            }
        }
    }
}