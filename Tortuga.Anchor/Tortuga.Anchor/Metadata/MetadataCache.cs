using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

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
        /// <typeparam name="T">The type of interest</typeparam>
        /// <returns>A thread-safe copy of the class's metadata</returns>
        /// <remarks>Actually fetching the metadata may require taking a lock. Therefore it is advisable to locally cache the metadata as well.</remarks>
        public static ClassMetadata GetMetadata<T>() => GetMetadata(typeof(T));

        /// <summary>
        /// Gets the metadata for the indicated type.
        /// </summary>
        /// <param name="type">The type of interest</param>
        /// <returns>A thread-safe copy of the class's metadata</returns>
        /// <remarks>Actually fetching the metadata may require taking a lock. Therefore it is advisable to locally cache the metadata as well.</remarks>
        public static ClassMetadata GetMetadata(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type), $"{nameof(type)} is null.");

            if (s_ModelInfo.TryGetValue(type, out ClassMetadata result))
                return result;

            //Cache the TypeInfo object
            if (type is TypeInfo)
                return s_ModelInfo.GetOrAdd(type, _ => new ClassMetadata((TypeInfo)type));

            //Cache both the Type and TypeInfo object
            var typeInfo = type.GetTypeInfo();
            result = s_ModelInfo.GetOrAdd(typeInfo, _ => new ClassMetadata(typeInfo));
            s_ModelInfo.TryAdd(type, result);
            return result;
        }

        static internal IEnumerable<string> GetColumnsFor(this Type type, string decompositionPrefix)
        {
            var metadata = GetMetadata(type);
            return GetColumnsFor(metadata, decompositionPrefix);
        }

        static internal IEnumerable<string> GetColumnsFor(ClassMetadata metadata, string? decompositionPrefix)
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
