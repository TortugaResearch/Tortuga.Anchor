using System.Collections.Generic;
using Tortuga.Anchor.Modeling.Internals;

#if !Serialization_Missing
using System.Runtime.Serialization;
#endif

namespace Tortuga.Anchor.Modeling
{

    /// <summary>
    /// ModelCollection using the default property bag implementation.
    /// </summary>
#if !Serialization_Missing
    [DataContract(Namespace = "http://github.com/docevaad/Anchor")]
#endif
    public class ModelCollection<T> : AbstractModelCollection<T, PropertyBag>
    {
        /// <summary>
        /// Creates a model collection using the default property bag implementation..
        /// </summary>

        protected ModelCollection() { }

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
