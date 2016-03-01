
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tortuga.Anchor.Collections;
using Tortuga.Dragnet;

namespace Tests.Collections
{
    [TestClass]
    public class Dictionary3Tests
    {
        [TestMethod]
        public void BasicDictionaryTest()
        {
            using (var verify = new Verify())
            {
                var dictionary = new Dictionary<int, int, int>();
                dictionary[1, 1] = 10;
                dictionary.Add(1, 2, 3);

                verify.AreEqual(10, dictionary[1, 1], "Key 1,1 should have the value 10");
                verify.AreEqual(3, dictionary[1, 2], "Key 1,2 should have the value 3");
            }
        }
    }
}
