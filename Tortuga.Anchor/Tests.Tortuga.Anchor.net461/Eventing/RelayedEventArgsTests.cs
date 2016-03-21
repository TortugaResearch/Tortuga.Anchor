using System;
using Tortuga.Anchor.Eventing;
using Tortuga.Dragnet;

#if MSTest
using Microsoft.VisualStudio.TestTools.UnitTesting;
#elif WINDOWS_UWP 
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#endif

namespace Tests.Eventing
{
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
                    var e = RelayedEventArgs.Create(new object(), (EventArgs)null);
                    verify.Fail("Expected an ArgumentNullException");
                }
                catch (ArgumentNullException ex)
                {
                    verify.AreEqual("eventArgs", ex.ParamName, "Parameter name is wrong");
                }
            }
        }
    }
}
