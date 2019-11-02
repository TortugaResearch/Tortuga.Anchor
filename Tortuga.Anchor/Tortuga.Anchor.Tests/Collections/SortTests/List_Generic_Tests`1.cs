// Licensed to the .NET Foundation under one or more agreements. The .NET Foundation licenses this
// file to you under the MIT license. See the LICENSE file in the project root for more information.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using Tortuga.Anchor;

namespace Tortuga.Anchor.Tests.Collections.SortTests
{
    public abstract partial class List_Generic_Tests<T> where T : IComparable<T>
    {
        public static IEnumerable<object[]> ValidCollectionSizes_GreaterThanOne()
        {
            yield return new object[] { 2 };
            yield return new object[] { 20 };
        }

        #region Sort

        [DataTestMethod, ValidCollectionSizes_GreaterThanOne]
        public void Sort_WithDuplicates(int count)
        {
            IList<T> list = GenericListFactory(count);
            list.Add(list[0]);
            IComparer<T> comparer = Comparer<T>.Default;
            list.Sort();
            foreach (var i in Enumerable.Range(0, count - 2))
            {
                Assert.IsTrue(comparer.Compare(list[i], list[i + 1]) <= 0);
            };
        }

        [DataTestMethod, ValidCollectionSizes_GreaterThanOne]
        public void Sort_WithoutDuplicates(int count)
        {
            IList<T> list = GenericListFactory(count);
            IComparer<T> comparer = Comparer<T>.Default;
            list.Sort();
            foreach (var i in Enumerable.Range(0, count - 2))
            {
                Assert.IsTrue(comparer.Compare(list[i], list[i + 1]) < 0);
            };
        }

        #endregion Sort

        #region Sort(IComparer)

        [DataTestMethod, ValidCollectionSizes_GreaterThanOne]
        public void Sort_IComparer_WithDuplicates(int count)
        {
            IList<T> list = GenericListFactory(count);
            list.Add(list[0]);
            IComparer<T> comparer = GetIComparer();
            list.Sort(comparer);
            foreach (var i in Enumerable.Range(0, count - 2))
            {
                Assert.IsTrue(comparer.Compare(list[i], list[i + 1]) <= 0);
            };
        }

        [DataTestMethod, ValidCollectionSizes_GreaterThanOne]
        public void Sort_IComparer_WithoutDuplicates(int count)
        {
            IList<T> list = GenericListFactory(count);
            IComparer<T> comparer = GetIComparer();
            list.Sort(comparer);
            foreach (var i in Enumerable.Range(0, count - 2))
            {
                Assert.IsTrue(comparer.Compare(list[i], list[i + 1]) < 0);
            };
        }

        protected virtual IComparer<T> GetIComparer() => Comparer<T>.Default;

        #endregion Sort(IComparer)

        #region Sort(Comparison)

        [DataTestMethod, ValidCollectionSizes_GreaterThanOne]
        public void Sort_Comparison_WithDuplicates(int count)
        {
            IList<T> list = GenericListFactory(count);
            list.Add(list[0]);
            IComparer<T> iComparer = GetIComparer();
            Comparison<T> comparison = ((T first, T second) => { return iComparer.Compare(first, second); });
            list.Sort(comparison);
            foreach (var i in Enumerable.Range(0, count - 2))
            {
                Assert.IsTrue(iComparer.Compare(list[i], list[i + 1]) <= 0);
            };
        }

        [DataTestMethod, ValidCollectionSizes_GreaterThanOne]
        public void Sort_Comparison_WithoutDuplicates(int count)
        {
            IList<T> list = GenericListFactory(count);
            IComparer<T> iComparer = GetIComparer();
            Comparison<T> comparison = ((T first, T second) => { return iComparer.Compare(first, second); });
            list.Sort(comparison);
            foreach (var i in Enumerable.Range(0, count - 2))
            {
                Assert.IsTrue(iComparer.Compare(list[i], list[i + 1]) < 0);
            };
        }

        #endregion Sort(Comparison)

        #region Sort(int, int, IComparer<T>)

        [DataTestMethod, ValidCollectionSizes]
        public void Sort_intintIComparer_InvalidRange_ThrowsArgumentException(int count)
        {
            IList<T> list = GenericListFactory(count);
            Tuple<int, int>[] InvalidParameters = new Tuple<int, int>[]
            {
                Tuple.Create(count, 1),
                Tuple.Create(count + 1, 0),
                Tuple.Create(int.MaxValue, 0),
            };

            foreach (var invalidSet in InvalidParameters)
            {
                Assert.ThrowsException<ArgumentOutOfRangeException>(() => list.Sort(invalidSet.Item1, invalidSet.Item2, GetIComparer()));
            };
        }

        [DataTestMethod, ValidCollectionSizes]
        public void Sort_intintIComparer_NegativeRange_ThrowsArgumentOutOfRangeException(int count)
        {
            IList<T> list = GenericListFactory(count);
            Tuple<int, int>[] InvalidParameters = new Tuple<int, int>[]
            {
                Tuple.Create(-1,-1),
                Tuple.Create(-1, 0),
                Tuple.Create(-1, 1),
                Tuple.Create(-1, 2),
                Tuple.Create(-2, 0),
                Tuple.Create(int.MinValue, 0),
                Tuple.Create(0 ,-1),
                Tuple.Create(0 ,-2),
                Tuple.Create(0 , int.MinValue),
                Tuple.Create(1 ,-1),
                Tuple.Create(2 ,-1),
            };

            foreach (var invalidSet in InvalidParameters)
            {
                Assert.ThrowsException<ArgumentOutOfRangeException>(() => list.Sort(invalidSet.Item1, invalidSet.Item2, GetIComparer()));
            };
        }

        [DataTestMethod, ValidCollectionSizes_GreaterThanOne]
        public void Sort_intintIComparer_WithDuplicates(int count)
        {
            IList<T> unsortedList = GenericListFactory(count);
            IComparer<T> comparer = GetIComparer();
            unsortedList.Add(unsortedList[0]);
            for (int startIndex = 0; startIndex < count - 2; startIndex++)
                for (int sortCount = 2; sortCount < count - startIndex; sortCount++)
                {
                    IList<T> list = new Collection<T>(unsortedList);
                    list.Sort(startIndex, sortCount + 1, comparer);
                    for (int i = startIndex; i < sortCount; i++)
                        InRange(comparer.Compare(list[i], list[i + 1]), int.MinValue, 1);
                }
        }

        [DataTestMethod, ValidCollectionSizes_GreaterThanOne]
        public void Sort_intintIComparer_WithoutDuplicates(int count)
        {
            IList<T> unsortedList = GenericListFactory(count);
            IComparer<T> comparer = GetIComparer();
            for (int startIndex = 0; startIndex < count - 2; startIndex++)
                for (int sortCount = 1; sortCount < count - startIndex; sortCount++)
                {
                    IList<T> list = new Collection<T>(unsortedList);
                    list.Sort(startIndex, sortCount + 1, comparer);
                    for (int i = startIndex; i < sortCount; i++)
                        InRange(comparer.Compare(list[i], list[i + 1]), int.MinValue, 0);
                }
        }

        protected IList<T> CreateEnumerable(IEnumerable<T> enumerableToMatchTo, int count, int numberOfMatchingElements, int numberOfDuplicateElements)
        {
            return new List<T>(CreateList(enumerableToMatchTo, count, numberOfMatchingElements, numberOfDuplicateElements));
        }

        protected IEnumerable<T> CreateList(IEnumerable<T> enumerableToMatchTo, int count, int numberOfMatchingElements, int numberOfDuplicateElements)
        {
            IList<T> list = new Collection<T>();
            int seed = 528;
            int duplicateAdded = 0;
            IList<T> match = null;

            // Add Matching elements
            if (enumerableToMatchTo != null)
            {
                match = enumerableToMatchTo.ToList();
                for (int i = 0; i < numberOfMatchingElements; i++)
                {
                    list.Add(match[i]);
                    while (duplicateAdded++ < numberOfDuplicateElements)
                        list.Add(match[i]);
                }
            }

            // Add elements to reach the desired count
            while (list.Count < count)
            {
                T toAdd = CreateT(seed++);
                while (list.Contains(toAdd) || (match != null && match.Contains(toAdd))) // Don't want any unexpectedly duplicate values
                    toAdd = CreateT(seed++);
                list.Add(toAdd);
                while (duplicateAdded++ < numberOfDuplicateElements)
                    list.Add(toAdd);
            }

            // Validate that the Enumerable fits the guidelines as expected
            if (match != null)
            {
                int actualMatchingCount = 0;
                foreach (T lookingFor in match)
                    actualMatchingCount += list.Contains(lookingFor) ? 1 : 0;
                Assert.AreEqual(numberOfMatchingElements, actualMatchingCount);
            }

            return list;
        }

        protected abstract T CreateT(int seed);

        protected virtual IList<T> GenericListFactory()
        {
            return new Collection<T>();
        }

        protected virtual IList<T> GenericListFactory(int count)
        {
            return new Collection<T>(CreateEnumerable(null, count, 0, 0));
        }

        void InRange(int compare, int minValue, int maxValue)
        {
            Assert.IsTrue(minValue <= compare);
            Assert.IsTrue(compare <= maxValue);
        }

        #endregion Sort(int, int, IComparer<T>)
    }
}
