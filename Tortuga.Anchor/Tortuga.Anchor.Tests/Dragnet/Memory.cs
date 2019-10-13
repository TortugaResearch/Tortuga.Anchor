using System;

namespace Tortuga.Dragnet
{
    /// <summary>
    /// This class is used for conducting memory based tests.
    /// </summary>
    public static class Memory
    {
        /// <summary>
        /// Cycles the Garbage Collector.
        /// </summary>
        public static void CycleGC()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }
    }
}
