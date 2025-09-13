using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Tortuga.Anchor;

/// <summary>
/// Extension methods for String.
/// </summary>
public static class StringUtilities
{
	[ThreadStatic]
	private static StringBuilder? ts_CachedStringBuilder;

	/// <summary>
	/// The maximum size of a string builder this will cache.
	/// </summary>
	/// <remarks>The default value is 1000. You may need to modify this based on your usage.</remarks>
	public static int MaxSize { get; set; } = 1000;

	/// <summary>
	/// Acquires a string builder of the indicated capacity.
	/// </summary>
	/// <param name="capacity">The desired capacity. If larger than MaxSize, a new instance of StringBuilder will be returned.</param>
	/// <returns>StringBuilder.</returns>
	/// <remarks>Caching is performed on a per-thread basis.</remarks>
	public static StringBuilder AcquireStringBuilder(int? capacity = null)
	{
		//Note: This is thread-safe only because CachedInstance is marked a ThreadStatic.

		if (capacity == null)
			capacity = 16; //default for .NET

		if (capacity > MaxSize)
			return new StringBuilder(capacity.Value); //we don't want to cache overly large string builders

		var result = ts_CachedStringBuilder;

		if (result != null && capacity <= result.Capacity) //if the cached string builder is too small, just get a new one.
		{
			ts_CachedStringBuilder = null;
			result.Clear(); //redundant with ReleaseStringBuilder's behavior, but in case of accidental use after release we're a bit safer.
			return result;
		}

		return new StringBuilder(capacity.Value);
	}

	/// <summary>
	/// Returns the index of the first occurrence of a substring in the StringBuilder, or -1 if not found.
	/// </summary>
	/// <param name="stringBuilder">The StringBuilder to search.</param>
	/// <param name="value">The substring to seek.</param>
	/// <param name="startIndex">The starting index for the search.</param>
	/// <returns>The zero-based index position of value if found; otherwise, -1.</returns>
	public static int IndexOf(this StringBuilder stringBuilder, string value, int startIndex = 0)
	{
		return IndexOf(stringBuilder, value, startIndex, StringComparison.Ordinal);
	}


	/// <summary>
	/// Returns the index of the first occurrence of a substring in the StringBuilder, using the specified StringComparison, or -1 if not found.
	/// </summary>
	/// <param name="stringBuilder">The StringBuilder to search.</param>
	/// <param name="value">The substring to seek.</param>
	/// <param name="startIndex">The starting index for the search.</param>
	/// <param name="comparisonType">The StringComparison option to use.</param>
	/// <returns>The zero-based index position of value if found; otherwise, -1.</returns>
	/// <remarks>This will allocate memory if you use a comparison type other than Ordinal or OrdinalIgnoreCase</remarks>
	public static int IndexOf(this StringBuilder stringBuilder, string value, int startIndex, StringComparison comparisonType)
	{
		ArgumentNullException.ThrowIfNull(stringBuilder);
		ArgumentNullException.ThrowIfNull(value);

		var sbLength = stringBuilder.Length;
		var length = value.Length;

		if (length == 0)
			return startIndex < sbLength ? startIndex : -1;

		if (startIndex < 0 || startIndex > sbLength - length)
			return -1;

		switch (comparisonType)
		{
			case StringComparison.Ordinal:
				{
					for (var i = startIndex; i <= sbLength - length; i++)
					{
						var found = true;
						for (var j = 0; j < length; j++)
						{
							if (stringBuilder[i + j] != value[j])
							{
								found = false;
								break;
							}
						}
						if (found)
							return i;
					}
					return -1;
				}

			case StringComparison.OrdinalIgnoreCase:
				{
					for (var i = startIndex; i <= sbLength - length; i++)
					{
						var found = true;
						for (var j = 0; j < length; j++)
						{
							if (char.ToUpperInvariant(stringBuilder[i + j]) != char.ToUpperInvariant(value[j]))
							{
								found = false;
								break;
							}
						}
						if (found)
							return i;
					}
					return -1;
				}

			default:
				//take the slow path for other comparison types
				return stringBuilder.ToString().IndexOf(value, startIndex, comparisonType);
		}
	}

	/// <summary>
	/// Returns the index of the first occurrence of a substring in the StringBuilder, using the specified StringComparison, or -1 if not found.
	/// </summary>
	/// <param name="stringBuilder">The StringBuilder to search.</param>
	/// <param name="value">The substring to seek.</param>
	/// <param name="comparisonType">The StringComparison option to use.</param>
	/// <returns>The zero-based index position of value if found; otherwise, -1.</returns>
	public static int IndexOf(this StringBuilder stringBuilder, string value, StringComparison comparisonType)
	{
		return stringBuilder.IndexOf(value, 0, comparisonType);
	}

	/// <summary>
	/// Indicates whether a specified string is null, empty, or consists only of white-space
	/// characters.</summary>
	/// <param name="value">The string to test</param>
	public static bool IsNullOrEmpty([NotNullWhen(false)] this string? value) => string.IsNullOrEmpty(value);

