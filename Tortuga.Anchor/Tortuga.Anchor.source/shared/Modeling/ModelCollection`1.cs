using System.Collections.Generic;
using System.Runtime.Serialization;
using Tortuga.Anchor.Modeling.Internals;

namespace Tortuga.Anchor.Modeling
{
    /// <summary>
    /// ModelCollection using the default property bag implementation.
    /// </summary>
    [DataContract(Namespace = "http://github.com/docevaad/Anchor")]
    public class ModelCollection<T> : AbstractModelCollection<T, PropertyBag>
    {
        /// <summary>
        /// Creates a model collection using the default property bag implementation..
        /// </summary>

        protected ModelCollection()
        {
        }

        /// <summary>
        /// Creates a model collection using the default property bag implementation..
        /// </summary>
        /// <param name="list">The list from which the elements are copied.</param>

        protected ModelCollection(List<T> list)
            : base(list)
        { }

        /// <summary>
        /// Creates a model collection using the default property bag implementation..
        /// </summary>
        /// <param name="collection">The collection from which the elements are copied.</param>

        protected ModelCollection(IEnumerable<T> collection)
            : base(collection)
        { }
    }
}