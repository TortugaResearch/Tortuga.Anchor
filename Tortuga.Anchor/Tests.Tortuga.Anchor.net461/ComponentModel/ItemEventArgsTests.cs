using Tortuga.Anchor.ComponentModel;
using Tortuga.Dragnet;

#if MSTest
using Microsoft.VisualStudio.TestTools.UnitTesting;
#elif WINDOWS_UWP 
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#endif

namespace Tests.ComponentModel
{

    [TestClass()]
    public class ItemEventArgsTests
    {

        [TestMethod()]
        public void ItemEventArgs_ItemTest()
        {
            using (var verify = new Verify())
            {
                var x = new ItemEventArgs<string>("xxx");
                verify.AreEqual("xxx", x.Item, "Item property was not set correctly");
            }
        }
    }
}
