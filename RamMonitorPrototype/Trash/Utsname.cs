
namespace RamMonitorPrototype
{


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


        public void Write(System.IO.TextWriter tw)
        {
            tw.Write("SysName:\t");
            tw.WriteLine(this.SysName); // Linux 

            tw.Write("NodeName:\t");
            tw.WriteLine(this.NodeName); // System.Environment.MachineName

            tw.Write("Release:\t");
            tw.WriteLine(this.Release); // Kernel-version

            tw.Write("Version:\t");
            tw.WriteLine(this.Version); // #40~18.04.1-Ubuntu SMP Thu Nov 14 12:06:39 UTC 2019

            tw.Write("Machine:\t");
            tw.WriteLine(this.Machine); // x86_64

            tw.Write("DomainName:\t");
            tw.WriteLine(this.DomainName); // (none)
        } // End Sub Print 


        public Utsname()
        {
            Initialize(this);
        } // End Constructor 


        [System.Runtime.InteropServices.DllImport("libc", EntryPoint = "uname", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        private static extern int uname_syscall(System.IntPtr buf);

        // https://github.com/jpobst/Pinta/blob/master/Pinta.Core/Managers/SystemManager.cs
        private static void Initialize(Utsname uts)
        {
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
            {
                if (DetermineOsBitness.Is64BitOperatingSystem())
                    uts.SysName = "Windows (64-Bit)";
                else
                    uts.SysName = "Windows (32-Bit)";

                uts.NodeName = System.Environment.MachineName;
                uts.Release = System.Environment.OSVersion.Version.ToString();
                uts.Version = System.Runtime.InteropServices.RuntimeInformation.OSDescription;
                uts.Machine = System.Runtime.InteropServices.RuntimeInformation.OSArchitecture.ToString();
                uts.DomainName = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;

                return;
            } // End if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows)) 


            System.IntPtr buf = System.IntPtr.Zero;

            buf = System.Runtime.InteropServices.Marshal.AllocHGlobal(8192);
            // This is a hacktastic way of getting sysname from uname ()
            if (uname_syscall(buf) == 0)
            {
                uts = new Utsname();

                long bufVal = buf.ToInt64();
                uts.SysName = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(buf);
                uts.NodeName = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(new System.IntPtr(bufVal + 1 * 65));
                uts.Release = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(new System.IntPtr(bufVal + 2 * 65));
                uts.Version = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(new System.IntPtr(bufVal + 3 * 65));
                uts.Machine = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(new System.IntPtr(bufVal + 4 * 65));
                uts.DomainName = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(new System.IntPtr(bufVal + 5 * 65));

                if (buf != System.IntPtr.Zero)
                    System.Runtime.InteropServices.Marshal.FreeHGlobal(buf);
            } // End if (uname_syscall(buf) == 0) 

        } // End Function Uname


    } // End Class Utsname 


} // End Namespace RamMonitorPrototype 
