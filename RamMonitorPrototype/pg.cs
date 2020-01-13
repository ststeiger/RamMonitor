
namespace RamMonitorPrototype
{


    class Bad
    {
        static void Test()
        {
            while (true)
            {
                long phav = PerformanceInfo.GetPhysicalAvailableMemoryInMiB();
                long tot = PerformanceInfo.GetTotalMemoryInMiB();
                decimal percentFree = ((decimal)phav / (decimal)tot) * 100;
                decimal percentOccupied = 100 - percentFree;
                System.Console.WriteLine("Available Physical Memory (MiB) " + phav.ToString());
                System.Console.WriteLine("Total Memory (MiB) " + tot.ToString());
                System.Console.WriteLine("Free (%) " + percentFree.ToString());
                System.Console.WriteLine("Occupied (%) " + percentOccupied.ToString());
                System.Console.ReadLine();
            }
        }
    }

    public static class PerformanceInfo
    {
        [System.Runtime.InteropServices.DllImport("psapi.dll", SetLastError = true)]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        public static extern bool GetPerformanceInfo([System.Runtime.InteropServices.Out] out PerformanceInformation PerformanceInformation, [System.Runtime.InteropServices.In] int Size);

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct PerformanceInformation
        {
            public int Size;
            public System.IntPtr CommitTotal;
            public System.IntPtr CommitLimit;
            public System.IntPtr CommitPeak;
            public System.IntPtr PhysicalTotal;
            public System.IntPtr PhysicalAvailable;
            public System.IntPtr SystemCache;
            public System.IntPtr KernelTotal;
            public System.IntPtr KernelPaged;
            public System.IntPtr KernelNonPaged;
            public System.IntPtr PageSize;
            public int HandlesCount;
            public int ProcessCount;
            public int ThreadCount;
        }

        public static long GetPhysicalAvailableMemoryInMiB()
        {
            PerformanceInformation pi = new PerformanceInformation();
            if (GetPerformanceInfo(out pi, System.Runtime.InteropServices.Marshal.SizeOf(pi)))
            {
                return System.Convert.ToInt64((pi.PhysicalAvailable.ToInt64() * pi.PageSize.ToInt64() / 1048576));
            }
            else
            {
                return -1;
            }

        }

        public static long GetTotalMemoryInMiB()
        {
            PerformanceInformation pi = new PerformanceInformation();
            if (GetPerformanceInfo(out pi, System.Runtime.InteropServices.Marshal.SizeOf(pi)))
            {
                return System.Convert.ToInt64((pi.PhysicalTotal.ToInt64() * pi.PageSize.ToInt64() / 1048576));
            }
            else
            {
                return -1;
            }

        }
    }
}