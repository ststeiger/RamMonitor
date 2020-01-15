
namespace RamMonitorPrototype
{


    public class OsInfo
    {

        [System.Runtime.InteropServices.DllImport("kernel32.dll", EntryPoint = "GetSystemDefaultLangID")]
        public static extern int GetSystemDefaultLangID();

        [System.Runtime.InteropServices.DllImport("kernel32.dll", EntryPoint = "GetSystemDefaultLCID")]
        static extern uint GetSystemDefaultLCID();

        [System.Runtime.InteropServices.DllImport("kernel32.dll", EntryPoint = "GetUserDefaultLCID")]
        private static extern int GetUserDefaultLCID();

        [System.Runtime.InteropServices.DllImport("kernel32.dll", EntryPoint = "GetOEMCP")]
        public static extern int GetOEMCP();



        public static System.Globalization.CultureInfo CurrentCultureInRegionalSettings()
        {
            // System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator;
            // System.Globalization.CultureInfo.CurrentCulture.TextInfo.


            return new System.Globalization.CultureInfo(GetUserDefaultLCID());
        }


        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        internal struct MEMORYSTATUSEX
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
        static extern bool GlobalMemoryStatusEx(ref MEMORYSTATUSEX lpBuffer);



        // https://stackoverflow.com/questions/1553336/how-can-i-get-the-total-physical-memory-in-c
        private static SystemMemoryMetrics GetWindowsMetrics()
        {
            MEMORYSTATUSEX statEX = new MEMORYSTATUSEX();
            statEX.dwLength = (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(MEMORYSTATUSEX));
            GlobalMemoryStatusEx(ref statEX);

            SystemMemoryMetrics mm = new SystemMemoryMetrics();

            mm.Used = statEX.dwMemoryLoad;
            mm.TotalPhys = statEX.ullTotalPhys;
            mm.AvailPhys = statEX.ullAvailPhys;
            mm.TotalPageFile = statEX.ullTotalPageFile;
            mm.AvailPageFile = statEX.ullAvailPageFile;
            mm.TotalVirtual = statEX.ullTotalVirtual;
            mm.AvailVirtual = statEX.ullAvailVirtual;
            mm.AvailExtendedVirtual = statEX.ullAvailExtendedVirtual;

            return mm;
        }


        private static SystemMemoryMetrics GetUnixMetrics()
        {
            SystemMemoryMetrics mm = new SystemMemoryMetrics();

            // At this point, we already checked if /proc/meminfo exists 
            // https://github.com/ststeiger/ReadMemInfo/blob/master/ReadMemInfo/Proc.cs

            using (System.Data.DataTable dt = Linux.ProcFS.GetMemInfo())
            {
                System.Console.WriteLine(dt);
            }

            return mm;
        }


        public static SystemMemoryMetrics GetMetrics()
        {
            SystemMemoryMetrics metrics = null;

            // X-Box supports GlobalMemoryStatusEx 
            // https://github.com/microsoft/Xbox-ATG-Samples/blob/master/UWPSamples/System/SystemInfoUWP/SystemInfo.cpp
            if (System.Environment.OSVersion.Platform != System.PlatformID.Unix)
            {
                metrics = GetWindowsMetrics();
            }
            else if (System.IO.File.Exists("/proc/meminfo"))
            {
                metrics = GetUnixMetrics();
            }
            else
            {
                throw new System.NotSupportedException("GetMetrics not supported for current operating system.");
            }

            return metrics;
        }


        public enum OSType_t
        {
            Windows,
            Linux,
            OSX,
            Unix,
            XBox,
            UNKNOWN
        }


        public static OSType_t OsType
        {
            get
            {
                if (System.Environment.OSVersion.Platform == System.PlatformID.Xbox)
                    return OSType_t.XBox;

                if (System.Environment.OSVersion.Platform != System.PlatformID.Unix)
                    return OSType_t.Windows;

                if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux))
                    return OSType_t.Linux;

