using System;
using System.Diagnostics;
using Tortuga.Dragnet;

#if MSTest
using Microsoft.VisualStudio.TestTools.UnitTesting;
#elif WINDOWS_UWP 
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#endif

namespace Tests.ChangeTracking
{
    [TestClass]
    public class ChangeTrackingTests
    {
        Customer CreateCustomer(Verify verify)
        {
            var result = new Customer();
            var order1 = new Order() { OrderDate = OrderDate1 };
            order1.Lines.Add(new OrderLine(1.95m, 10m));
            order1.Lines.Add(new OrderLine(12.95m, 1m));
            order1.Lines.Add(new OrderLine(19.95m, 2m));
            result.Orders.Add(order1);

            var order2 = new Order() { OrderDate = OrderDate2 };
            order2.Lines.Add(new OrderLine(2.50m, 5m));
            order2.Lines.Add(new OrderLine(1.75m, 12m));
            order2.Lines.Add(new OrderLine(.50m, 4m));
            result.Orders.Add(order2);

            result.AcceptChanges();

            if (result.IsChanged)
                verify.Inconclusive("Unable to setup test. New Customer object is indicating changes.");


            if (order1.IsChangedLocal)
                verify.Inconclusive("Unable to setup test. The order1 object is indicating local changes.");

            if (order1.IsChanged)
                verify.Inconclusive("Unable to setup test. The order1 object is indicating changes.");

            if (order2.IsChangedLocal)
                verify.Inconclusive("Unable to setup test. The order2 object is indicating local changes.");

            if (order2.IsChanged)
                verify.Inconclusive("Unable to setup test. The order2 object is indicating changes.");


            return result;
        }


        private readonly DateTime OrderDate1 = new DateTime(2010, 1, 1);
        private readonly DateTime OrderDate2 = new DateTime(2010, 2, 1);


        [TestMethod]
        public void LocalChangesWithRevert()
        {
            using (var verify = new Verify())
            {
                var customer = CreateCustomer(verify);

                //more setup 
                customer.FirstName = "Tom";
                customer.LastName = "Doe";
                customer.AcceptChanges();
                if (customer.IsChanged)
                    verify.Inconclusive("setup failed");

                var eventAssertCustomer = new PropertyChangedEventTest(verify, customer);

                customer.PropertyChanged += (s, e) => Debug.WriteLine(e.PropertyName);

                //local changes
                customer.FirstName = "Bob";
                eventAssertCustomer.ExpectEvent("FirstName");
                eventAssertCustomer.ExpectEvent("FullName");
                eventAssertCustomer.ExpectEvent("IsChangedLocal");
                eventAssertCustomer.ExpectEvent("IsChanged");
                eventAssertCustomer.ExpectNothing();
                verify.IsTrue(customer.IsChanged, "");
                verify.IsTrue(customer.IsChangedLocal, "");
                verify.AreEqual(1, customer.ChangedProperties().Count, "There should be one changed property");
                verify.AreEqual("FirstName", customer.ChangedProperties()[0], "The property should be FirstName");

                //this time IsChanged shouldn't be raised again
                customer.LastName = "Jones";
                eventAssertCustomer.ExpectEvent("LastName");
                eventAssertCustomer.ExpectEvent("FullName");
                eventAssertCustomer.ExpectNothing();
                verify.IsTrue(customer.IsChanged, "");
                verify.IsTrue(customer.IsChangedLocal, "");

                //reject changes
                customer.RejectChanges();
                verify.IsFalse(customer.IsChanged, "");
                verify.IsFalse(customer.IsChangedLocal, "");
                eventAssertCustomer.ExpectUnordered("FirstName", "LastName", "FullName", "FullName");
                eventAssertCustomer.ExpectEvent("IsChangedLocal");
                eventAssertCustomer.ExpectEvent("IsChanged");
                eventAssertCustomer.ExpectNothing();
                verify.AreEqual("Tom", customer.FirstName, "");
                verify.AreEqual("Doe", customer.LastName, "");
                verify.AreEqual(0, customer.ChangedProperties().Count, "There shouldn't be any changed properties");
            }
        }

