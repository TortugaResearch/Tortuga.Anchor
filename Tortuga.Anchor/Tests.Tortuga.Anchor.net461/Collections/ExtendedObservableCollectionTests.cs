

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using Tests.HelperClasses;
using Tests.Mocks;
using Tortuga.Anchor.Collections;
using Tortuga.Anchor.ComponentModel;
using Tortuga.Anchor.Eventing;
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

                var weakNotifier = new WeakNotifier();
                var notifier = new Notifier();

                result.Add(weakNotifier);
                result.Add(notifier);

                ConstructorAssertions(verify, result, weakNotifier, notifier);
            }
        }


        [TestMethod]
        public void ExtendedObservableCollection_ConstructorTest2()
        {
            using (var verify = new Verify())
            {
                var weakNotifier = new WeakNotifier();
                var notifier = new Notifier();
                var temp = new object[] { notifier, weakNotifier };


                var result = new ObservableCollectionExtended<object>(temp.ToList());

                ConstructorAssertions(verify, result, weakNotifier, notifier);
            }
        }

        [TestMethod]
        public void ExtendedObservableCollection_ConstructorTest3()
        {
            using (var verify = new Verify())
            {
                var weakNotifier = new WeakNotifier();
                var notifier = new Notifier();
                var temp = new[] { notifier, weakNotifier };


                var result = new ObservableCollectionExtended<object>(temp.AsEnumerable());

                ConstructorAssertions(verify, result, weakNotifier, notifier);
            }
        }


        [TestMethod]
        public void ExtendedObservableCollection_AddRange_Test1()
        {
            using (var verify = new Verify())
            {
                var weakNotifier = new WeakNotifier();
                var notifier = new Notifier();
                var temp = new[] { notifier, weakNotifier };


                var result = new ObservableCollectionExtended<object>();
                result.AddRange(temp);
                ConstructorAssertions(verify, result, weakNotifier, notifier);
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

        private static void ConstructorAssertions(Verify verify, ObservableCollectionExtended<object> result, WeakNotifier weakNotifier, Notifier notifier)
        {
            var itemAssert = new ItemPropertyChangedEventTest(verify, result);

            var itemAddedEventQueue = new Queue<Tuple<object, ItemEventArgs<object>>>();
            result.ItemAdded += (s, e) => itemAddedEventQueue.Enqueue(Tuple.Create(s, e));

            var itemRemovedEventQueue = new Queue<Tuple<object, ItemEventArgs<object>>>();
            result.ItemRemoved += (s, e) => itemRemovedEventQueue.Enqueue(Tuple.Create(s, e));


            weakNotifier.Age += 1;
            itemAssert.ExpectEvent(weakNotifier, "Age");

            notifier.Age += 1;
            itemAssert.ExpectEvent(notifier, "Age");


            var oldCount = result.Count;
            result.Clear();
            verify.AreEqual(oldCount, itemRemovedEventQueue.Count, "incorrect number of items reported as being removed");

            weakNotifier.Age += 1;
            notifier.Age += 1;

            verify.AreEqual(0, result.Count, "The collection should be empty");
            itemAssert.ExpectCountEquals(0, "ItemPropertyChanged events were fired when the collection was supposed to be empty.");

            result.Add(weakNotifier);
            result.Add(notifier);
            verify.AreEqual(2, result.Count, "The new items were not added");
            verify.AreEqual(2, itemAddedEventQueue.Count, "The new item events didn't fire");
            CollectionAssert.AreEqual(result, result.ReadOnlyWrapper);

            itemRemovedEventQueue.Clear();
            result.RemoveAt(0);
            verify.AreEqual(1, itemRemovedEventQueue.Count, "the item wasn't removed");
            verify.AreSame(notifier, result[0], "the wrong item was removed");
            CollectionAssert.AreEqual(result, result.ReadOnlyWrapper);

            itemAddedEventQueue.Clear();
            itemRemovedEventQueue.Clear();
            result[0] = weakNotifier;
            verify.AreSame(weakNotifier, result[0], "the item wasn't updated");
            verify.AreEqual(1, itemAddedEventQueue.Count, "the add event for replacing an item didn't fire");
            verify.AreEqual(1, itemRemovedEventQueue.Count, "the remove event for replacing an item didn't fire");
            CollectionAssert.AreEqual(result, result.ReadOnlyWrapper);
        }



        [TestMethod]
        public void ExtendedObservableCollection_MemoryTest1()
        {
            using (var verify = new Verify())
            {
                Func<WeakReference> builder = () => {

                    var result = new ObservableCollectionExtended<object>();
                    var weakNotifier = new WeakNotifier();
                    result.Add(weakNotifier);

                    return new WeakReference(result);
                };


                var wr = builder();

                Memory.CycleGC();

                verify.IsFalse(wr.IsAlive, "An item in the collection is preventing the collection from being collected");
            }
        }

        [TestMethod]
        public void ExtendedObservableCollection_PropertyChangedListenerTest()
        {
            using (var verify = new Verify())
            {
                var result = new FooCollection();
                var propertyChangedEventQueue = new Queue<Tuple<object, PropertyChangedEventArgs>>();
                var propertyChangedListener = new Listener<PropertyChangedEventArgs>((s, e) => propertyChangedEventQueue.Enqueue(Tuple.Create(s, e)));
                result.AddHandler(propertyChangedListener);

                result.Boom += 1;
                verify.AreEqual(1, propertyChangedEventQueue.Count, "event didn't fire");
                var event1 = propertyChangedEventQueue.Dequeue();
                verify.AreSame(result, event1.Item1, "event has the wrong item");
                verify.AreEqual("Boom", event1.Item2.PropertyName, "event as the wrong parameter name");

                result.RemoveHandler(propertyChangedListener);
                result.Boom += 1;
                verify.AreEqual(0, propertyChangedEventQueue.Count, "event handler wasn't removed");
            }
        }

        [TestMethod]
        public void ExtendedObservableCollection_ItemPropertyChangedListenerTest1()
        {
            using (var verify = new Verify())
            {
                var result = new FooCollection();
                var itemPropertyChangedEventQueue = new Queue<Tuple<object, RelayedEventArgs<PropertyChangedEventArgs>>>();
                var itemPropertyChangedListener = new Listener<RelayedEventArgs<PropertyChangedEventArgs>>((s, e) => itemPropertyChangedEventQueue.Enqueue(Tuple.Create(s, e)));

                var mutator = new Foo();
                result.Add(mutator);

                result.AddHandler(itemPropertyChangedListener);
                mutator.FooBar = "AAA";

                verify.AreEqual(1, itemPropertyChangedEventQueue.Count, "event didn't fire");
                var event1 = itemPropertyChangedEventQueue.Dequeue();
                verify.AreSame(result, event1.Item1, "event has the wrong item");
                verify.AreEqual<object>(mutator, event1.Item2.OriginalSender, "event has the wrong original sender");
                verify.AreEqual("FooBar", event1.Item2.EventArgs.PropertyName, "event as the wrong parameter name");

                result.RemoveHandler(itemPropertyChangedListener);

                mutator.FooBar = "BBB";

                verify.AreEqual(0, itemPropertyChangedEventQueue.Count, "event handler wasn't removed");
            }
        }

        [TestMethod]
        public void ExtendedObservableCollection_ItemPropertyChangedListenerTest2()
        {
            using (var verify = new Verify())
            {
                var result = new FooCollection();
                var itemPropertyChangedEventQueue = new Queue<Tuple<object, RelayedEventArgs<PropertyChangedEventArgs>>>();
                var itemPropertyChangedListener = new Listener<RelayedEventArgs<PropertyChangedEventArgs>>((s, e) => itemPropertyChangedEventQueue.Enqueue(Tuple.Create(s, e)));

                var mutator = new Foo();
                result.Add(mutator);

                result.AddHandler(itemPropertyChangedListener);
                mutator.FooBar = "AAA";

                verify.AreEqual(1, itemPropertyChangedEventQueue.Count, "event didn't fire");
                var event1 = itemPropertyChangedEventQueue.Dequeue();
                verify.AreSame(result, event1.Item1, "event has the wrong item");
                verify.AreEqual<object>(mutator, event1.Item2.OriginalSender, "event has the wrong original sender");
                verify.AreEqual("FooBar", event1.Item2.EventArgs.PropertyName, "event as the wrong parameter name");

                result.Remove(mutator);

                mutator.FooBar = "BBB";

                verify.AreEqual(0, itemPropertyChangedEventQueue.Count, "event handler wasn't removed");
            }
        }


        [TestMethod]
        public void ExtendedObservableCollection_CollectionChangedListenerTest()
        {
            using (var verify = new Verify())
            {
                var result = new FooCollection();
                var collectionChangedEventQueue = new Queue<Tuple<object, NotifyCollectionChangedEventArgs>>();
                var collectionChangedListener = new Listener<NotifyCollectionChangedEventArgs>((s, e) => collectionChangedEventQueue.Enqueue(Tuple.Create(s, e)));
                result.AddHandler(collectionChangedListener);

                result.Add(new Foo());
                verify.AreEqual(1, collectionChangedEventQueue.Count, "event didn't fire");
                var event1 = collectionChangedEventQueue.Dequeue();
                verify.AreSame(result, event1.Item1, "event has the wrong item");
                verify.AreEqual(NotifyCollectionChangedAction.Add, event1.Item2.Action, "event as the wrong action");

                result.RemoveHandler(collectionChangedListener);
                result.Add(new Foo());
                verify.AreEqual(0, collectionChangedEventQueue.Count, "event handler wasn't removed");
            }
        }

        /// <summary>
        /// This exercises the code path where we remove a listener without first adding one.
        /// </summary>
        [TestMethod]
        public void ExtendedObservableCollection_RemoveHandlerTest()
        {
            using (var verify = new Verify())
            {
                var result = new FooCollection();
                var collectionChangedEventQueue = new Queue<Tuple<object, NotifyCollectionChangedEventArgs>>();
                var collectionChangedListener = new Listener<NotifyCollectionChangedEventArgs>((s, e) => collectionChangedEventQueue.Enqueue(Tuple.Create(s, e)));
                result.RemoveHandler(collectionChangedListener);

                var propertyChangedEventQueue = new Queue<Tuple<object, PropertyChangedEventArgs>>();
                var propertyChangedListener = new Listener<PropertyChangedEventArgs>((s, e) => propertyChangedEventQueue.Enqueue(Tuple.Create(s, e)));
                result.RemoveHandler(propertyChangedListener);


                var itemPropertyChangedEventQueue = new Queue<Tuple<object, RelayedEventArgs<PropertyChangedEventArgs>>>();
                var itemPropertyChangedListener = new Listener<RelayedEventArgs<PropertyChangedEventArgs>>((s, e) => itemPropertyChangedEventQueue.Enqueue(Tuple.Create(s, e)));
                result.RemoveHandler(itemPropertyChangedListener);
            }
        }

        /// <summary>
        /// This exercises the code path where we remove a listener twice
        /// </summary>
        [TestMethod]
        public void ExtendedObservableDictionary_RemoveHandlerTest2()
        {
            using (var verify = new Verify())
            {
                var result = new FooCollection();
                var collectionChangedEventQueue = new Queue<Tuple<object, NotifyCollectionChangedEventArgs>>();
                var collectionChangedListener = new Listener<NotifyCollectionChangedEventArgs>((s, e) => collectionChangedEventQueue.Enqueue(Tuple.Create(s, e)));
                result.AddHandler(collectionChangedListener);
                result.RemoveHandler(collectionChangedListener);
                result.RemoveHandler(collectionChangedListener);

                var propertyChangedEventQueue = new Queue<Tuple<object, PropertyChangedEventArgs>>();
                var propertyChangedListener = new Listener<PropertyChangedEventArgs>((s, e) => propertyChangedEventQueue.Enqueue(Tuple.Create(s, e)));
                result.AddHandler(propertyChangedListener);
                result.RemoveHandler(propertyChangedListener);
                result.RemoveHandler(propertyChangedListener);


                var itemPropertyChangedEventQueue = new Queue<Tuple<object, RelayedEventArgs<PropertyChangedEventArgs>>>();
                var itemPropertyChangedListener = new Listener<RelayedEventArgs<PropertyChangedEventArgs>>((s, e) => itemPropertyChangedEventQueue.Enqueue(Tuple.Create(s, e)));
                result.AddHandler(itemPropertyChangedListener);
                result.RemoveHandler(itemPropertyChangedListener);
                result.RemoveHandler(itemPropertyChangedListener);
            }
        }

        [TestMethod]
        public void ExtendedObservableCollection_AddNullHandlerTest()
        {
            using (var verify = new Verify())
            {
                var result = new FooCollection();

                verify.ArgumentNullException("eventHandler", () => result.AddHandler((IListener<NotifyCollectionChangedEventArgs>)null));
                verify.ArgumentNullException("eventHandler", () => result.AddHandler((IListener<PropertyChangedEventArgs>)null));
                verify.ArgumentNullException("eventHandler", () => result.AddHandler((IListener<RelayedEventArgs<PropertyChangedEventArgs>>)null));
            }
        }

        [TestMethod]
        public void ExtendedObservableCollection_RemoveNullHandlerTest()
        {
            using (var verify = new Verify())
            {
                var result = new FooCollection();

                verify.ArgumentNullException("eventHandler", () => result.RemoveHandler((IListener<NotifyCollectionChangedEventArgs>)null));
                verify.ArgumentNullException("eventHandler", () => result.RemoveHandler((IListener<PropertyChangedEventArgs>)null));
                verify.ArgumentNullException("eventHandler", () => result.RemoveHandler((IListener<RelayedEventArgs<PropertyChangedEventArgs>>)null));
            }
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

