using System.Linq;
using Tortuga.Anchor.DataAnnotations;
using Tortuga.Dragnet;

#if MSTest
using Microsoft.VisualStudio.TestTools.UnitTesting;
#elif WINDOWS_UWP 
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#endif

namespace Tests.DataAnnotations
{
    [TestClass]
    public class ValidationResultCollectionTests
    {
        [TestMethod]
        public void ValidationResultCollection_Test1()
        {
            using (var verify = new Verify())
            {
                var collection = new ValidationResultCollection();
                var result = collection.Add("Test", "FirstName", "LastName");
                verify.AreEqual("Test", result.ErrorMessage, "Error message");
                verify.AreEqual("FirstName", result.MemberNames.ToList()[0], "member 0");
                verify.AreEqual("LastName", result.MemberNames.ToList()[1], "member 1");
            }
        }

    }
}