        [TestMethod]
        public void ChildPropertyChangesWithCommit()
        {
            using (var verify = new Verify())
            {
                var customer = CreateCustomer(verify);

                //more setup 
                customer.FirstName = "Tom";
                customer.LastName = "Doe";
                customer.AcceptChanges();
                if (customer.IsChanged)
                    verify.Inconclusive("setup failed");

                var eventAssertCustomer = new PropertyChangedEventTest(verify, customer);

                customer.PropertyChanged += (s, e) => Debug.WriteLine(e.PropertyName);

                //local changes
                customer.FirstName = "Bob";
                eventAssertCustomer.ExpectEvent("FirstName");
                eventAssertCustomer.ExpectEvent("FullName");
                eventAssertCustomer.ExpectEvent("IsChangedLocal");
                eventAssertCustomer.ExpectEvent("IsChanged");
                eventAssertCustomer.ExpectNothing();
                verify.IsTrue(customer.IsChanged, "");
                verify.IsTrue(customer.IsChangedLocal, "");
                verify.AreEqual(1, customer.ChangedProperties().Count, "There should be one changed property");
                verify.AreEqual("FirstName", customer.ChangedProperties()[0], "The property should be FirstName");

                //this time IsChanged shouldn't be raised again
                customer.LastName = "Jones";
                eventAssertCustomer.ExpectEvent("LastName");
                eventAssertCustomer.ExpectEvent("FullName");
                eventAssertCustomer.ExpectNothing();
                verify.IsTrue(customer.IsChanged, "");
                verify.IsTrue(customer.IsChangedLocal, "");

                //accept changes
                customer.AcceptChanges();
                verify.IsFalse(customer.IsChanged, "");
                verify.IsFalse(customer.IsChangedLocal, "");
                eventAssertCustomer.ExpectEvent("IsChangedLocal");
                eventAssertCustomer.ExpectEvent("IsChanged");
                eventAssertCustomer.ExpectNothing();
                verify.AreEqual("Bob", customer.FirstName, "");
                verify.AreEqual("Jones", customer.LastName, "");
                verify.AreEqual(0, customer.ChangedProperties().Count, "There shouldn't be any changed properties");
            }
        }

        [TestMethod]
        public void ChildPropertyChangesWithRevert()
        {
            using (var verify = new Verify())
            {
                var customer = CreateCustomer(verify);
                var eventAssertCustomer = new PropertyChangedEventTest(verify, customer);

                //changes to a child in a collection
                var eventAssertOrder0 = new PropertyChangedEventTest(verify, customer.Orders[0]);
                var newOrderDate = new DateTime(2010, 5, 5);
                customer.Orders[0].OrderDate = newOrderDate;

                eventAssertOrder0.ExpectEvent("OrderDate");
                eventAssertOrder0.ExpectEvent("IsChangedLocal");
                eventAssertOrder0.ExpectEvent("IsChanged");
                eventAssertOrder0.ExpectNothing();

                //eventAssertCustomer.ExpectEvent("IsChanged");
                //eventAssertCustomer.ExpectNothing();

                //rollback
                customer.RejectChanges();
                verify.AreEqual(OrderDate1, customer.Orders[0].OrderDate, "Order date didn't roll back");

                verify.IsFalse(customer.IsChanged, "");
                eventAssertCustomer.ExpectEvent("IsChanged");

            }
        }


        [TestMethod]
        public void LocalChangesWithCommit()
        {
            using (var verify = new Verify())
            {
                var customer = CreateCustomer(verify);
                var eventAssertCustomer = new PropertyChangedEventTest(verify, customer);

                //local changes
                customer.FirstName = "Tom";
                eventAssertCustomer.ExpectEvent("FirstName");
                eventAssertCustomer.ExpectEvent("FullName");
                eventAssertCustomer.ExpectEvent("IsChangedLocal");
                eventAssertCustomer.ExpectEvent("IsChanged");
                eventAssertCustomer.ExpectNothing();
                verify.IsTrue(customer.IsChanged, "");
                verify.IsTrue(customer.IsChangedLocal, "");

                //accept changes
                customer.AcceptChanges();
                eventAssertCustomer.ExpectEvent("IsChangedLocal");
                eventAssertCustomer.ExpectEvent("IsChanged");
                eventAssertCustomer.ExpectNothing();
                verify.IsFalse(customer.IsChanged, "");
                verify.IsFalse(customer.IsChangedLocal, "");

                //more local changes
                customer.FirstName = "Bob";
                eventAssertCustomer.ExpectEvent("FirstName");
                eventAssertCustomer.ExpectEvent("FullName");
                eventAssertCustomer.ExpectEvent("IsChangedLocal");
                eventAssertCustomer.ExpectEvent("IsChanged");
                eventAssertCustomer.ExpectNothing();
                verify.IsTrue(customer.IsChanged, "");
                verify.IsTrue(customer.IsChangedLocal, "");

                //this time IsChanged shouldn't be raised again
                customer.LastName = "Jones";
                eventAssertCustomer.ExpectEvent("LastName");
                eventAssertCustomer.ExpectEvent("FullName");
                eventAssertCustomer.ExpectNothing();
                verify.IsTrue(customer.IsChanged, "");
                verify.IsTrue(customer.IsChangedLocal, "");

                //accept changes
                customer.AcceptChanges();
                verify.IsFalse(customer.IsChanged, "");
                verify.IsFalse(customer.IsChangedLocal, "");
                eventAssertCustomer.ExpectEvent("IsChangedLocal");
                eventAssertCustomer.ExpectEvent("IsChanged");
                eventAssertCustomer.ExpectNothing();
                verify.AreEqual("Bob", customer.FirstName, "");
                verify.AreEqual("Jones", customer.LastName, "");
            }
        }


    }
}
