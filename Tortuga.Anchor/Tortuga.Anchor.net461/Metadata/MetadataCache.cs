using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Xml.Linq;

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
                        yield return decompositionPrefix + item;
                }
                else if (property.CanWrite)
                {
                    yield return decompositionPrefix + property.MappedColumnName;
                }
            }
        }

        /// <summary>
        /// Populates the complex object.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The object being populated.</param>
        /// <param name="decompositionPrefix">The decomposition prefix.</param>
        /// <remarks>This honors the Column and Decompose attributes.</remarks>
        static public void PopulateComplexObject(IReadOnlyDictionary<string, object> source, object target, string decompositionPrefix)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source), $"{nameof(source)} is null.");
            if (target == null)
                throw new ArgumentNullException(nameof(target), $"{nameof(target)} is null.");

            foreach (var property in GetMetadata(target.GetType()).Properties)
            {
                if (property.CanWrite && source.ContainsKey(decompositionPrefix + property.MappedColumnName))
                {
                    var value = source[property.MappedColumnName];

                    //XML values come to us as strings
                    if (value is string)
                    {
                        if (property.PropertyType == typeof(XElement))
                            value = XElement.Parse((string)value);
                        else if (property.PropertyType == typeof(XDocument))
                            value = XDocument.Parse((string)value);
                    }

                    property.InvokeSet(target, value);
                }
                else if (property.Decompose)
                {
                    object child = null;

                    if (property.CanRead)
                        child = property.InvokeGet(target);

                    if (child == null && property.CanWrite && property.PropertyType.GetConstructor(new Type[0]) != null)
                    {
                        child = Activator.CreateInstance(property.PropertyType);
                        property.InvokeSet(target, child);
                    }

                    if (child != null)
                        PopulateComplexObject(source, child, decompositionPrefix + property.DecompositionPrefix);
                }
            }
        }

    }
}
