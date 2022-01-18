using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tortuga.Anchor;

namespace Tests.Collections;

[TestClass()]
public class NaturalSortComparerTests
{
	[TestMethod]
	public void Test1()
	{
		var input = new[] { "6.04", "6.01", "6.03", "6#04" };
		var expected = new[] { "6.01", "6.03", "6.04", "6#04" };
		TestCore(input, expected);
	}

	[TestMethod]
	public void Test2()
	{
		var input = new[] { "Plate 1", "Plate 10", "Plate 11", "Plate 12", "Plate 13", "Plate 14", "Plate 15", "Plate 16", "Plate 17", "Plate 18", "Plate 19", "Plate 2", "Plate 20", "Plate 21", "Plate 22", "Plate 23", "Plate 24", "Plate 25", "Plate 26", "Plate 27", "Plate 28", "Plate 29", "Plate 3", "Plate 30", "Plate 31", "Plate 32", "Plate 33", "Plate 34", "Plate 35", "Plate 36" };
		var expected = new[] { "Plate 1", "Plate 2", "Plate 3", "Plate 10", "Plate 11", "Plate 12", "Plate 13", "Plate 14", "Plate 15", "Plate 16", "Plate 17", "Plate 18", "Plate 19", "Plate 20", "Plate 21", "Plate 22", "Plate 23", "Plate 24", "Plate 25", "Plate 26", "Plate 27", "Plate 28", "Plate 29", "Plate 30", "Plate 31", "Plate 32", "Plate 33", "Plate 34", "Plate 35", "Plate 36" };
		TestCore(input, expected);
	}

	static void TestCore(string[] input, string[] expected)
	{
		var output = input.OrderBy(s => s, new NaturalSortComparer()).ToList();

		Assert.AreEqual(input.Length, expected.Length, "Test setup failure");
		Assert.AreEqual(input.Length, output.Count);
		for (var i = 0; i < input.Length; i++)
			Assert.AreEqual(expected[i], output[i], "Element " + i);
	}
}
