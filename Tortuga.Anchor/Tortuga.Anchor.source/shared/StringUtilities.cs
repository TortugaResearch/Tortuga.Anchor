using System;
using System.Collections.Generic;
using System.Text;

namespace Tortuga.Anchor
{


    /// <summary>
    /// Extension methods for String.
    /// </summary>
    public static class StringUtilities
    {
        [ThreadStatic]
        private static StringBuilder ts_CachedStringBuilder;

        /// <summary>
        /// The maximum size of a string builder this will cache. 
        /// </summary>
        /// <remarks>The default value is 500. You may need to modify this based on your usage.</remarks>
        public static int MaxSize { get; set; } = 500;

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
        /// Indicates whether a specified string is null, empty, or consists only of white-space
        /// characters.</summary>
        /// <param name="value">The string to test</param>
        public static bool IsNullOrEmpty(this string value) => string.IsNullOrEmpty(value);

        /// <summary>
        /// Indicates whether a specified string is null, empty, or consists only of white-space
        /// characters.</summary>
        /// <param name="value">The string to test</param>
        public static bool IsNullOrWhiteSpace(this string value) => string.IsNullOrWhiteSpace(value);


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
            if (values == null)
                throw new ArgumentNullException("values");

            if (!Enum.IsDefined(typeof(StringJoinOption), option))
                throw new ArgumentOutOfRangeException("option", option, "Option is not defined.");

            if (option == StringJoinOption.None) //no options means we can use the built-in version
                return string.Join(separator, values);

            if (separator == null)
                separator = "";

            using (var en = values.GetEnumerator())
            {
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

                    return ReleaseStringBuilder(result);
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
                    return ReleaseStringBuilder(result);

                }
                else
                    throw new NotImplementedException(); //would only happen if we added a new option type but forgot to implement it.

            }

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
            if (values == null)
                throw new ArgumentNullException("values");

            if (!Enum.IsDefined(typeof(StringJoinOption), option))
                throw new ArgumentOutOfRangeException("option", option, "Option is not defined.");

            if (option == StringJoinOption.None) //no options means we can use the built-in version
                return string.Join(separator, values);

            if (separator == null)
                separator = "";

            int? capacity = null;
            var list = values as IReadOnlyList<string>;
            if (list != null)
            {
                foreach (var item in list)
                    capacity += item?.Length ?? 0;
                capacity += separator.Length * (list.Count - 1);
            }

            using (var en = values.GetEnumerator())
            {
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

                    return ReleaseStringBuilder(result);
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
                    return ReleaseStringBuilder(result);

                }
                else
                    throw new NotImplementedException(); //would only happen if we added a new option type but forgot to implement it.

            }

        }
        /// <summary>
        /// Releases the specified string builder back to the cache.
        /// </summary>
        /// <param name="stringBuilder">The string builder to release. Once released, it can no longer be used until reacquired.</param>
        /// <returns>Contents of the string builder.</returns>
        /// <remarks>Though not strictly required, it is perferable to release a string builder onto the same thread that acquired it.</remarks>
        public static string ReleaseStringBuilder(StringBuilder stringBuilder)
        {
            var result = stringBuilder.ToString();

            stringBuilder.Clear(); //Though not strictly necessary, this will make accidental reuse after release more visible.

            if (stringBuilder.Capacity <= MaxSize)
                ts_CachedStringBuilder = stringBuilder.Clear();

            return result;
        }



    }
}
