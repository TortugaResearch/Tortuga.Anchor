using System.ComponentModel;

namespace Tortuga.Anchor.Collections
{
    internal static class CommonProperties
    {
        public readonly static PropertyChangedEventArgs CountProperty = new PropertyChangedEventArgs("Count");
        public readonly static PropertyChangedEventArgs ItemIndexedProperty = new PropertyChangedEventArgs("Item[]");
    }
}