                if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.OSX))
                    return OSType_t.OSX;

                if (System.Environment.OSVersion.Platform == System.PlatformID.Unix)
                    return OSType_t.Unix;

                return OSType_t.UNKNOWN;
            }

        }



        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern void GetNativeSystemInfo(ref SYSTEM_INFO lpSystemInfo);

        private const int PROCESSOR_ARCHITECTURE_AMD64 = 9;
        private const int PROCESSOR_ARCHITECTURE_IA64 = 6;
        private const int PROCESSOR_ARCHITECTURE_INTEL = 0;


        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        private struct SYSTEM_INFO
        {
            public short wProcessorArchitecture;
            public short wReserved;
            public int dwPageSize;
            public System.IntPtr lpMinimumApplicationAddress;
            public System.IntPtr lpMaximumApplicationAddress;
            public System.IntPtr dwActiveProcessorMask;
            public int dwNumberOfProcessors;
            public int dwProcessorType;
            public int dwAllocationGranularity;
            public short wProcessorLevel;
            public short wProcessorRevision;
        }


        public static System.Reflection.ProcessorArchitecture GetProcessorArchitecture()
        {
            SYSTEM_INFO si = new SYSTEM_INFO();
            GetNativeSystemInfo(ref si);
            switch (si.wProcessorArchitecture)
            {
                case PROCESSOR_ARCHITECTURE_AMD64:
                    return System.Reflection.ProcessorArchitecture.Amd64;

                case PROCESSOR_ARCHITECTURE_IA64:
                    return System.Reflection.ProcessorArchitecture.IA64;

                case PROCESSOR_ARCHITECTURE_INTEL:
                    return System.Reflection.ProcessorArchitecture.X86;

                default:
                    return System.Reflection.ProcessorArchitecture.None; // that's weird :-)
            }
        }


        public static void GetProcessorArchitectureSimple()
        {
            switch (typeof(string).Assembly.GetName().ProcessorArchitecture)
            {
                case System.Reflection.ProcessorArchitecture.X86:
                    break;
                case System.Reflection.ProcessorArchitecture.Amd64:
                    break;
                case System.Reflection.ProcessorArchitecture.IA64:
                    break;
                case System.Reflection.ProcessorArchitecture.Arm:
                    break;
            }
        }

        public static System.Runtime.InteropServices.OSPlatform GetOperatingSystem()
        {
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.OSX))
            {
                return System.Runtime.InteropServices.OSPlatform.OSX;
            }

            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux))
            {
                return System.Runtime.InteropServices.OSPlatform.Linux;
            }

            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
            {
                return System.Runtime.InteropServices.OSPlatform.Windows;
            }

            throw new System.Exception("Cannot determine operating system!");
        }



        // https://stackoverflow.com/a/55202696/155077
        //[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        //unsafe internal struct Utsname_internal
        //{
        //    public fixed byte sysname[65];
        //    public fixed byte nodename[65];
        //    public fixed byte release[65];
        //    public fixed byte version[65];
        //    public fixed byte machine[65];
        //    public fixed byte domainname[65];
        //}


        public class Utsname
        {
            public string SysName; // char[65]
            public string NodeName; // char[65]
            public string Release; // char[65]
            public string Version; // char[65]
            public string Machine; // char[65]
            public string DomainName; // char[65]

            public void Print()
            {
                System.Console.Write("SysName:\t");
                System.Console.WriteLine(this.SysName); // Linux 

                System.Console.Write("NodeName:\t");
                System.Console.WriteLine(this.NodeName); // System.Environment.MachineName

                System.Console.Write("Release:\t");
                System.Console.WriteLine(this.Release); // Kernel-version

                System.Console.Write("Version:\t");
                System.Console.WriteLine(this.Version); // #40~18.04.1-Ubuntu SMP Thu Nov 14 12:06:39 UTC 2019

                System.Console.Write("Machine:\t");
                System.Console.WriteLine(this.Machine); // x86_64

                System.Console.Write("DomainName:\t");
                System.Console.WriteLine(this.DomainName); // (none)
            }


        }

        
        [System.Runtime.InteropServices.DllImport("libc", EntryPoint = "uname", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        private static extern int uname_syscall(System.IntPtr buf);

        // https://github.com/jpobst/Pinta/blob/master/Pinta.Core/Managers/SystemManager.cs
        private static Utsname Uname()
        {
            Utsname uts = null;
            System.IntPtr buf = System.IntPtr.Zero;

            buf = System.Runtime.InteropServices.Marshal.AllocHGlobal(8192);
            // This is a hacktastic way of getting sysname from uname ()
            if (uname_syscall(buf) == 0)
            {
                uts = new Utsname();
                uts.SysName = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(buf);

                long bufVal = buf.ToInt64();
                uts.NodeName = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(new System.IntPtr(bufVal + 1 * 65));
                uts.Release = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(new System.IntPtr(bufVal + 2 * 65));
                uts.Version = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(new System.IntPtr(bufVal + 3 * 65));
                uts.Machine = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(new System.IntPtr(bufVal + 4 * 65));
                uts.DomainName = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(new System.IntPtr(bufVal + 5 * 65));

                if (buf != System.IntPtr.Zero)
                    System.Runtime.InteropServices.Marshal.FreeHGlobal(buf);
            } // End if (uname_syscall(buf) == 0) 

            return uts;
        } // End Function Uname



        private static string s_osFullName;


        public static string OSFullName
        {
            get
            {
                if (s_osFullName != null)
                    return s_osFullName;

                try
                {
                    
                    if (System.Environment.OSVersion.Platform == System.PlatformID.Unix)
                    {
                        s_osFullName = System.Environment.OSVersion.Platform.ToString();
                        
                        s_osFullName = System.Runtime.InteropServices.RuntimeInformation.OSDescription + " (" + System.Runtime.InteropServices.RuntimeInformation.OSArchitecture + ")";
                        
                        
                        // Utsname uts = Uname();
                        // s_osFullName = uts.SysName + " (" + uts.Release + ") " + uts.Version;
                        
                        // System.Console.WriteLine(System.Environment.OSVersion.Platform.ToString());
                        // System.Console.WriteLine(System.Environment.OSVersion.Version); // Kernel-Version
                        // System.Console.WriteLine(System.Environment.OSVersion.VersionString);
                        // System.Console.WriteLine(System.Environment.OSVersion.Version.Major);
                        
                        return s_osFullName;
                    }
                    
                    
                    System.Management.SelectQuery query = new System.Management.SelectQuery("Win32_OperatingSystem");

                    using (System.Management.ManagementObjectSearcher mos = new System.Management.ManagementObjectSearcher(query))
                    {
                        using (System.Management.ManagementObjectCollection managementObjectCollection = mos.Get())
                        {

                            if (managementObjectCollection.Count <= 0)
                            {
                                throw new System.InvalidOperationException(@"Could not obtain full operation system name due to internal error. 
This might be caused by WMI not existing on the current machine.");
                            }

                            using (System.Management.ManagementObjectCollection.ManagementObjectEnumerator enumerator = 
                                managementObjectCollection.GetEnumerator())
                            {
                                enumerator.MoveNext();

                                if (s_osFullName == null)
                                { 
                                    s_osFullName = System.Convert.ToString(enumerator.Current.Properties["Name"].Value);

                                    if (s_osFullName.Contains(System.Convert.ToString('|')))
                                    {
                                        s_osFullName = s_osFullName.Substring(0, s_osFullName.IndexOf('|'));
                                        return s_osFullName;
                                    } // End if (s_osFullName.Contains(System.Convert.ToString('|'))) 

                                } // End if (s_osFullName == null) 

                                return s_osFullName;
                            } // End Using enumerator 

                        } // End Using managementObjectCollection 

                    } // End Using mos 

                }
                catch (System.Runtime.InteropServices.COMException)
                {
                    s_osFullName = System.Environment.OSVersion.Platform.ToString();
                    return s_osFullName;
                }
            }
        }



        public static void ListInfos()
        {

            //var pc = new System.Diagnostics.PerformanceCounter("Mono Memory", "Available Physical Memory");
            //long availableMemory = pc.RawValue;
            //var pc2 = new System.Diagnostics.PerformanceCounter("Mono Memory", "Total Physical Memory");
            //long physicalMemory = pc2.RawValue;

            // Microsoft.VisualBasic.Devices.ComputerInfo ci = new Microsoft.VisualBasic.Devices.ComputerInfo();

            // typeof(Microsoft.AspNetCore.Mvc.Controller).Assembly.GetName().Version
            // new System.Reflection.AssemblyName(typeof(Microsoft.AspNetCore.Mvc.MvcOptions).Assembly.FullName).Version.ToString();


            // Microsoft.Extensions.PlatformAbstractions.PlatformServices.Default.Application.ApplicationVersion

            // https://github.com/dotnet-architecture/HealthChecks/blob/dev/src/Microsoft.Extensions.HealthChecks/Checks/SystemChecks.cs

            // System.Diagnostics.Process.GetCurrentProcess().WorkingSet64;
            // System.Diagnostics.Process.GetCurrentProcess().MaxWorkingSet;
            // System.Diagnostics.Process.GetCurrentProcess().PeakWorkingSet;
            // System.Diagnostics.Process.GetCurrentProcess().PeakWorkingSet64;




            System.Console.WriteLine(System.Globalization.CultureInfo.InstalledUICulture.EnglishName);

            // System.Collections.Generic.List<MemoryMetrics> ls = new System.Collections.Generic.List<MemoryMetrics>();
            //using System.Linq;
            // ulong[] a = ls.Select(x => x.AvailExtendedVirtual).ToArray();
            // ulong[] a = System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Select(ls, x => x.AvailExtendedVirtual));

            System.Console.WriteLine(System.Runtime.InteropServices.RuntimeInformation.OSDescription);
            System.Console.WriteLine(System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture);
            System.Console.WriteLine(System.Runtime.InteropServices.RuntimeInformation.OSArchitecture);
            System.Console.WriteLine(System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription);

            System.Console.WriteLine(System.Environment.Is64BitOperatingSystem);
            System.Console.WriteLine(System.Environment.WorkingSet);
            System.Console.WriteLine(System.Environment.SystemPageSize);
            System.Console.WriteLine(System.Environment.ProcessorCount);
            System.Console.WriteLine(System.Environment.MachineName);
            System.Console.WriteLine(System.Environment.OSVersion.Platform);
            System.Console.WriteLine(System.Environment.OSVersion.VersionString);
            // System.Console.WriteLine(System.Environment.OSVersion.Version.*);
            System.Console.WriteLine(System.Environment.OSVersion.ServicePack);

            System.Console.WriteLine(System.Environment.Is64BitProcess);
            System.Console.WriteLine(System.Environment.TickCount); // uptime
        }


        // https://stackoverflow.com/questions/767613/identifying-the-cpu-architecture-type-using-c-sharp
        public static void GetProcessArchitecture()
        {
            System.Reflection.PortableExecutableKinds peKind;
            System.Reflection.ImageFileMachine machine;
            typeof(object).Module.GetPEKind(out peKind, out machine);

            System.Console.WriteLine(peKind);
            System.Console.WriteLine(machine);
        }


    }


}
