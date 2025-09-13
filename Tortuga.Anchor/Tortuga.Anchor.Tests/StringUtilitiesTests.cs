using Microsoft.VisualStudio.TestTools.UnitTesting;
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
}
