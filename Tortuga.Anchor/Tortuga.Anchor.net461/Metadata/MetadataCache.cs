using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Tortuga.Anchor.Metadata
{
    /// <summary>
    /// Cache's metadata fetched via reflection.
    /// </summary>
    public static class MetadataCache
    {

        readonly static ConcurrentDictionary<Type, ClassMetadata> s_ModelInfo = new ConcurrentDictionary<Type, ClassMetadata>();

        /// <summary>
        /// Gets the metadata for the indicated type.
        /// </summary>
        /// <param name="type">The type of interest</param>
        /// <returns>A thread-safe copy of the class's metadata</returns>
        /// <remarks>Actually fetching the metadata requires taking a lock. Therefore it is advisable to locally cache the metadata as well.</remarks>
        public static ClassMetadata GetMetadata(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type), $"{nameof(type)} is null.");

            return s_ModelInfo.GetOrAdd(type, t => new ClassMetadata(t));
        }

        static internal IEnumerable<string> GetColumnsFor(this Type type, string decompositionPrefix)
        {
            var metadata = GetMetadata(type);
            return GetColumnsFor(metadata, decompositionPrefix);
        }

        static internal IEnumerable<string> GetColumnsFor(ClassMetadata metadata, string decompositionPrefix)
        {
            foreach (var property in metadata.Properties)
            {
                if (property.Decompose)
                {
                    foreach (var item in GetColumnsFor(property.PropertyType, decompositionPrefix + property.DecompositionPrefix))
                        yield return item;
                }
                else if (property.CanWrite && property.MappedColumnName != null)
                {
                    yield return decompositionPrefix + property.MappedColumnName;
                }
            }
        }

    }
}
