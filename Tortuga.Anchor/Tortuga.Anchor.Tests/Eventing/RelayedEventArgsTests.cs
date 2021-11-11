using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tortuga.Anchor.Eventing;
using Tortuga.Dragnet;

namespace Tests.Eventing;

[TestClass]
public class RelayedEventArgsTests
{
	[TestMethod]
	public void RelayedEventArgs_Test1()
	{
		using (var verify = new Verify())
		{
			try
			{
				var e = RelayedEventArgs.Create(new object(), (EventArgs)null!);
				verify.Fail("Expected an ArgumentNullException");
			}
			catch (ArgumentNullException ex)
			{
				verify.AreEqual("eventArgs", ex.ParamName, "Parameter name is wrong");
			}
		}
	}
}
