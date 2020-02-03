
namespace RamMonitor
{


    public class OsInfo
    {


        public static System.Globalization.CultureInfo InstalledCulture
        {
            get
            {
                return System.Globalization.CultureInfo.InstalledUICulture;
            }
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

        } // End Property OsType 


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
        } // End Function GetOperatingSystem 


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

            } // End Get 

        } // End Property OSFullName 


    } // End Class OsInfo 


} // End Namespace RamMonitorPrototype 
