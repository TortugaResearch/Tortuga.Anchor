using System.Runtime.Serialization;
using Tortuga.Anchor.Modeling.Internals;

namespace Tortuga.Anchor.Modeling
{

    /// <summary>
    /// ModelBase using the default property bag implementation.
    /// </summary>
    [DataContract(Namespace = "http://github.com/docevaad/Anchor")]
    public class SimpleModelBase : ModelBase<PropertyBag>
    {
        /// <summary>
        /// Creates a model using the default property bag implementation..
        /// </summary>

        protected SimpleModelBase()
        {

        }
    }


}
