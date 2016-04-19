using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

#if !DataAnnotations_Missing
#endif

namespace Tortuga.Anchor.Metadata
{


    /// <summary>
    /// Class ConstructorMetadata.
    /// </summary>
    public sealed class ConstructorMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructorMetadata"/> class.
        /// </summary>
        /// <param name="constructorInfo">The constructor information.</param>
        internal ConstructorMetadata(ConstructorInfo constructorInfo)
        {
            ConstructorInfo = constructorInfo;

            Signature = constructorInfo.GetParameters().Select(p => p.ParameterType).ToImmutableArray();
        }

        /// <summary>
        /// Gets the signature.
        /// </summary>
        /// <value>The signature.</value>
        public ImmutableArray<Type> Signature { get; }

        /// <summary>
        /// Gets the constructor information.
        /// </summary>
        /// <value>The constructor information.</value>
        public ConstructorInfo ConstructorInfo { get; }
    }

        
}
