
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


        public static GlobalMemoryMetrics MemoryMetrics
        {
            get
            {
                return GlobalMemoryMetrics.Instance;
            }
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


        private static string s_osFullName;


        public static string OSFullName
        {
            get
            {
                if (s_osFullName != null)
                    return s_osFullName;

                try
                {

                    s_osFullName = System.Runtime.InteropServices.RuntimeInformation.OSDescription + " (" + System.Runtime.InteropServices.RuntimeInformation.OSArchitecture + ")";

                    if (!System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
                        return s_osFullName;

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

                                s_osFullName = System.Convert.ToString(enumerator.Current.Properties["Name"].Value);

                                if (s_osFullName.Contains(System.Convert.ToString('|')))
                                {
                                    s_osFullName = s_osFullName.Substring(0, s_osFullName.IndexOf('|'));
                                    
                                } // End if (s_osFullName.Contains(System.Convert.ToString('|'))) 

                                s_osFullName += " ["+System.Runtime.InteropServices.RuntimeInformation.OSArchitecture+"] ";

                                s_osFullName += " (" + System.Environment.OSVersion.Platform.ToString() + " "
                                        + System.Environment.OSVersion.Version.ToString();

                                if (!string.IsNullOrEmpty(System.Environment.OSVersion.ServicePack))
                                {
                                    s_osFullName += " SP" + System.Environment.OSVersion.ServicePack;
                                }
                                s_osFullName += ")";

                                return s_osFullName;
                            } // End Using enumerator 

                        } // End Using managementObjectCollection 

                    } // End Using mos 

                }
                catch (System.Exception)
                {
                    s_osFullName = System.Runtime.InteropServices.RuntimeInformation.OSDescription + " (" + System.Runtime.InteropServices.RuntimeInformation.OSArchitecture + ")";

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
