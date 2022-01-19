using System.Collections;
using System.Collections.Concurrent;
using System.Reflection;
using Tortuga.Anchor.Modeling.Internals;

namespace Tortuga.Anchor.Metadata;

/// <summary>
/// Cache's metadata fetched via reflection.
/// </summary>
public static class MetadataCache
{
	readonly static ConcurrentDictionary<Type, ClassMetadata> s_ModelInfo = new();

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

		if (s_ModelInfo.TryGetValue(type, out ClassMetadata? result))
			return result;

		//Cache the TypeInfo object
		if (type is TypeInfo info)
			return s_ModelInfo.GetOrAdd(type, _ => new ClassMetadata(info));

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

	/// <summary>
	/// Clones the specified source.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="source">The source object to copy.</param>
	/// <param name="options">The clone options.</param>
	/// <param name="maxRecursion">The maximum recursion. Only applicable when CloneOptions.Recursive is used.</param>
	/// <returns>T.</returns>
	public static T Clone<T>(this T source, CloneOptions options, int? maxRecursion = null) where T : class, new()
	{
		object? CloneValue(object? value)
		{
			if (value == null)
				return null;

			if (options.HasFlag(CloneOptions.UseIClonable) && value is ICloneable cloneable)
				return cloneable.Clone();

			if (!options.HasFlag(CloneOptions.DeepClone)) //deep clone not requested
				return value;

			if (maxRecursion <= 0) //out of recursions
				return value;

			if (GetMetadata(value.GetType()).Constructors.HasDefaultConstructor)
			{
				var method = typeof(MetadataCache).GetMethod("Clone")!;
				var generic = method.MakeGenericMethod(value.GetType());
				return generic.Invoke(null, new object?[] { value, options, (maxRecursion - 1) });
			}

			return value;
		}


		var target = new T();

		if (options.HasFlag(CloneOptions.BypassProperties) && source is IUsesPropertyTracking tracked)
		{
			var sourceValues = tracked.Properties.GetInternalValues();
			var targetValues = sourceValues.Select(x => CloneValue(x)).ToArray();
			((IUsesPropertyTracking)target).Properties.SetInternalValues(targetValues);
		}
		else
		{
			foreach (var property in GetMetadata<T>().Properties)
			{
				if (property.CanRead && property.CanWrite)
				{
					var sourceValue = property.InvokeGet(source);
					var targetValue = CloneValue(sourceValue);
					property.InvokeSet(target, targetValue);
				}
			}

			if (source is IList collectionSource)
			{
				var targetCollection = (IList)target;

				foreach (var sourceValue in collectionSource)
				{
					var targetValue = CloneValue(sourceValue);
					targetCollection.Add(targetValue);
				}
			}
		}

		return target;
	}
}

