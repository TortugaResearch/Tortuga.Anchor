using System;
using System.Collections.Generic;
using System.Reflection;
using Tortuga.Anchor.Metadata;
using Tortuga.Anchor.Modeling.Internals;

#if MSTest
using Microsoft.VisualStudio.TestTools.UnitTesting;
#elif WINDOWS_UWP 
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#endif

namespace Tests.Modeling.Internals
{
    [TestClass]
    public class PropertyBagBaseTests
    {
        MockPropertyBag GetPropertyBagBase()
        {
            return (new MockModel()).GetPropertyBag();
        }

        [TestMethod]
        public void PropertyBagBase_RevalidatePropertyTest()
        {
            var bag = GetPropertyBagBase();
            try
            {
                bag.RevalidateProperty(null);
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("property", ex.ParamName);
            }
        }

        [TestMethod]
        public void PropertyBagBase_PropertyChangedTest()
        {
            var bag = GetPropertyBagBase();
            try
            {
                bag.PropertyChanged(null);
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("property", ex.ParamName);
            }
        }

        class MockModel : AbstractModelBase<MockPropertyBag>
        {

            internal MockPropertyBag GetPropertyBag()
            {
                return base.Properties;
            }
        }

        class MockPropertyBag : PropertyBagBase
        {
            public MockPropertyBag(object owner)
                : base(owner)
            {

            }

            public new void RevalidateProperty(PropertyMetadata property)
            {
                base.OnRevalidateProperty(property);
            }

            public new void PropertyChanged(PropertyMetadata property)
            {
                base.OnPropertyChanged(property);
            }

            Dictionary<string, object> m_Values = new Dictionary<string, object>();

            public override object GetValue(string propertyName)
            {
                if (m_Values.ContainsKey(propertyName))
                    return m_Values[propertyName];
                else
                    return Missing.Value;
            }

            public override bool IsDefined(string propertyName)
            {
                return m_Values.ContainsKey(propertyName);
            }

            public override bool Set(object value, PropertySetModes mode, string propertyName, out object oldValue)
            {
                oldValue = null;
                m_Values[propertyName] = value;
                return true;
            }
        }

    }
}
