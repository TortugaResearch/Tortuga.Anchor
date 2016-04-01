
using Tortuga.Anchor.Modeling;
using Tortuga.Dragnet;

#if MSTest
using Microsoft.VisualStudio.TestTools.UnitTesting;
#elif WINDOWS_UWP 
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#endif

namespace Tests.DataAnnotations
{



    [TestClass]
    public class CalculatedFieldAttributeTests
    {
        [TestMethod]
        public void CalculatedFieldAttribute_Test1()
        {
            using (var verify = new Verify())
            {
                string fields = "AAA, BBB,CCC";
                var result = new CalculatedFieldAttribute(fields);
                verify.AreEqual(fields, result.Sources, "the field list shouldn't be altered in any way");
                verify.ItemsAreEqual(new[] { "AAA", "BBB", "CCC" }, result.SourceProperties, "");

            }
        }

        [TestMethod]
        public void CalculatedFieldAttribute_Test2()
        {
            using (var verify = new Verify())
            {
                verify.ArgumentNullException("sources", () => new CalculatedFieldAttribute(null));

            }
        }

        [TestMethod]
        public void CalculatedFieldAttribute_Test3()
        {
            using (var verify = new Verify())
            {
                verify.ArgumentException("sources", () => new CalculatedFieldAttribute(null), "empty lists are not allowed");
            }
        }



    }
}
