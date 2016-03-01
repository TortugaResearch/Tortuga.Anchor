using System.Collections.Generic;
using Tortuga.Anchor.Collections;

namespace Tests.Mocks
{
    public class FooCollection : ObservableCollectionExtended<Foo>
    {
        private int m_Boom;
        private int m_RemoveCount;
        private int m_AddCount;
        /// <summary>
        /// Initializes a new instance of the ImprovedObservableCollection class.
        /// </summary>
        public FooCollection()
        {

        }
        /// <summary>
        /// Initializes a new instance of the ImprovedObservableCollection class that contains elements copied from the specified list.
        /// </summary>
        /// <param name="list"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        public FooCollection(List<Foo> list)
            : base(list)
        {

        }
        /// <summary>
        /// Initializes a new instance of the ImprovedObservableCollection class that contains elements copied from the specified collection.
        /// </summary>
        /// <param name="collection"></param>
        public FooCollection(IEnumerable<Foo> collection)
            : base(collection)
        {

        }

        public int Boom
        {
            get { return m_Boom; }
            set
            {
                m_Boom = value;
                base.OnPropertyChanged("Boom");
            }
        }

        public void InvokeSetItem(int index, Foo item)
        {
            base.SetItem(index, item);
        }

        public void InvokeRemoveItem(int index)
        {
            base.RemoveItem(index);
        }

        protected override void OnItemAdded(Foo item)
        {
            base.OnItemAdded(item);
            m_AddCount += 1;
        }

        protected override void OnItemRemoved(Foo item)
        {
            base.OnItemRemoved(item);
            m_RemoveCount += 1;
        }
        public int AddCount
        {
            get { return m_AddCount; }
        }
        public int M_RemoveCount
        {
            get { return m_RemoveCount; }
        }
    }
}