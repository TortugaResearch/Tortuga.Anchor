using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.DataAnnotations;
using Tortuga.Anchor.Modeling.Internals;

namespace Tests.Modeling.Internals;

[TestClass]
public class ValidationResultEqualityComparerTests
{
	[TestMethod]
	public void ValidationResultEqualityComparer_BasicTests()
	{
		var a1 = new ValidationResult("AAA", new string[] { "FirstName", "LastName" });
		var a2 = new ValidationResult("AAA", new string[] { "FirstName", "LastName" });
		var a1clone = new ValidationResult(a1.ErrorMessage, a1.MemberNames);
		var b = new ValidationResult("BBB", new string[] { "FirstName", "LastName" });

		var c1 = new ValidationResult("AAA", null);
		var c2 = new ValidationResult("AAA", null);

		var d1 = new ValidationResult(null, new string[] { "FirstName", "LastName" });
		var d2 = new ValidationResult(null, new string[] { "FirstName", "LastName" });

		var e = new ValidationResult(null, null);

		var test = ValidationResultEqualityComparer.Default;
		Assert.IsTrue(test.Equals(a1, a1));
		Assert.IsFalse(test.Equals(a1, null));
		Assert.IsFalse(test.Equals(null, a1));
		Assert.IsFalse(test.Equals(a1, b));
		Assert.IsFalse(test.Equals(b, a1));
		Assert.IsTrue(test.Equals(c1, c1));
		Assert.IsFalse(test.Equals(a1, c1));
		Assert.IsFalse(test.Equals(c1, a1));
		Assert.IsTrue(test.Equals(c1, c2));
		Assert.IsTrue(test.Equals(a1clone, a1));
		Assert.IsTrue(test.Equals(a1, a1clone));
		Assert.IsTrue(test.Equals(a1, a2));
		Assert.IsTrue(test.Equals(d1, d2));
		Assert.IsFalse(test.Equals(d1, e));
		Assert.IsFalse(test.Equals(e, d1));

		Assert.AreEqual(0, test.GetHashCode(null));
		Assert.AreEqual(0, test.GetHashCode(d1));
		Assert.AreEqual(test.GetHashCode(a1), test.GetHashCode(a2));
	}
}
