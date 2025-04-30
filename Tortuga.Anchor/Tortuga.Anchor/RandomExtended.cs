﻿namespace Tortuga.Anchor;

/// <summary>
/// An extended version of the Random class.
/// </summary>
public class RandomExtended : Random
{
	/// <summary>
	/// Initializes a new instance of the <see cref="RandomExtended"/> class.
	/// </summary>

	public RandomExtended()
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="RandomExtended"/> class.
	/// </summary>
	/// <param name="seed">A number used to calculate a starting value for the pseudo-random number sequence. If a negative number is specified, the absolute value of the number is used.</param>

	public RandomExtended(int seed)
		: base(seed)
	{
	}

	/// <summary>
	/// Choose one item from the list, leaving the original list unaltered.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="list">The list.</param>
	/// <returns></returns>
	/// <exception cref="ArgumentNullException">list;list is null.</exception>
	public T Choose<T>(IReadOnlyList<T> list)
	{
		if (list == null || list.Count == 0)
			throw new ArgumentException($"{nameof(list)} is null or empty.", nameof(list));

		return list[Next(0, list.Count)];
	}

	/// <summary>
	/// Choose count items from the list, leaving the original list unaltered.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="list">The list.</param>
	/// <param name="count">The count.</param>
	/// <param name="allowDuplicates">if set to <c>true</c> is the same item can be picked multiple times.</param>
	/// <returns></returns>
	/// <exception cref="ArgumentNullException">list;list is null.</exception>
	/// <exception cref="ArgumentOutOfRangeException">
	/// count;count must be greater than or equal to zero
	/// or
	/// count;count must be less than or equal to list.Count if allowDuplicates is false
	/// </exception>
	public List<T> Choose<T>(IReadOnlyList<T> list, int count, bool allowDuplicates = true)
	{
		if (list == null || list.Count == 0)
			throw new ArgumentException($"{nameof(list)} is null or empty.", nameof(list));

		if (count < 0)
			throw new ArgumentOutOfRangeException(nameof(count), count, $"{nameof(count)} must be greater than or equal to zero");
		if (list.Count < count && !allowDuplicates)
			throw new ArgumentOutOfRangeException(nameof(count), count, $"{nameof(count)} must be less than or equal to {nameof(list)}.Count if {nameof(allowDuplicates)} is false");

		var result = new List<T>();

		if (count == 0)
			return result;

		if (allowDuplicates)
		{
			for (var x = 0; x < count; x++)
				result.Add(Choose(list));
		}
		else
		{
			var temp = list.ToList();
			for (var x = 0; x < count; x++)
				result.Add(Pick(temp));
		}

		return result;
	}

	/// <summary>
	/// Returns a random date/time that is within the indicated time span.
	/// </summary>
	/// <param name="minValue">The inclusive minimum value.</param>
	/// <param name="maxValue">The exclusive maximum value.</param>
	/// <returns></returns>
	/// <exception cref="ArgumentOutOfRangeException">maxValue;maxValue must be greater than or equal to minValue.</exception>
	public DateTime NextDateTime(DateTime minValue, DateTime maxValue)
	{
		if (minValue > maxValue)
			throw new ArgumentOutOfRangeException(nameof(maxValue), maxValue, $"{nameof(maxValue)} must be greater than or equal to {nameof(minValue)}.");

		return NextDateTime(minValue, maxValue - minValue);
	}

	/// <summary>
	/// Returns a random date/time that is within the indicated time span.
	/// </summary>
	/// <param name="minValue">The inclusive minimum value.</param>
	/// <param name="maxSpan">The exclusive maximum span.</param>
	/// <returns></returns>
	public DateTime NextDateTime(DateTime minValue, TimeSpan maxSpan)
	{
		if (maxSpan.Ticks < 0)
			throw new ArgumentOutOfRangeException(nameof(maxSpan), maxSpan, $"{nameof(maxSpan)} must be greater than or equal to 0");

		return minValue.AddTicks((long)(NextDouble() * maxSpan.Ticks));
	}

	/// <summary>
	/// Returns a random date/time that is within the indicated time span.
	/// </summary>
	/// <param name="minValue">The inclusive minimum value.</param>
	/// <param name="maxSpan">The exclusive maximum span.</param>
	/// <returns></returns>
	public DateTimeOffset NextDateTimeOffset(DateTimeOffset minValue, TimeSpan maxSpan)
	{
		if (maxSpan.Ticks < 0)
			throw new ArgumentOutOfRangeException(nameof(maxSpan), maxSpan, $"{nameof(maxSpan)} must be greater than or equal to 0");

		return minValue.AddTicks(NextInt64(maxSpan.Ticks));
	}

