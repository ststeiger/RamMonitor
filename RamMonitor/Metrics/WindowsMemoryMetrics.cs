
namespace RamMonitor
{


    public class WindowsMemoryMetrics
        : GlobalMemoryMetrics
    {
        public ulong TotalPhys;
        public ulong AvailPhys;
        public ulong TotalPageFile;
        public ulong AvailPageFile;
        public ulong TotalVirtual;
        public ulong AvailVirtual;
        public ulong AvailExtendedVirtual;

        
        public override ulong TotalRAM
        {
            get
            {
                return this.TotalPhys;
            }
        }

        public override ulong FreeRAM
        {
            get
            {
                return this.AvailPhys;
            }
        }


        public override ulong TotalSwap
        {
            get
            {
                return this.TotalPageFile;
            }
        }

        public override ulong FreeSwap
        {
            get
            {
                return this.AvailPageFile;
            }
        }

        public WindowsMemoryMetrics()
        {
            GetWindowsMetrics(this);
        }
        


        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        private struct MEMORYSTATUSEX
        {
            // The size of the structure, in bytes. 
            // You must set this member before calling
            internal uint dwLength;

            // https://docs.microsoft.com/en-us/windows/win32/api/sysinfoapi/ns-sysinfoapi-memorystatusex
            // A number between 0 and 100 that specifies the approximate percentage of physical memory that is in use 
            // (0 indicates no memory use and 100 indicates full memory use).
            internal uint dwMemoryLoad;

            internal ulong ullTotalPhys;
            internal ulong ullAvailPhys;
            internal ulong ullTotalPageFile;
            internal ulong ullAvailPageFile;
            internal ulong ullTotalVirtual;
            internal ulong ullAvailVirtual;
            internal ulong ullAvailExtendedVirtual;

        }


        // [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        // [System.Runtime.InteropServices.DllImport("Kernel32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
        // internal static extern bool GlobalMemoryStatusEx([System.Runtime.InteropServices.In, System.Runtime.InteropServices.Out] MEMORYSTATUSEX lpBuffer);

        // Alternate Version Using "ref," And Works With Alternate Code Below.
        // Also See Alternate Version Of [MEMORYSTATUSEX] Structure With
        // Fields Documented.
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        [System.Runtime.InteropServices.DllImport("kernel32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, EntryPoint = "GlobalMemoryStatusEx", SetLastError = true)]
        private static extern bool GlobalMemoryStatusEx(ref MEMORYSTATUSEX lpBuffer);



        // https://stackoverflow.com/questions/1553336/how-can-i-get-the-total-physical-memory-in-c
        private static void GetWindowsMetrics(WindowsMemoryMetrics mm)
        {
            MEMORYSTATUSEX statEX = new MEMORYSTATUSEX();
            statEX.dwLength = (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(MEMORYSTATUSEX));
            GlobalMemoryStatusEx(ref statEX);

            mm.TotalPhys = statEX.ullTotalPhys;
            mm.AvailPhys = statEX.ullAvailPhys;
            mm.TotalPageFile = statEX.ullTotalPageFile;
            mm.AvailPageFile = statEX.ullAvailPageFile;
            mm.TotalVirtual = statEX.ullTotalVirtual;
            mm.AvailVirtual = statEX.ullAvailVirtual;
            mm.AvailExtendedVirtual = statEX.ullAvailExtendedVirtual;
        }


    }


}
