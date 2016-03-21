using System;
using System.Collections.Generic;
using Tests.Mocks;
using Tortuga.Anchor.Collections;
using Tortuga.Dragnet;

#if MSTest
using Microsoft.VisualStudio.TestTools.UnitTesting;
#elif WINDOWS_UWP 
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#endif


namespace Tests.Collections
{
    [TestClass]
    public class WeakReferenceCollectionTests
    {
        [TestMethod]
        public void WeakReferenceCollection_Add()
        {
            using (var verify = new Verify())
            {
                var result = new WeakReferenceCollection<string>();
                verify.ArgumentNullException("item", () => result.Add(null));
            }
        }

        [TestMethod]
        public void WeakReferenceCollection_CopyTo_Test1()
        {
            using (var verify = new Verify())
            {
                var result = new WeakReferenceCollection<string>();
                verify.ArgumentNullException("array", () => result.CopyTo(null, 0));
            }
        }

        [TestMethod]
        public void WeakReferenceCollection_CopyTo_Test2()
        {
            using (var verify = new Verify())
            {
                var result = new WeakReferenceCollection<string>();
                result.Add("");
                var array = new string[0];
                verify.ArgumentException("array", () => result.CopyTo(array, 0), "array is too small");
            }
        }

        [TestMethod]
        public void WeakReferenceCollection_CopyTo_Test3()
        {
            using (var verify = new Verify())
            {
                var result = new WeakReferenceCollection<string>();
                result.Add("AAA");
                var array = new string[1];
                result.CopyTo(array, 0);
                verify.ItemsAreEqual(new[] { "AAA" }, array, "");
            }
        }

        [TestMethod]
        public void WeakReferenceCollection_CopyTo_Test4()
        {
            using (var verify = new Verify())
            {
                var result = new WeakReferenceCollection<string>();
                result.Add("");
                var array = new string[0];
                verify.ArgumentOutOfRangeException("arrayIndex", 1, () => result.CopyTo(array, 1));
            }
        }

        [TestMethod]
        public void WeakReferenceCollection_CopyTo_Test5()
        {
            using (var verify = new Verify())
            {
                var result = new WeakReferenceCollection<string>();
                result.Add("");
                var array = new string[0];
                verify.ArgumentOutOfRangeException("arrayIndex", -1, () => result.CopyTo(array, -1));
            }
        }

        [TestMethod]
        public void WeakReferenceCollection_AddRange_Test1()
        {
            using (var verify = new Verify())
            {
                IList<string> list = null;
                var result = new WeakReferenceCollection<string>();
                verify.ArgumentNullException("list", () => result.AddRange(list));
            }
        }

        [TestMethod]
        public void WeakReferenceCollection_AddRange_Test2()
        {
            using (var verify = new Verify())
            {
                string item1 = "AAA";
                string item2 = "BBB";

                var list = new List<string>();
                list.Add(item1);
                list.Add(item2);

                var result = new WeakReferenceCollection<string>();
                result.AddRange(list);

                verify.ItemsAreSame(new[] { "AAA", "BBB" }, result, "Two items should have been in the collection.");
            }
        }

        [TestMethod]
        public void WeakReferenceCollection_AddRange_Test3()
        {
            using (var verify = new Verify())
            {
                string item1 = "AAA";
                string item2 = "BBB";

                var list = new List<string>();
                list.Add(item1);
                list.Add(null);
                list.Add(item2);
                var result = new WeakReferenceCollection<string>();
                verify.ArgumentException("list", () => result.AddRange(list), "The list contains a null item");
            }
        }

        [TestMethod]
        public void WeakReferenceCollection_Clear_Test1()
        {
            using (var verify = new Verify())
            {
                string item1 = "AAA";
                string item2 = "BBB";

                var list = new List<string>();
                list.Add(item1);
                list.Add(item2);

                var result = new WeakReferenceCollection<string>();
                result.AddRange(list);

                verify.ItemsAreEqual(new[] { "AAA", "BBB" }, result, "Two items should have been in the collection.");

                result.Clear();

                verify.IsEmpty(result, "");
            }
        }

        [TestMethod]
        public void WeakReferenceCollection_Remove_Test1()
        {
            using (var verify = new Verify())
            {
                string item1 = "AAA";
                string item2 = "BBB";

                var list = new List<string>();
                list.Add(item1);
                list.Add(item2);

                var result = new WeakReferenceCollection<string>();
                result.AddRange(list);
                verify.ItemsAreEqual(new[] { "AAA", "BBB" }, result, "Two items should have been in the collection.");

                verify.IsTrue(result.Remove(item2), "item 1 should have been removable");

                verify.ItemsAreEqual(new[] { "AAA" }, result, "One item should have been left in the collection.");

                verify.IsTrue(result.Remove(item1), "item 2 should have been removable");

                verify.IsEmpty(result, "Removal failed");

                verify.IsFalse(result.Remove(item1), "item was already removed");

                verify.ArgumentNullException("item", () => result.Remove(null));

            }
        }

        [TestMethod]
        public void WeakReferenceCollection_Remove_Test2()
        {
            using (var verify = new Verify())
            {
                var result = new WeakReferenceCollection<Foo>();

                Action add = () => result.Add(new Foo());

                add();

                Memory.CycleGC();

                var f = new Foo();
                result.Add(f);
                result.Add(f);
                result.Remove(f);
                result.Remove(f);
                result.CleanUp();

                verify.IsEmpty(result, "");
            }
        }

        [TestMethod]
        public void WeakReferenceCollection_GetEnumerator()
        {
            using (var verify = new Verify())
            {
                string item1 = "AAA";
                string item2 = "BBB";

                var list = new List<string>();
                list.Add(item1);
                list.Add(item2);

                var result = new WeakReferenceCollection<string>();
                result.AddRange(list);

                verify.ItemsAreEqual((IEnumerable<string>)new[] { "AAA", "BBB" }, list, "");


                verify.IsFalse(((ICollection<string>)result).IsReadOnly, "Is read only should be false");
            }
        }
    }
}



