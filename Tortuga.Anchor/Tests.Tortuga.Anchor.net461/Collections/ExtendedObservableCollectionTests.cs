using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using Tests.HelperClasses;
using Tests.Mocks;
using Tortuga.Anchor.Collections;
using Tortuga.Anchor.ComponentModel;
using Tortuga.Dragnet;

namespace Tests.Collections
{
    [TestClass]
    public class ExtendedObservableCollectionTests
    {
        [TestMethod]
        public void ExtendedObservableCollection_ConstructorTest1()
        {
            using (var verify = new Verify())
            {
                var result = new ObservableCollectionExtended<object>();
                result.Add(1);
                result.Add("Boat");

                var notifier = new Notifier();

                result.Add(notifier);

                ConstructorAssertions(verify, result);
            }
        }


        [TestMethod]
        public void ExtendedObservableCollection_ConstructorTest2()
        {
            using (var verify = new Verify())
            {
                var notifier = new Notifier();
                var temp = new object[] { notifier };


                var result = new ObservableCollectionExtended<object>(temp.ToList());

                ConstructorAssertions(verify, result);

            }
        }

        [TestMethod]
        public void ExtendedObservableCollection_ConstructorTest3()
        {
            using (var verify = new Verify())
            {
                var notifier = new Notifier();
                var temp = new[] { notifier };


                var result = new ObservableCollectionExtended<object>(temp.AsEnumerable());

                ConstructorAssertions(verify, result);

            }
        }


        [TestMethod]
        public void ExtendedObservableCollection_AddRange_Test1()
        {
            using (var verify = new Verify())
            {
                var notifier = new Notifier();
                var temp = new[] { notifier };


                var result = new ObservableCollectionExtended<object>();
                result.AddRange(temp);
                ConstructorAssertions(verify, result);
            }
        }

        [TestMethod]
        public void ExtendedObservableCollection_AddRange_Test2()
        {
            using (var verify = new Verify())
            {
                try
                {
                    var result = new ObservableCollectionExtended<object>();
                    result.AddRange(null);
                    verify.Fail("Expected an ArgumentNullException exception");
                }
                catch (ArgumentNullException ex)
                {
                    verify.AreEqual("list", ex.ParamName, "Parameter name is incorrect");
                }
            }
        }

        private static void ConstructorAssertions(Verify verify, ObservableCollectionExtended<object> result)
        {
            var itemAssert = new ItemPropertyChangedEventTest(verify, result);

            var itemAddedEventQueue = new Queue<Tuple<object, ItemEventArgs<object>>>();
            result.ItemAdded += (s, e) => itemAddedEventQueue.Enqueue(Tuple.Create(s, e));

            var itemRemovedEventQueue = new Queue<Tuple<object, ItemEventArgs<object>>>();
            result.ItemRemoved += (s, e) => itemRemovedEventQueue.Enqueue(Tuple.Create(s, e));

            Notifier notifier = new Notifier();
            Notifier notifier2 = new Notifier();




            var oldCount = result.Count;
            result.Clear();
            verify.AreEqual(oldCount, itemRemovedEventQueue.Count, "incorrect number of items reported as being removed");

            notifier.Age += 1;

            verify.AreEqual(0, result.Count, "The collection should be empty");
            itemAssert.ExpectCountEquals(0, "ItemPropertyChanged events were fired when the collection was supposed to be empty.");

            result.Add(notifier);
            result.Add(notifier2);
            verify.AreEqual(2, result.Count, "The new items were not added");
            verify.AreEqual(2, itemAddedEventQueue.Count, "The new item events didn't fire");
            CollectionAssert.AreEqual(result, result.ReadOnlyWrapper);

            notifier.Age += 1;
            itemAssert.ExpectEvent(notifier, "Age");

            itemRemovedEventQueue.Clear();
            result.RemoveAt(0);
            verify.AreEqual(1, itemRemovedEventQueue.Count, "the item wasn't removed");
            verify.AreSame(notifier2, result[0], "the wrong item was removed");
            CollectionAssert.AreEqual(result, result.ReadOnlyWrapper);

            notifier.Age += 1;
            itemAssert.ExpectNothing("Item was removed from collection prior to the event being fired.");


            CollectionAssert.AreEqual(result, result.ReadOnlyWrapper);
        }


