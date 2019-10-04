using System;
using System.Collections.Generic;
using System.Linq;
using Tortuga.Anchor;
using Tortuga.Dragnet;

 
using Microsoft.VisualStudio.TestTools.UnitTesting;
 

namespace Tests
{
    [TestClass]
    public class RandomExtendedTests
    {

        const int LoopLimit = 1000;

        [TestMethod]
        public void RandomExtended_DateTime()
        {
            using (var verify = new Verify())
            {
                var rand = new RandomExtended(0);
                var minValue = new DateTime(2001, 1, 1);
                var maxValue = new DateTime(2010, 12, 31);

                for (int i = 0; i < LoopLimit; i++)
                {
                    var result = rand.NextDateTime(minValue, maxValue);
                    verify.IsBetween(minValue, result, maxValue, "random result not within expected range");
                }
            }
        }

        [TestMethod]
        public void RandomExtended_DateTimeOffset()
        {
            using (var verify = new Verify())
            {
                var rand = new RandomExtended(0);
                var minValue = new DateTimeOffset(new DateTime(2001, 1, 1), TimeSpan.FromHours(-8));
                var maxValue = new DateTimeOffset(new DateTime(2010, 12, 31), TimeSpan.FromHours(-8));

                for (int i = 0; i < LoopLimit; i++)
                {
                    var result = rand.NextDateTimeOffset(minValue, maxValue);
                    verify.IsBetween(minValue, result, maxValue, "random result not within expected range");
                }
            }
        }

        [TestMethod]
        public void RandomExtended_Decimal()
        {
            using (var verify = new Verify())
            {
                var rand = new RandomExtended(0);
                var minValue = 1.0M;
                var maxValue = 100.0M;

                for (int i = 0; i < LoopLimit; i++)
                {
                    var result = rand.NextDecimal(minValue, maxValue);
                    verify.IsBetween(minValue, result, maxValue, "random result not within expected range");
                }
            }
        }

        [TestMethod]
        public void RandomExtended_Long()
        {
            using (var verify = new Verify())
            {
                var rand = new RandomExtended(0);
                var minValue = 1L;
                var maxValue = 100L;

                for (int i = 0; i < LoopLimit; i++)
                {
                    var result = rand.NextInt64(minValue, maxValue);
                    verify.IsBetween(minValue, result, maxValue, "random result not within expected range");
                }
            }
        }

        [TestMethod]
        public void RandomExtended_Pick()
        {
            using (var verify = new Verify())
            {
                var rand = new RandomExtended(0);
                var source = new List<int>();
                for (int i = 100; i < 200; i++)
                    source.Add(i);
                for (int i = 0; i < 100; i++)
                {
                    var item = rand.Pick(source);
                    verify.IsBetween(100, item, 199, "Item couldn't be in the source list");
                    verify.IsFalse(source.Contains(item), "Item should have been removed from list");
                }
                Assert.AreEqual(0, source.Count, "List should have been left empty");
            }
        }

        [TestMethod]
        public void RandomExtended_Pick2()
        {
            using (var verify = new Verify())
            {
                var rand = new RandomExtended(0);
                var source = new List<int>();
                for (int i = 100; i < 200; i++)
                    source.Add(i);
                for (int i = 0; i < 100; i += 10)
                {
                    var items = rand.Pick(source, 10);
                    foreach (var item in items)
                    {
                        verify.IsBetween(100, item, 199, "Item couldn't be in the source list");
                        verify.IsFalse(source.Contains(item), "Item should have been removed from list");
                    }
                }
                Assert.AreEqual(0, source.Count, "List should have been left empty");
            }
        }

        [TestMethod]
        public void RandomExtended_Choose()
        {
            using (var verify = new Verify())
            {
                var rand = new RandomExtended(0);
                var source = new List<int>();
                for (int i = 100; i < 200; i++)
                    source.Add(i);
                for (int i = 0; i < 100; i++)
                {
                    var item = rand.Choose(source);
                    verify.IsBetween(100, item, 199, "Item couldn't be in the sourse list");
                    verify.IsTrue(source.Contains(item), "Item should have been left in the list");
                }
            }
        }

        [TestMethod]
        public void RandomExtended_Choose2()
        {
            using (var verify = new Verify())
            {
                var rand = new RandomExtended(0);
                var source = new List<int>();
                for (int i = 100; i < 200; i++)
                    source.Add(i);
                for (int i = 0; i < 100; i += 10)
                {
                    var items = rand.Choose(source, 10, true);
                    foreach (var item in items)
                    {
                        verify.IsBetween(100, item, 199, "Item couldn't be in the source list");
                        verify.IsTrue(source.Contains(item), "Item should have been left in the list");
                    }
                }
            }
        }

        [TestMethod]
        public void RandomExtended_Choose3()
        {
            using (var verify = new Verify())
            {
                var rand = new RandomExtended(0);
                var source = new List<int>();
                for (int i = 100; i < 200; i++)
                    source.Add(i);
                for (int i = 0; i < 100; i += 10)
                {
                    var items = rand.Choose(source, 10, false);
                    foreach (var item in items)
                    {
                        verify.IsBetween(100, item, 199, "Item couldn't be in the source list");
                        verify.IsTrue(source.Contains(item), "Item should have been left in the list");
                    }
                    var distinct = items.Distinct().Count();
                    verify.AreEqual(items.Count, distinct, "There shouldn't have been any duplicates");
                }
            }
        }

    }
}



