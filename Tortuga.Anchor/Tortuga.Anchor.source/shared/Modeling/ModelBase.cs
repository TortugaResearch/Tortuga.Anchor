using Tortuga.Anchor.Modeling.Internals;

#if !Serialization_Missing
using System.Runtime.Serialization;
#endif

namespace Tortuga.Anchor.Modeling
{

    /// <summary>
    /// ModelBase using the default property bag implementation.
    /// </summary>
#if !Serialization_Missing
    [DataContract(Namespace = "http://github.com/docevaad/Anchor")]
#endif
    public class ModelBase : AbstractModelBase<PropertyBag>
    {
        /// <summary>
        /// Creates a model using the default property bag implementation..
        /// </summary>

        protected ModelBase()
        {

        }
    }


}