	/// <summary>
	/// Returns <paramref name="replacementValue"/> if <paramref name="value"/> is null or empty; otherwise returns <paramref name="value"/>.
	/// Mimics SQL Server's ISNULL function for strings.
	/// </summary>
	/// <param name="value">The string to check.</param>
	/// <param name="replacementValue">The value to use if <paramref name="value"/> is null or empty.</param>
	/// <returns>The original string or the replacement value.</returns>
	public static string IsNullOrEmpty(this string? value, string replacementValue)
	{
		return string.IsNullOrEmpty(value) ? replacementValue : value!;
	}

	/// <summary>
	/// Indicates whether a specified string is null, empty, or consists only of white-space
	/// characters.</summary>
	/// <param name="value">The string to test</param>
	public static bool IsNullOrWhiteSpace([NotNullWhen(false)] this string? value) => string.IsNullOrWhiteSpace(value);

	/// <summary>
	/// Returns <paramref name="replacementValue"/> if <paramref name="value"/> is null, empty, or consists only of white-space characters; otherwise returns <paramref name="value"/>.
	/// Mimics SQL Server's ISNULL function for strings, but uses IsNullOrWhiteSpace semantics.
	/// </summary>
	/// <param name="value">The string to check.</param>
	/// <param name="replacementValue">The value to use if <paramref name="value"/> is null, empty, or white-space.</param>
	/// <returns>The original string or the replacement value.</returns>
	public static string IsNullOrWhiteSpace(this string? value, string replacementValue)
	{
		return string.IsNullOrWhiteSpace(value) ? replacementValue : value!;
	}

	/// <summary>
	/// Joins the list of values using the specified separator and options.
	/// </summary>
	/// <param name="values">The values.</param>
	/// <param name="separator">The separator.</param>
	/// <param name="option">The option.</param>
	/// <returns>System.String.</returns>
	/// <exception cref="ArgumentNullException">values</exception>
	public static string Join<T>(this IEnumerable<T> values, string separator, StringJoinOption option = StringJoinOption.None)
	{
		ArgumentNullException.ThrowIfNull(values);

		if (!Enum.IsDefined(option))
			throw new ArgumentOutOfRangeException(nameof(option), option, "Option is not defined.");

		if (option == StringJoinOption.None) //no options means we can use the built-in version
			return string.Join(separator, values);

		if (separator == null)
			separator = "";

		using var en = values.GetEnumerator();

		if (!en.MoveNext())
			return "";

		if (option == StringJoinOption.DiscardEmptyAndNull)
		{
			//find the first non-empty string
			while (string.IsNullOrEmpty(en.Current?.ToString()))
			{
				var moreRecords = en.MoveNext();
				if (!moreRecords)
					return "";
			}

			var result = AcquireStringBuilder();
			result.Append(en.Current);

			//add the rest
			while (en.MoveNext())
			{
				if (!string.IsNullOrEmpty(en.Current?.ToString()))
				{
					result.Append(separator);
					result.Append(en.Current);
				}
			}

			return Release(result);
		}
		else if (option == StringJoinOption.DiscardNulls)
		{
			//find the first non-null string
			while (en.Current == null)
			{
				var moreRecords = en.MoveNext();
				if (!moreRecords)
					return "";
			}

			var result = AcquireStringBuilder();
			result.Append(en.Current);

			//add the rest
			while (en.MoveNext())
			{
				if (en.Current != null)
				{
					result.Append(separator);
					result.Append(en.Current);
				}
			}
			return Release(result);
		}
		else
			throw new ArgumentOutOfRangeException(nameof(option), option, "Option is not implemented.");
	}

	/// <summary>
	/// Joins the list of strings using the specified separator and options.
	/// </summary>
	/// <param name="values">The values.</param>
	/// <param name="separator">The separator.</param>
	/// <param name="option">The option.</param>
	/// <returns>System.String.</returns>
	/// <exception cref="ArgumentNullException">values</exception>
	public static string Join(this IEnumerable<string> values, string separator, StringJoinOption option = StringJoinOption.None)
	{
		ArgumentNullException.ThrowIfNull(values);

		if (!Enum.IsDefined(option))
			throw new ArgumentOutOfRangeException(nameof(option), option, "Option is not defined.");

		if (option == StringJoinOption.None) //no options means we can use the built-in version
			return string.Join(separator, values);

		if (separator == null)
			separator = "";

		int? capacity = null;

		if (values is IReadOnlyList<string> list)
		{
			foreach (var item in list)
				capacity += item?.Length ?? 0;
			capacity += separator.Length * (list.Count - 1);
		}

		using var en = values.GetEnumerator();

		if (!en.MoveNext())
			return "";

		if (option == StringJoinOption.DiscardEmptyAndNull)
		{
			//find the first non-empty string
			while (string.IsNullOrEmpty(en.Current))
			{
				var moreRecords = en.MoveNext();
				if (!moreRecords)
					return "";
			}

			var result = AcquireStringBuilder(capacity);
			result.Append(en.Current);

			//add the rest
			while (en.MoveNext())
			{
				if (!string.IsNullOrEmpty(en.Current))
				{
					result.Append(separator);
					result.Append(en.Current);
				}
			}

			return Release(result);
		}
		else if (option == StringJoinOption.DiscardNulls)
		{
			//find the first non-null string
			while (en.Current == null)
			{
				var moreRecords = en.MoveNext();
				if (!moreRecords)
					return "";
			}

			var result = AcquireStringBuilder(capacity);
			result.Append(en.Current);

			//add the rest
			while (en.MoveNext())
			{
				if (en.Current != null)
				{
					result.Append(separator);
					result.Append(en.Current);
				}
			}
			return result.Release();
		}
		else
			throw new ArgumentOutOfRangeException(nameof(option), option, "Option is not implemented.");
	}

