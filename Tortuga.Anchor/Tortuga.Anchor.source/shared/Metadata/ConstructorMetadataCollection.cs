using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

#if !DataAnnotations_Missing
#endif

namespace Tortuga.Anchor.Metadata
{

    /// <summary>
    /// Class ConstructorMetadataCollection.
    /// </summary>
    /// <seealso cref="ICollection{PropertyMetadata}" />
    public sealed class ConstructorMetadataCollection : ICollection<ConstructorMetadata>
    {
        private readonly ImmutableArray<ConstructorMetadata> m_Constructors;

        internal ConstructorMetadataCollection(IEnumerable<ConstructorInfo> constructors)
        {
            m_Constructors = ImmutableArray.CreateRange(constructors.Where(c => c.IsPublic).Select(c => new ConstructorMetadata(c)));
            HasDefaultConstructor = m_Constructors.Any(c => c.Signature.Length == 0);
        }

        /// <summary>
        /// Gets a value indicating whether this instance has a default constructor.
        /// </summary>
        /// <value><c>true</c> if this instance has a default constructor; otherwise, <c>false</c>.</value>
        public bool HasDefaultConstructor { get; }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get { return m_Constructors.Length; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
        /// </summary>
        /// <value><c>true</c> if this instance is read only; otherwise, <c>false</c>.</value>
        public bool IsReadOnly
        {
            get { return true; }
        }

        void ICollection<ConstructorMetadata>.Add(ConstructorMetadata item)
        {
            throw new NotSupportedException();
        }

        void ICollection<ConstructorMetadata>.Clear()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1" /> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <returns>true if <paramref name="item" /> is found in the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false.</returns>
        public bool Contains(ConstructorMetadata item)
        {
            return m_Constructors.Contains(item);
        }

        /// <summary>
        /// Determines whether [contains] [the specified signature].
        /// </summary>
        /// <param name="signature">The signature.</param>
        /// <returns><c>true</c> if [contains] [the specified signature]; otherwise, <c>false</c>.</returns>
        public bool Contains(IReadOnlyList<Type> signature)
        {
            return Find(signature) != null;
        }

        /// <summary>
        /// Determines whether [contains] [the specified signature].
        /// </summary>
        /// <param name="signature">The signature.</param>
        /// <returns><c>true</c> if [contains] [the specified signature]; otherwise, <c>false</c>.</returns>
        public bool Contains(params Type[] signature)
        {
            return Find(signature) != null;
        }

        /// <summary>
        /// Attempts to find the specified constructor. Returns null if no match was found.
        /// </summary>
        /// <param name="signature">The signature.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public ConstructorMetadata Find(IReadOnlyList<Type> signature)
        {
            if (signature == null)
                throw new ArgumentNullException(nameof(signature), $"{nameof(signature)} is null");

            foreach (var item in m_Constructors)
            {
                if (item.Signature.Length != signature.Count)
                    continue;

                var isMatch = true;
                for (var i = 0; i < item.Signature.Length; i++)
                {
                    if (item.Signature[i] != signature[i])
                    {
                        isMatch = false;
                        break;
                    }
                }
                if (isMatch)
                    return item;
            }
            return null;
        }

        /// <summary>
        /// Attempts to find the specified constructor. Returns null if no match was found.
        /// </summary>
        /// <param name="signature">The signature.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public ConstructorMetadata Find(params Type[] signature)
        {
            if (signature == null)
                throw new ArgumentNullException(nameof(signature), $"{nameof(signature)} is null");

            foreach (var item in m_Constructors)
            {
                if (item.Signature.Length != signature.Length)
                    continue;

                var isMatch = true;
                for (var i = 0; i < item.Signature.Length; i++)
                {
                    if (item.Signature[i] != signature[i])
                    {
                        isMatch = false;
                        break;
                    }
                }
                if (isMatch)
                    return item;
            }
            return null;
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1" /> to an <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        public void CopyTo(ConstructorMetadata[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array), $"{nameof(array)} is null");

            m_Constructors.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
        public IEnumerator<ConstructorMetadata> GetEnumerator()
        {
            return ((ICollection<ConstructorMetadata>)m_Constructors).GetEnumerator();
        }

        bool ICollection<ConstructorMetadata>.Remove(ConstructorMetadata item)
        {
            throw new NotSupportedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((ICollection<ConstructorMetadata>)m_Constructors).GetEnumerator();
        }
    }
}
