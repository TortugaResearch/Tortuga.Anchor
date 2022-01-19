using System.ComponentModel;

namespace Tortuga.Anchor.Collections;

internal static class CommonProperties
{
	public static readonly PropertyChangedEventArgs CountProperty = new("Count");
	public static readonly PropertyChangedEventArgs ItemIndexedProperty = new("Item[]");
}
