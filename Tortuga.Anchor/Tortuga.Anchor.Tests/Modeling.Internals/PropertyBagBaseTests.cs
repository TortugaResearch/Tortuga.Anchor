using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Tortuga.Anchor.Metadata;
using Tortuga.Anchor.Modeling.Internals;

namespace Tests.Modeling.Internals
{
    [TestClass]
    public class PropertyBagBaseTests
    {
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

        MockPropertyBag GetPropertyBagBase()
        {
            return (new MockModel()).GetPropertyBag();
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
            Dictionary<string, object> m_Values = new Dictionary<string, object>();

            public MockPropertyBag(object owner)
                            : base(owner)
            {
            }

            public override object GetValue(string propertyName)
            {
                if (m_Values.ContainsKey(propertyName))
                    return m_Values[propertyName];
                else
                    return NotSet.Value;
            }

            public override bool IsDefined(string propertyName)
            {
                return m_Values.ContainsKey(propertyName);
            }

            public new void PropertyChanged(PropertyMetadata property)
            {
                base.OnPropertyChanged(property);
            }

            public new void RevalidateProperty(PropertyMetadata property)
            {
                base.OnRevalidateProperty(property);
            }

            public override bool Set(object? value, PropertySetModes mode, string propertyName, out object oldValue)
            {
                oldValue = null;
                m_Values[propertyName] = value;
                return true;
            }
        }
    }
}