	/// <summary>
	/// Returns a random date/time that is within the indicated range.
	/// </summary>
	/// <param name="minValue">The inclusive minimum value.</param>
	/// <param name="maxValue">The exclusive maximum value.</param>
	/// <returns></returns>
	/// <exception cref="ArgumentOutOfRangeException">maxValue;maxValue must be greater than or equal to minValue.</exception>
	public DateTimeOffset NextDateTimeOffset(DateTimeOffset minValue, DateTimeOffset maxValue)
	{
		if (minValue > maxValue)
			throw new ArgumentOutOfRangeException(nameof(maxValue), maxValue, $"{nameof(maxValue)} must be greater than or equal to {nameof(minValue)}.");

		return NextDateTimeOffset(minValue, maxValue - minValue);
	}

	/// <summary>
	/// Returns a random decimal that is less than maxValue
	/// </summary>
	/// <param name="maxValue">The exclusive maximum value.</param>
	/// <returns></returns>
	/// <exception cref="ArgumentOutOfRangeException">maxValue;maxValue must be greater than or equal to 0.</exception>
	public decimal NextDecimal(decimal maxValue)
	{
		if (maxValue < 0)
			throw new ArgumentOutOfRangeException(nameof(maxValue), maxValue, $"nameof(maxValue) must be greater than or equal to 0.");

		return NextDecimal(0m, maxValue);
	}

	/// <summary>
	/// Returns a random decimal that is within the indicated range
	/// </summary>
	/// <param name="minValue">The inclusive minimum value.</param>
	/// <param name="maxValue">The exclusive maximum value.</param>
	/// <returns></returns>
	/// <exception cref="ArgumentOutOfRangeException">maxValue;maxValue must be greater than or equal to minValue.</exception>
	public decimal NextDecimal(decimal minValue, decimal maxValue)
	{
		if (minValue > maxValue)
			throw new ArgumentOutOfRangeException(nameof(maxValue), maxValue, $"{nameof(maxValue)} must be greater than or equal to {nameof(minValue)}.");

		return ((decimal)NextDouble() * (maxValue - minValue)) + minValue;
	}

	/// <summary>
	/// Pick and remove one item from the list.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="list">The list.</param>
	/// <returns></returns>
	/// <exception cref="ArgumentNullException">list;list is null.</exception>
	/// <exception cref="ArgumentException">List cannot be read-only;list</exception>
	public T Pick<T>(IList<T> list)
	{
		if (list == null || list.Count == 0)
			throw new ArgumentException($"{nameof(list)} is null or empty.", nameof(list));
		if (list.IsReadOnly)
			throw new ArgumentException($"{nameof(list)} cannot be read-only", nameof(list));

		var index = Next(0, list.Count);
		var result = list[index];
		list.RemoveAt(index);
		return result;
	}

	/// <summary>
	/// Pick and remove count items from the list.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="list">The list.</param>
	/// <param name="count">The count.</param>
	/// <returns></returns>
	/// <exception cref="ArgumentNullException">list;list is null.</exception>
	/// <exception cref="ArgumentException">List cannot be read-only;list</exception>
	/// <exception cref="ArgumentOutOfRangeException">
	/// count;count must be greater than or equal to zero
	/// or
	/// count;count must be less than or equal to list.Count
	/// </exception>
	public List<T> Pick<T>(IList<T> list, int count)
	{
		if (list == null || list.Count == 0)
			throw new ArgumentException($"{nameof(list)} is null or empty.", nameof(list));
		if (list.IsReadOnly)
			throw new ArgumentException($"{nameof(list)} cannot be read-only", nameof(list));
		if (count < 0)
			throw new ArgumentOutOfRangeException(nameof(count), count, $"{nameof(count)} must be greater than or equal to zero");
		if (list.Count < count)
			throw new ArgumentOutOfRangeException(nameof(count), count, $"{nameof(count)} must be less than or equal to {nameof(list)}.Count");

		var result = new List<T>();

		if (count == 0)
			return result;

		for (var x = 0; x < count; x++)
			result.Add(Pick(list));

		return result;
	}
}
