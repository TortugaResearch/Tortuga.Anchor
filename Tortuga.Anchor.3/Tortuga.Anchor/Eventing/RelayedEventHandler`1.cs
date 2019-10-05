using System;

namespace Tortuga.Anchor.Eventing
{
    /// <summary>
    /// This represents a relayed event. That is, an event that has both an immediate and and original source.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void RelayedEventHandler<T>(object sender, RelayedEventArgs<T> e) where T : EventArgs;
}
