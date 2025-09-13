using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using Tortuga.Anchor;

namespace Tests;

[TestClass]
public class StringUtilitiesTests
{
	[TestMethod]
	public void IsNullOrEmpty_WithEmpty_ReturnsReplacement()
	{
		string input = "";
		var result = input.IsNullOrEmpty("replacement");
		Assert.AreEqual("replacement", result);
	}

	[TestMethod]
	public void IsNullOrEmpty_WithNull_ReturnsReplacement()
	{
		string? input = null;
		var result = input.IsNullOrEmpty("replacement");
		Assert.AreEqual("replacement", result);
	}

	[TestMethod]
	public void IsNullOrEmpty_WithValue_ReturnsValue()
	{
		string input = "value";
		var result = input.IsNullOrEmpty("replacement");
		Assert.AreEqual("value", result);
	}

	[TestMethod]
	public void IsNullOrWhiteSpace_WithEmpty_ReturnsReplacement()
	{
		string input = "";
		var result = input.IsNullOrWhiteSpace("replacement");
		Assert.AreEqual("replacement", result);
	}

	[TestMethod]
	public void IsNullOrWhiteSpace_WithNull_ReturnsReplacement()
	{
		string? input = null;
		var result = input.IsNullOrWhiteSpace("replacement");
		Assert.AreEqual("replacement", result);
	}

	[TestMethod]
	public void IsNullOrWhiteSpace_WithValue_ReturnsValue()
	{
		string input = "value";
		var result = input.IsNullOrWhiteSpace("replacement");
		Assert.AreEqual("value", result);
	}

	[TestMethod]
	public void IsNullOrWhiteSpace_WithWhitespace_ReturnsReplacement()
	{
		string input = "   ";
		var result = input.IsNullOrWhiteSpace("replacement");
		Assert.AreEqual("replacement", result);
	}

	[TestMethod]
	public void StringBuilder_IndexOf_FindsSubstring()
	{
		var sb = new StringBuilder("Hello, world!");
		int idx = sb.IndexOf("world");
		Assert.AreEqual(7, idx);
	}

	[TestMethod]
	public void StringBuilder_IndexOf_NotFound()
	{
		var sb = new StringBuilder("Hello, world!");
		int idx = sb.IndexOf("foo");
		Assert.AreEqual(-1, idx);
	}

	[TestMethod]
	public void StringBuilder_IndexOf_EmptySubstring()
	{
		var sb = new StringBuilder("Hello");
		int idx = sb.IndexOf("");
		Assert.AreEqual(0, idx);
	}

	[TestMethod]
	public void StringBuilder_LastIndexOf_FindsSubstring()
	{
		var sb = new StringBuilder("ababcababc");
		int idx = sb.LastIndexOf("abc");
		Assert.AreEqual(7, idx);
	}

	[TestMethod]
	public void StringBuilder_LastIndexOf_NotFound()
	{
		var sb = new StringBuilder("Hello, world!");
		int idx = sb.LastIndexOf("foo");
		Assert.AreEqual(-1, idx);
	}

	[TestMethod]
	public void StringBuilder_LastIndexOf_EmptySubstring()
	{
		var sb = new StringBuilder("Hello");
		int idx = sb.LastIndexOf("");
		Assert.AreEqual(4, idx);
	}

	[TestMethod]
	public void StringBuilder_IndexOf_WithStartIndex()
	{
		var sb = new StringBuilder("ababcababc");
		int idx = sb.IndexOf("abc", 3);
		Assert.AreEqual(7, idx);
	}

	[TestMethod]
	public void StringBuilder_LastIndexOf_WithStartIndex()
	{
		var sb = new StringBuilder("ababcababc");
		int idx = sb.LastIndexOf("abc", 6);
		Assert.AreEqual(2, idx);
	}

	[TestMethod]
	public void StringBuilder_IndexOf_StringComparison_FindsSubstring()
	{
		var sb = new StringBuilder("Hello, WORLD!");
		int idx = sb.IndexOf("world", StringComparison.OrdinalIgnoreCase);
		Assert.AreEqual(7, idx);
	}

	[TestMethod]
	public void StringBuilder_IndexOf_StringComparison_NotFound()
	{
		var sb = new StringBuilder("Hello, world!");
		int idx = sb.IndexOf("foo", StringComparison.OrdinalIgnoreCase);
		Assert.AreEqual(-1, idx);
	}

	[TestMethod]
	public void StringBuilder_LastIndexOf_StringComparison_FindsSubstring()
	{
		var sb = new StringBuilder("abAbcABabc");
		int idx = sb.LastIndexOf("abc", StringComparison.OrdinalIgnoreCase);
		Assert.AreEqual(7, idx);
	}

	[TestMethod]
	public void StringBuilder_LastIndexOf_StringComparison_NotFound()
	{
		var sb = new StringBuilder("Hello, world!");
		int idx = sb.LastIndexOf("foo", StringComparison.OrdinalIgnoreCase);
		Assert.AreEqual(-1, idx);
	}

	[TestMethod]
	public void StringBuilder_IndexOf_StringComparison_WithStartIndex()
	{
		var sb = new StringBuilder("abAbcABabc");
		int idx = sb.IndexOf("abc", 3, StringComparison.OrdinalIgnoreCase);
		Assert.AreEqual(7, idx);
	}

	[TestMethod]
	public void StringBuilder_LastIndexOf_StringComparison_WithStartIndex()
	{
		var sb = new StringBuilder("abAbcABabc");
		int idx = sb.LastIndexOf("abc", 6, StringComparison.OrdinalIgnoreCase);
		Assert.AreEqual(2, idx);
	}

	[TestMethod]
	public void StringBuilder_IndexOf_OrdinalIgnoreCase_FindsSubstring_NoToString()
	{
		var sb = new StringBuilder("Hello, WORLD!");
		int idx = sb.IndexOf("world", StringComparison.OrdinalIgnoreCase);
		Assert.AreEqual(7, idx);
	}

	[TestMethod]
	public void StringBuilder_IndexOf_Ordinal_FindsSubstring_NoToString()
	{
		var sb = new StringBuilder("Hello, world!");
		int idx = sb.IndexOf("world", StringComparison.Ordinal);
		Assert.AreEqual(7, idx);
	}

	[TestMethod]
	public void StringBuilder_LastIndexOf_OrdinalIgnoreCase_FindsSubstring_NoToString()
	{
		var sb = new StringBuilder("abAbcABabc");
		int idx = sb.LastIndexOf("abc", StringComparison.OrdinalIgnoreCase);
		Assert.AreEqual(7, idx);
	}

	[TestMethod]
	public void StringBuilder_LastIndexOf_Ordinal_FindsSubstring_NoToString()
	{
		var sb = new StringBuilder("ababcababc");
		int idx = sb.LastIndexOf("abc", StringComparison.Ordinal);
		Assert.AreEqual(7, idx);
	}

	[TestMethod]
	public void StringUtilities_IndexOf_StringComparison_WithStartIndex_NoToString()
	{
		var sb = new StringBuilder("abAbcABabc");
		int idx = StringUtilities.IndexOf(sb, "abc", 3, StringComparison.OrdinalIgnoreCase);
		Assert.AreEqual(7, idx);
	}

	[TestMethod]
	public void StringUtilities_LastIndexOf_StringComparison_WithStartIndex_NoToString()
	{
		var sb = new StringBuilder("abAbcABabc");
		int idx = StringUtilities.LastIndexOf(sb, "abc", 6, StringComparison.OrdinalIgnoreCase);
		Assert.AreEqual(2, idx);
	}
}
