using System;
using System.Collections.Generic;
using System.Reflection;

#if !Concurrent_Missing
using System.Collections.Concurrent;
#endif

namespace Tortuga.Anchor.Metadata
{
    /// <summary>
    /// Cache's metadata fetched via reflection.
    /// </summary>
    public static class MetadataCache
    {
#if !Concurrent_Missing && !TypeInfo_Is_Not_Type
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

            ClassMetadata result;

            if (s_ModelInfo.TryGetValue(type, out result))
                return result;

            //Cache the TypeInfo object
            if (type is TypeInfo)
                return s_ModelInfo.GetOrAdd(type, t => new ClassMetadata((TypeInfo)type));

            //Cache both the Type and TypeInfo object
            var typeInfo = type.GetTypeInfo();
            result = s_ModelInfo.GetOrAdd(typeInfo, t => new ClassMetadata(typeInfo));
            s_ModelInfo.TryAdd(type, result);
            return result;
        }
#elif !Concurrent_Missing 
        readonly static ConcurrentDictionary<Type, ClassMetadata> s_ModelInfo = new ConcurrentDictionary<Type, ClassMetadata>();
        readonly static ConcurrentDictionary<TypeInfo, ClassMetadata> s_ModelInfo2 = new ConcurrentDictionary<TypeInfo, ClassMetadata>();

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

            ClassMetadata result;

            if (s_ModelInfo.TryGetValue(type, out result))
                return result;

            //Cache both the Type and TypeInfo object
            var typeInfo = type.GetTypeInfo();
            result = s_ModelInfo2.GetOrAdd(typeInfo, t => new ClassMetadata(typeInfo));
            s_ModelInfo.TryAdd(type, result);
            return result;
        }

        /// <summary>
        /// Gets the metadata for the indicated type.
        /// </summary>
        /// <param name="typeInfo">The type of interest</param>
        /// <returns>A thread-safe copy of the class's metadata</returns>
        /// <remarks>Actually fetching the metadata requires taking a lock. Therefore it is advisable to locally cache the metadata as well.</remarks>
        public static ClassMetadata GetMetadata(TypeInfo typeInfo)
        {
            if (typeInfo == null)
                throw new ArgumentNullException(nameof(typeInfo), $"{nameof(typeInfo)} is null.");

            ClassMetadata result;

            if (s_ModelInfo2.TryGetValue(typeInfo, out result))
                return result;

            //Cache both the Type and TypeInfo object
            //var typeInfo = typeInfo.GetTypeInfo();
            result = s_ModelInfo2.GetOrAdd(typeInfo, t => new ClassMetadata(typeInfo));
            s_ModelInfo.TryAdd(typeInfo.AsType(), result);
            return result;
        }
#else
        readonly static Dictionary<object, ClassMetadata> s_ModelInfo = new Dictionary<object, ClassMetadata>();
        readonly static object s_SyncRoot = new object();

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

            lock (s_SyncRoot)
            {
                ClassMetadata result;
                if (s_ModelInfo.TryGetValue(type, out result))
                    return result;


                var typeInfo = type.GetTypeInfo();
                result = new ClassMetadata(typeInfo);

                //Cache both the Type and TypeInfo object
                s_ModelInfo.Add(type, result);
                s_ModelInfo.Add(typeInfo, result);
                return result;
            }
        }

        /// <summary>
        /// Gets the metadata for the indicated type.
        /// </summary>
        /// <param name="typeInfo">The type of interest</param>
        /// <returns>A thread-safe copy of the class's metadata</returns>
        /// <remarks>Actually fetching the metadata requires taking a lock. Therefore it is advisable to locally cache the metadata as well.</remarks>
        public static ClassMetadata GetMetadata(TypeInfo typeInfo)
        {
            if (typeInfo == null)
                throw new ArgumentNullException(nameof(typeInfo), $"{nameof(typeInfo)} is null.");

            lock (s_SyncRoot)
            {
                ClassMetadata result;
                if (s_ModelInfo.TryGetValue(typeInfo, out result))
                    return result;

                result = new ClassMetadata(typeInfo);

                //Cache the TypeInfo object
                s_ModelInfo.Add(typeInfo, result);
                return result;
            }
        }
#endif

#if !DataAnnotations_Missing
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
#endif

    }
}
