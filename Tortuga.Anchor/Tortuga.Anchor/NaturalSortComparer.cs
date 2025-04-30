using System.Collections.Concurrent;
using System.Globalization;

namespace Tortuga.Anchor;

/// <summary>
/// IComparer used for natural sorts.
/// </summary>
public class NaturalSortComparer : IComparer<string?>
{
	readonly bool m_IgnoreCase;
	readonly CultureInfo m_Culture;
	readonly ConcurrentDictionary<string, SortKey> m_Cache = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="NaturalSortComparer" /> class using InvariantCulture.
	/// </summary>
	public NaturalSortComparer() : this(false, CultureInfo.InvariantCulture)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="NaturalSortComparer" /> class.
	/// </summary>
	/// <param name="culture">The culture used for number parsing and comparisons.</param>
	public NaturalSortComparer(CultureInfo culture) : this(false, culture)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="NaturalSortComparer" /> class using InvariantCulture.
	/// </summary>
	/// <param name="ignoreCase">True if case should be ignored.</param>
	public NaturalSortComparer(bool ignoreCase) : this(ignoreCase, CultureInfo.InvariantCulture)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="NaturalSortComparer"/> class.
	/// </summary>
	/// <param name="ignoreCase">True if case should be ignored.</param>
	/// <param name="culture">The culture used for number parsing and comparisons.</param>
	public NaturalSortComparer(bool ignoreCase, CultureInfo culture)
	{
		m_IgnoreCase = ignoreCase;
		m_Culture = culture;
	}

	/// <summary>
	/// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
	/// </summary>
	/// <param name="x">The first object to compare.</param>
	/// <param name="y">The second object to compare.</param>
	/// <returns>A signed integer that indicates the relative values of <paramref name="x" /> and <paramref name="y" />, as shown in the following table.
	/// Value
	/// Meaning
	/// Less than zero
	/// <paramref name="x" /> is less than <paramref name="y" />.
	/// Zero
	/// <paramref name="x" /> equals <paramref name="y" />.
	/// Greater than zero
	/// <paramref name="x" /> is greater than <paramref name="y" />.</returns>
	public int Compare(string? x, string? y)
	{
		var xKey = m_Cache.GetOrAdd(x ?? "", s => new SortKey(s, m_Culture));
		var yKey = m_Cache.GetOrAdd(y ?? "", s => new SortKey(s, m_Culture));
		return SortKey.Compare(xKey, yKey, m_IgnoreCase, m_Culture);
	}

	struct Fragment
	{
		public Fragment(string stringValue, CultureInfo culture)
		{
			StringValue = stringValue;
			if (decimal.TryParse(stringValue, NumberStyles.Any, culture, out var numericValue))
				NumericValue = numericValue;
			else
				NumericValue = null;
		}

		public decimal? NumericValue { get; set; }
		public string? StringValue { get; set; }

		public static int Compare(Fragment x, Fragment y, bool ignoreCase, CultureInfo culture)
		{
			if (x.StringValue.IsNullOrEmpty() && y.StringValue.IsNullOrEmpty())
				return 0;
			if (x.StringValue.IsNullOrEmpty())
				return -1;
			if (y.StringValue.IsNullOrEmpty())
				return 1;

			if (x.NumericValue.HasValue && !y.NumericValue.HasValue)
				return -1;
			if (y.NumericValue.HasValue && !x.NumericValue.HasValue)
				return 1;

			if (x.NumericValue.HasValue && y.NumericValue.HasValue)
			{
				if (x.NumericValue < y.NumericValue)
					return -1;
				else if (x.NumericValue == y.NumericValue)
					return 0;
				else
					return 1;
			}

#pragma warning disable CA1309 // Use ordinal string comparison
			return string.Compare(x.StringValue, y.StringValue, ignoreCase, culture);
#pragma warning restore CA1309 // Use ordinal string comparison
		}
	}

	sealed class SortKey
	{
		static readonly char[] s_SplitCharacters = new[] { ' ', '\t', '-', '=', '/', '\\', ':', '\r', '\n', '(', ')'
			, '[', ']', '{', '}', '|', '_'};

		readonly Fragment[] m_Fragments;
		readonly string? m_Value;

		public SortKey(string? value, CultureInfo culture)
		{
			m_Value = value;
			if (value.IsNullOrEmpty())
				m_Fragments = Array.Empty<Fragment>();
			else
				m_Fragments = value.Split(s_SplitCharacters).Select(f => new Fragment(f, culture)).ToArray();
		}

		public static int Compare(SortKey x, SortKey y, bool ignoreCase, CultureInfo culture)
		{
			var xFragCount = x.m_Fragments.Length;
			var yFragCount = y.m_Fragments.Length;

			//Compare each fragment
			int i = 0;
			while (i < xFragCount && i < yFragCount)
			{
				var result = Fragment.Compare(x.m_Fragments[i], y.m_Fragments[i], ignoreCase, culture);
				if (result != 0)
					return result;

				i++;
			}
			//Does one have extra fragments?
			if (xFragCount < yFragCount)
				return -1;
			else if (xFragCount > yFragCount)
				return 1;

			//Compare the raw strings. This picks up stuff we were ignoring before
#pragma warning disable CA1309 // Use ordinal string comparison
			return string.Compare(x.m_Value, y.m_Value, ignoreCase, culture);
#pragma warning restore CA1309 // Use ordinal string comparison
		}
	}
}
