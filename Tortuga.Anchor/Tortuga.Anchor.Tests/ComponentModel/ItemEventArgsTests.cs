using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tortuga.Anchor.ComponentModel;
using Tortuga.Dragnet;

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