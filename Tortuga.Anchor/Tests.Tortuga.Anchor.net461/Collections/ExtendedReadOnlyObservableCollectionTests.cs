


using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using Tests.HelperClasses;
using Tests.Mocks;
using Tortuga.Anchor.Collections;
using Tortuga.Dragnet;

namespace Tests.Collections
{

    [TestClass()]
    public class ExtendedReadOnlyObservableCollectionTests
    {

        [TestMethod()]
        public void ExtendedReadOnlyObservableCollection_MemoryTest()
        {
            using (var verify = new Verify())
            {
                var source = new ObservableCollectionExtended<int>();
                var target = new ReadOnlyObservableCollectionExtended<int>(source);
                var wr = new WeakReference(target);

                var sourceEvents = new Queue<NotifyCollectionChangedEventArgs>();
                var targetEvents = new Queue<NotifyCollectionChangedEventArgs>();

                var sourceEvents2 = new Queue<PropertyChangedEventArgs>();
                var targetEvents2 = new Queue<PropertyChangedEventArgs>();

                source.CollectionChanged += (s, e) => sourceEvents.Enqueue(e);
                target.CollectionChanged += (s, e) => targetEvents.Enqueue(e);

                source.PropertyChanged += (s, e) => sourceEvents2.Enqueue(e);
                target.PropertyChanged += (s, e) => targetEvents2.Enqueue(e);

                source.Add(1);
                source.Add(2);
                source.Add(3);

                verify.AreEqual(3, sourceEvents.Count, "NotifyCollectionChangedEventArgs in source was wrong");
                verify.AreEqual(3, targetEvents.Count, "NotifyCollectionChangedEventArgs in target was wrong");
                verify.AreEqual(6, sourceEvents2.Count, "PropertyChangedEventArgs in source was wrong.  There should be 2 per add.");
                verify.AreEqual(6, targetEvents2.Count, "PropertyChangedEventArgs in target was wrong. There should be 2 per add.");

                verify.ItemsAreEqual(source, target, "Events fired by the collection and read-only wrapper should be the same");

                target = null;

               
            }
        }

        [TestMethod]
        public void ExtendedReadOnlyObservableCollection_Constructor_Test1()
        {
            using (var verify = new Verify())
            {
                try
                {
                    ObservableCollectionExtended<int> list = null;
                    var result = new ReadOnlyObservableCollectionExtended<int>(list);
                    verify.Fail("Expected an ArgumentNullException");
                }
                catch (ArgumentNullException ex)
                {
                    verify.AreEqual("list", ex.ParamName, "Parameter name is wrong");
                }
            }
        }

        [TestMethod]
        public void ExtendedReadOnlyObservableCollection_OnSourcePropertyChanged()
        {
            using (var verify = new Verify())
            {
                var list = new FooCollection();
                var result = new ReadOnlyFooCollection(list);
                var propertyAssert = new PropertyChangedEventTest(verify, result);

                list.Boom = 10;

                propertyAssert.ExpectEvent("Boom");
                verify.AreEqual(10, result.Boom, "Boom");
            }
        }

        [TestMethod]
        public void ExtendedReadOnlyObservableCollection_OnSourcePropertyChanged2()
        {
            using (var verify = new Verify())
            {
                var list = new NotifierCollection();
                var result = new ReadOnlyObservableCollectionExtended<Notifier>(list);
                var propertyAssert = new PropertyChangedEventTest(verify, result);

                list.RaisePropertyName("");

                propertyAssert.ExpectEvent("");
            }
        }

        [TestMethod]
        public void ExtendedReadOnlyObservableCollection_OnPropertyChanged2()
        {
            using (var verify = new Verify())
            {
                var list = new NotifierCollection();
                var result = new ReadOnlyNotifierCollection(list);
                var propertyAssert = new PropertyChangedEventTest(verify, result);

                result.RaisePropertyName("");

                propertyAssert.ExpectEvent("");
            }
        }

        [TestMethod]
        public void ExtendedReadOnlyObservableCollection_OnItemPropertyChanged()
        {
            using (var verify = new Verify())
            {
                var list = new ObservableCollectionExtended<Notifier>();
                var result = new ReadOnlyObservableCollectionExtended<Notifier>(list);
                var itemPropertyAssert = new ItemPropertyChangedEventTest(verify, result);

                var notifier = new Notifier();
                list.Add(notifier);
                notifier.Name = "Frank";

                itemPropertyAssert.ExpectEvent(notifier, "Name");
            }
        }







        

        


    }
}


