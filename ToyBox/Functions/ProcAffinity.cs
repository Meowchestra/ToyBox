

using System.Diagnostics;

namespace ToyBox.Functions;

internal static class ProcAffinity
{
    public static int GetCPUCores()
    {
        return Environment.ProcessorCount;
    }

    public static IntPtr GetAffinity()
    {
        return Process.GetCurrentProcess().ProcessorAffinity;
    }

    public static void SetAffinity(long AffinityMask)
    {
        if (AffinityMask == 0)
            return;

        Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)AffinityMask;
    }

}