	/// <summary>
	/// Returns the index of the last occurrence of a substring in the StringBuilder, or -1 if not found.
	/// </summary>
	/// <param name="stringBuilder">The StringBuilder to search.</param>
	/// <param name="value">The substring to seek.</param>
	/// <param name="startIndex">The starting index for the search (searches backward from here).</param>
	/// <returns>The zero-based index position of value if found; otherwise, -1.</returns>
	public static int LastIndexOf(this StringBuilder stringBuilder, string value, int? startIndex = null)
	{
		return LastIndexOf(stringBuilder, value, startIndex, StringComparison.Ordinal);
	}

	/// <summary>
	/// Returns the index of the last occurrence of a substring in the StringBuilder, using the specified StringComparison, or -1 if not found.
	/// </summary>
	/// <param name="sb">The StringBuilder to search.</param>
	/// <param name="value">The substring to seek.</param>
	/// <param name="startIndex">The starting index for the search (searches backward from here).</param>
	/// <param name="comparisonType">The StringComparison option to use.</param>
	/// <returns>The zero-based index position of value if found; otherwise, -1.</returns>
	public static int LastIndexOf(this StringBuilder sb, string value, int? startIndex, StringComparison comparisonType)
	{
		ArgumentNullException.ThrowIfNull(sb);
		ArgumentNullException.ThrowIfNull(value);

		var length = value.Length;
		var sbLength = sb.Length;

		if (length == 0)
			return (startIndex ?? sbLength - 1) < sbLength ? (startIndex ?? sbLength - 1) : -1;

		var start = startIndex ?? sbLength - length;
		if (start < 0 || start > sbLength - length)
			start = sbLength - length;

		switch (comparisonType)
		{
			case StringComparison.Ordinal:
				{
					for (var i = start; i >= 0; i--)
					{
						var found = true;
						for (var j = 0; j < length; j++)
						{
							if (sb[i + j] != value[j])
							{
								found = false;
								break;
							}
						}
						if (found)
							return i;
					}
					return -1;
				}

			case StringComparison.OrdinalIgnoreCase:
				{
					for (var i = start; i >= 0; i--)
					{
						var found = true;
						for (var j = 0; j < length; j++)
						{
							if (char.ToUpperInvariant(sb[i + j]) != char.ToUpperInvariant(value[j]))
							{
								found = false;
								break;
							}
						}
						if (found)
							return i;
					}
					return -1;
				}

			default:
				//take the slow path for other comparison types
				return sb.ToString().LastIndexOf(value, start, comparisonType);
		}
	}

	/// <summary>
	/// Returns the index of the last occurrence of a substring in the StringBuilder, using the specified StringComparison, or -1 if not found. 
	/// </summary>
	/// <param name="stringBuilder">The StringBuilder to search.</param>
	/// <param name="value">The substring to seek.</param>
	/// <param name="comparisonType">The StringComparison option to use.</param>
	/// <returns>The zero-based index position of value if found; otherwise, -1.</returns>
	public static int LastIndexOf(this StringBuilder stringBuilder, string value, StringComparison comparisonType)
	{
		return stringBuilder.LastIndexOf(value, null, comparisonType);
	}

	/// <summary>
	/// Releases the specified string builder back to the cache.
	/// </summary>
	/// <param name="stringBuilder">The string builder to release. Once released, it can no longer be used until re-acquired.</param>
	/// <returns>Contents of the string builder.</returns>
	/// <remarks>Though not strictly required, it is preferable to release a string builder onto the same thread that acquired it.</remarks>
	public static string Release(this StringBuilder stringBuilder)
	{
		if (stringBuilder == null)
			throw new ArgumentNullException(nameof(stringBuilder), $"{nameof(stringBuilder)} is null.");

		var result = stringBuilder.ToString();

		stringBuilder.Clear(); //Though not strictly necessary, this will make accidental reuse after release more visible.

		//Keep the largest string builder under max capacity.
		if (stringBuilder.Capacity <= MaxSize && ts_CachedStringBuilder?.Capacity < stringBuilder.Capacity)
			ts_CachedStringBuilder = stringBuilder;

		return result;
	}
}
