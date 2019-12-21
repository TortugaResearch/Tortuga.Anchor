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
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ValidCollectionSizesAttribute : Attribute, ITestDataSource
    {
        public IEnumerable<object[]> GetData(MethodInfo methodInfo)
        {
            yield return new object[] { 0 };
            yield return new object[] { 1 };
            yield return new object[] { 75 };
        }

        public string GetDisplayName(MethodInfo methodInfo, object[] data)
        {
            if (data != null)
                return string.Format(CultureInfo.CurrentCulture, "{0} ({1})", methodInfo.Name, string.Join(",", data));

            return null;
        }
    }
}
