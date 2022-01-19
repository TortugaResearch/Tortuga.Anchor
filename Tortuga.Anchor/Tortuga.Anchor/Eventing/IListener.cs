using System;

namespace Tortuga.Anchor.Eventing
{
    /// <summary>
    /// Used for weak event callbacks
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IListener<in T> where T : EventArgs
    {
        /// <summary>
        /// This is the callback on the listener
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Invoke(object? sender, T e);
    }
}
