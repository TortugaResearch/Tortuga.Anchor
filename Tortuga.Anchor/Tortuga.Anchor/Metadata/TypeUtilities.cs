using System.Reflection;
using System.Text;

namespace Tortuga.Anchor.Metadata;

static class TypeUtilities
{
	/// <summary>
	/// Gets the fully quantified name in C# format.
	/// </summary>
	/// <param name="type">The type.</param>
	public static string CSharpFullName(this Type type)
	{
		if (type == null)
			throw new ArgumentNullException(nameof(type), $"{nameof(type)} is null.");

		var result = new StringBuilder(type.ToString().Length);
		BuildFullName(type, null, result, "<", ", ", ">");

		return result.ToString();
	}

	/// <summary>
	/// Gets the fully quantified name in F# format.
	/// </summary>
	/// <param name="type">The type.</param>
	public static string FSharpFullName(this Type type)
	{
		if (type == null)
			throw new ArgumentNullException(nameof(type), $"{nameof(type)} is null.");

		var result = new StringBuilder(type.ToString().Length);
		BuildFullName(type, null, result, "<'", ", '", ">");

		return result.ToString();
	}

	/// <summary>
	/// Gets the fully quantified name in VB format.
	/// </summary>
	/// <param name="type">The type.</param>
	public static string VisualBasicFullName(this Type type)
	{
		if (type == null)
			throw new ArgumentNullException(nameof(type), $"{nameof(type)} is null.");

		var result = new StringBuilder(type.ToString().Length);
		BuildFullName(type, null, result, "(Of ", ", ", ")");

		return result.ToString();
	}

	static void BuildFullName(Type typeInfo, List<Type>? typeArgs, StringBuilder result, string genericOpen, string genericSeparator, string genericClose)
	{
		var localTypeParamCount = typeInfo.GetTypeInfo().GenericTypeParameters.Length;
		var localTypeArgCount = typeInfo.GetTypeInfo().GenericTypeArguments.Length;

		if (typeArgs == null)
			typeArgs = new List<Type>(typeInfo.GetTypeInfo().GenericTypeArguments);

		if (typeInfo.IsNested)
			BuildFullName(typeInfo.DeclaringType!, typeArgs, result, genericOpen, genericSeparator, genericClose);
		else
			result.Append(typeInfo.Namespace);

		result.Append('.');
		foreach (var c in typeInfo.Name)
		{
			if (c == '`') //we found a generic
				break;
			result.Append(c);
		}

		if (localTypeParamCount > 0)
		{
			result.Append(genericOpen);

			for (int i = 0; i < localTypeParamCount; i++)
			{
				if (i > 0)
					result.Append(genericSeparator);
				BuildFullName(typeArgs[i], null, result, genericOpen, genericSeparator, genericClose); //note that we are "eating" the typeArgs that we passed to us from the nested type.
			}
			typeArgs.RemoveRange(0, localTypeParamCount); //remove the used args

			result.Append(genericClose);
		}
		else if (localTypeArgCount > 0 && typeArgs.Count > 0)
		{
			result.Append(genericOpen);

			for (int i = 0; i < Math.Min(localTypeArgCount, typeArgs.Count); i++)
			{
				if (i > 0)
					result.Append(genericSeparator);
				BuildFullName(typeArgs[i], null, result, genericOpen, genericSeparator, genericClose);
			}
			result.Append(genericClose);
		}
	}
}
