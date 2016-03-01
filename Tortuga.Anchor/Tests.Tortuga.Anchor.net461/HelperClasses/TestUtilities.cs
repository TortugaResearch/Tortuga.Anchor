using System;

namespace Tests.HelperClasses
{
    public static class TestUtilities
    {
        public static void GCCycle()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }
    }

}
