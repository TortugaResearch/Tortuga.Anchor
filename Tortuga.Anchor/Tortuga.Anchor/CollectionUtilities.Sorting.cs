// Sort functionality licensed to the .NET Foundation under one or more agreements. The .NET
// Foundation licenses this file to you under the MIT license. See the License.txt file in the
// project root for more information.

namespace Tortuga.Anchor;

static partial class CollectionUtilities
{
	/// <summary>
	/// This is the threshold where Introspective sort switches to Insertion sort. Empirically,
	/// 16 seems to speed up most cases without slowing down others, at least for integers. Large
	/// value types may benefit from a smaller number.
	/// </summary>
	const int s_IntrospectiveSortSizeThreshold = 16;

	static void DownHeap<T>(IList<T> keys, int i, int n, int lo, Comparison<T> comparer)
	{
		T d = keys[lo + i - 1];
		int child;
		while (i <= n / 2)
		{
			child = 2 * i;
			if (child < n && comparer(keys[lo + child - 1], keys[lo + child]) < 0)
			{
				child++;
			}
			if (!(comparer(d, keys[lo + child - 1]) < 0))
				break;
			keys[lo + i - 1] = keys[lo + child - 1];
			i = child;
		}
		keys[lo + i - 1] = d;
	}

	static void Heapsort<T>(IList<T> keys, int lo, int hi, Comparison<T> comparer)
	{
		int n = hi - lo + 1;
		for (int i = n / 2; i >= 1; i--)
		{
			DownHeap(keys, i, n, lo, comparer);
		}
		for (int i = n; i > 1; i--)
		{
			Swap(keys, lo, lo + i - 1);
			DownHeap(keys, 1, i - 1, lo, comparer);
		}
	}

	static void InsertionSort<T>(IList<T> keys, int lo, int hi, Comparison<T> comparer)
	{
		int i, j;
		T t;
		for (i = lo; i < hi; i++)
		{
			j = i;
			t = keys[i + 1];
			while (j >= lo && comparer(t, keys[j]) < 0)
			{
				keys[j + 1] = keys[j];
				j--;
			}
			keys[j + 1] = t;
		}
	}

	static void IntrospectiveSort<T>(IList<T> keys, int left, int length, Comparison<T> comparison)
	{
		if (length <= 1)
			return;

		IntrospectiveSort(keys, left, length + left - 1, 2 * FloorLog2PlusOne(length), comparison);

		static int FloorLog2PlusOne(int n)
		{
			int result = 0;
			while (n >= 1)
			{
				result++;
				n /= 2;
			}
			return result;
		}
	}

	static void IntrospectiveSort<T>(IList<T> keys, int lo, int hi, int depthLimit, Comparison<T> comparison)
	{
		while (hi > lo)
		{
			int partitionSize = hi - lo + 1;
			if (partitionSize <= s_IntrospectiveSortSizeThreshold)
			{
				if (partitionSize == 1)
				{
					return;
				}
				if (partitionSize == 2)
				{
					SwapIfGreater(keys, comparison, lo, hi);
					return;
				}
				if (partitionSize == 3)
				{
					SwapIfGreater(keys, comparison, lo, hi - 1);
					SwapIfGreater(keys, comparison, lo, hi);
					SwapIfGreater(keys, comparison, hi - 1, hi);
					return;
				}

				InsertionSort(keys, lo, hi, comparison);
				return;
			}

			if (depthLimit == 0)
			{
				Heapsort(keys, lo, hi, comparison);
				return;
			}
			depthLimit--;

			int p = PickPivotAndPartition(keys, lo, hi, comparison);
			// Note we've already partitioned around the pivot and do not have to move the pivot again.
			IntrospectiveSort(keys, p + 1, hi, depthLimit, comparison);
			hi = p - 1;
		}
	}

	static int PickPivotAndPartition<T>(IList<T> keys, int lo, int hi, Comparison<T> comparison)
	{
		// Compute median-of-three. But also partition them, since we've done the comparison.
		int middle = lo + ((hi - lo) / 2);

		// Sort lo, mid and hi appropriately, then pick mid as the pivot.
		SwapIfGreater(keys, comparison, lo, middle);  // swap the low with the mid point
		SwapIfGreater(keys, comparison, lo, hi);   // swap the low with the high
		SwapIfGreater(keys, comparison, middle, hi); // swap the middle with the high

		T pivot = keys[middle];
		Swap(keys, middle, hi - 1);
		int left = lo, right = hi - 1;  // We already partitioned lo and hi and put the pivot in hi - 1.  And we pre-increment & decrement below.

		while (left < right)
		{
			while (comparison(keys[++left], pivot) < 0) ;
			while (comparison(pivot, keys[--right]) < 0) ;

			if (left >= right)
				break;

			Swap(keys, left, right);
		}

		// Put pivot in the right location.
		Swap(keys, left, hi - 1);
		return left;
	}

	private static void Swap<T>(IList<T> a, int i, int j)
	{
		if (i != j)
			(a[i], a[j]) = (a[j], a[i]);
	}

	static void SwapIfGreater<T>(IList<T> keys, Comparison<T> comparison, int a, int b)
	{
		if ((a != b) && (comparison(keys[a], keys[b]) > 0))
			(keys[a], keys[b]) = (keys[b], keys[a]);
	}
}
