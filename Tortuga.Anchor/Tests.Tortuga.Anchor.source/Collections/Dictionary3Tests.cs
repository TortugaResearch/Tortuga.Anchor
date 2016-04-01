using Tortuga.Anchor.Collections;
using Tortuga.Dragnet;
using System.Collections.Generic;

#if MSTest
using Microsoft.VisualStudio.TestTools.UnitTesting;
#elif WINDOWS_UWP 
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#endif

namespace Tests.Collections
{
    [TestClass]
    public class Dictionary3Tests
    {
        [TestMethod]
        public void Dictionary3_ConstructorTest1()
        {
            using (var verify = new Verify())
            {
                var dictionary = new Dictionary<int, int, int>();
                dictionary[1, 1] = 10;
                dictionary.Add(1, 2, 3);

                verify.AreEqual(10, dictionary[1, 1], "Key 1,1 should have the value 10");
                verify.AreEqual(3, dictionary[1, 2], "Key 1,2 should have the value 3");
            }
        }


        [TestMethod]
        public void Dictionary3_ConstructorTest2()
        {
            using (var verify = new Verify())
            {
                var source = new Dictionary<Pair<int, int>, int>();
                source.Add(new Pair<int, int>(1, 1), 10);
                source.Add(new Pair<int, int>(1, 2), 3);
                var dictionary = new Dictionary<int, int, int>(source);

                verify.AreEqual(10, dictionary[1, 1], "Key 1,1 should have the value 10");
                verify.AreEqual(3, dictionary[1, 2], "Key 1,2 should have the value 3");
            }
        }

        [TestMethod]
        public void Dictionary3_ConstructorTest3()
        {
            using (var verify = new Verify())
            {
                IDictionary<Pair<int, int>, int> source = new Dictionary<Pair<int, int>, int>();
                source.Add(new Pair<int, int>(1, 1), 10);
                source.Add(new Pair<int, int>(1, 2), 3);
                var dictionary = new Dictionary<int, int, int>(source);

                verify.AreEqual(10, dictionary[1, 1], "Key 1,1 should have the value 10");
                verify.AreEqual(3, dictionary[1, 2], "Key 1,2 should have the value 3");
            }
        }

        [TestMethod]
        public void Dictionary3_ConstructorTest4()
        {
            using (var verify = new Verify())
            {
                var dictionary = new Dictionary<int, int, int>(100);
                dictionary[1, 1] = 10;
                dictionary.Add(1, 2, 3);

                verify.AreEqual(10, dictionary[1, 1], "Key 1,1 should have the value 10");
                verify.AreEqual(3, dictionary[1, 2], "Key 1,2 should have the value 3");

            }
        }

        [TestMethod]
        public void Dictionary3_ConstructorTest5()
        {
            using (var verify = new Verify())
            {
                var dictionary = new Dictionary<int, int, int>(new ReversableIEqualityComparer());
                dictionary[1, 1] = 10;
                dictionary.Add(1, 2, 3);

                verify.AreEqual(10, dictionary[1, 1], "Key 1,1 should have the value 10");
                verify.AreEqual(3, dictionary[1, 2], "Key 1,2 should have the value 3");
                verify.AreEqual(3, dictionary[2, 1], "Key 2,1 should be the same as key 1,2");
            }
        }

        [TestMethod]
        public void Dictionary3_ConstructorTest6()
        {
            using (var verify = new Verify())
            {
                var source = new Dictionary<Pair<int, int>, int>();
                source.Add(new Pair<int, int>(1, 1), 10);
                source.Add(new Pair<int, int>(1, 2), 3);
                var dictionary = new Dictionary<int, int, int>(source, new ReversableIEqualityComparer());


                verify.AreEqual(10, dictionary[1, 1], "Key 1,1 should have the value 10");
                verify.AreEqual(3, dictionary[1, 2], "Key 1,2 should have the value 3");
                verify.AreEqual(3, dictionary[2, 1], "Key 2,1 should be the same as key 1,2");
            }
        }

        [TestMethod]
        public void Dictionary3_Misc()
        {
            using (var verify = new Verify())
            {
                var dictionary = new Dictionary<int, int, int>();
                dictionary[1, 1] = 10;
                dictionary.Add(1, 2, 3);

                verify.AreEqual(10, dictionary[1, 1], "Key 1,1 should have the value 10");
                verify.AreEqual(3, dictionary[1, 2], "Key 1,2 should have the value 3");

                verify.IsTrue(dictionary.ContainsKey(Pair.Create(1, 1)), "Key 1,1 should exist");
                verify.IsTrue(dictionary.ContainsKey(1, 2), "Key 1,2 should exist");

                dictionary.Clear();
                verify.AreEqual(0, dictionary.Count, "Dictioanry should have been cleared");

                verify.IsFalse(dictionary.ContainsKey(Pair.Create(1, 1)), "Key 1,1 should not exist");
                verify.IsFalse(dictionary.ContainsKey(1, 2), "Key 1,2 should not exist");


                dictionary.Add(Pair.Create(3, 4), 12);
                dictionary[Pair.Create(3, 5)] = 15;

                verify.AreEqual(12, dictionary[Pair.Create(3, 4)], "Key 3,4 should have the value 12");
                verify.AreEqual(15, dictionary[Pair.Create(3, 5)], "Key 3,5 should have the value 15");


                verify.IsTrue(dictionary.Remove(Pair.Create(3, 4)), "Key 3,4 should exist");
                verify.IsTrue(dictionary.Remove(3, 5), "Key 3,5, should exist");

                verify.IsFalse(dictionary.Remove(Pair.Create(3, 4)), "Key 3,4 should not exist");
                verify.IsFalse(dictionary.Remove(3, 5), "Key 3,5, should not exist");

            }
        }


        public class ReversableIEqualityComparer : IEqualityComparer<Pair<int, int>>
        {
            public bool Equals(Pair<int, int> x, Pair<int, int> y)
            {
                return (x.Item1 == y.Item1 && x.Item2 == y.Item2) || (x.Item1 == y.Item2 & x.Item2 == y.Item1);
            }

            public int GetHashCode(Pair<int, int> obj)
            {
                return obj.Item1.GetHashCode() ^ obj.Item2.GetHashCode();
            }
        }

    }
}