        [TestMethod]
        public void ExtendedObservableCollection_RemoveItemFailTest1()
        {
            using (var verify = new Verify())
            {
                var result = new FooCollection();
                verify.ArgumentOutOfRangeException("index", -1, result.InvokeRemoveItem);
            }
        }

        [TestMethod]
        public void ExtendedObservableCollection_RemoveItemFailTest2()
        {
            using (var verify = new Verify())
            {
                var result = new FooCollection();
                verify.ArgumentOutOfRangeException("index", 0, result.InvokeRemoveItem);
            }
        }

        [TestMethod]
        public void ExtendedObservableCollection_SetItemFailTest1()
        {
            using (var verify = new Verify())
            {
                var result = new FooCollection();
                verify.ArgumentOutOfRangeException("index", -1, () => result.InvokeSetItem(-1, null));
            }
        }

        [TestMethod]
        public void ExtendedObservableCollection_SetItemFailTest2()
        {
            using (var verify = new Verify())
            {
                var result = new FooCollection();
                verify.ArgumentOutOfRangeException("index", 0, () => result.InvokeSetItem(0, null));
            }
        }

        [TestMethod]
        public void ExtendedObservableCollection_AddRemoveOverrideTest()
        {
            using (var verify = new Verify())
            {
                var result = new FooCollection(new[] { new Foo(), new Foo(), new Foo() });
                verify.AreEqual(3, result.AddCount, "count was incorrect");
            }
        }


        [TestMethod]
        public void ExtendedObservableCollection_MixedAddRemoveTest()
        {
            using (var verify = new Verify())
            {
                var fired = 0;
                var result = new FooCollection();
                PropertyChangedEventHandler eventHandler = (s, e) => fired += 1;


                result.PropertyChanged += eventHandler;
                result.Add(new Foo());
                verify.AreEqual(2, fired, "events were not fired"); //Count and Item[]

                ((INotifyPropertyChanged)result).PropertyChanged -= eventHandler;
                result.Add(new Foo());

                verify.AreEqual(2, fired, "handler wasn't removed"); //Count and Item[]

            }
        }

        [TestMethod]
        public void ExtendedObservableCollection_SerializationTest1()
        {
            using (var verify = new Verify())
            {
                var collection = new ObservableCollectionExtended<Foo>();
                collection.Add(new Foo());
                collection.Add(new Foo());
                collection.Add(new Foo());

                var stream = new System.IO.MemoryStream();
                var fooSerializer = new DataContractSerializer(typeof(ObservableCollectionExtended<Foo>));
                fooSerializer.WriteObject(stream, collection);
                stream.Position = 0;
                var newFoo = (ObservableCollectionExtended<Foo>)fooSerializer.ReadObject(stream);

                verify.AreEqual(collection.Count, newFoo.Count, "collection count was off");

            }
        }

        [TestMethod]
        public void ExtendedObservableCollection_SerializationTest2()
        {
            using (var verify = new Verify())
            {
                var collection = new FooCollection();
                collection.Add(new Foo());
                collection.Add(new Foo());
                collection.Add(new Foo());

                var stream = new System.IO.MemoryStream();
                var fooSerializer = new DataContractSerializer(typeof(FooCollection));
                fooSerializer.WriteObject(stream, collection);
                stream.Position = 0;
                var newFoo = (FooCollection)fooSerializer.ReadObject(stream);

                verify.AreEqual(collection.Count, newFoo.Count, "collection count was off");

            }
        }

    }
}

