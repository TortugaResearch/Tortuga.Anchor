using System.ComponentModel.DataAnnotations;

namespace Tortuga.Anchor.Modeling
{
    static class ValidationContextHelper
    {
        public static ValidationContext Create(object instance)
        {
#if NETFX_CORE
    			return new ValidationContext(instance);
#else
            return new ValidationContext(instance, null, null);
#endif
        }
    }
}