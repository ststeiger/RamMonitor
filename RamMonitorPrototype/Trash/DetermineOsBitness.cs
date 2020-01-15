
namespace RamMonitorPrototype
{


    // https://github.com/microsoft/referencesource/blob/master/System/compmod/microsoft/win32/UnsafeNativeMethods.cs
    // https://github.com/dotnet/corefx/blob/master/src/Common/src/CoreLib/System/Environment.Windows.cs
    public class DetermineOsBitness
    {
        private const string Kernel32 = "kernel32.dll";


        [System.Runtime.InteropServices.DllImport(Kernel32, CharSet = System.Runtime.InteropServices.CharSet.Auto, BestFitMapping = false)]
        [System.Runtime.Versioning.ResourceExposure(System.Runtime.Versioning.ResourceScope.Machine)]
        private static extern System.IntPtr GetModuleHandle(string modName);


        [System.Runtime.InteropServices.DllImport(Kernel32, CharSet = System.Runtime.InteropServices.CharSet.Ansi, BestFitMapping = false, SetLastError = true, ExactSpelling = true)]
        [System.Runtime.Versioning.ResourceExposure(System.Runtime.Versioning.ResourceScope.None)]
        private static extern System.IntPtr GetProcAddress(System.IntPtr hModule, string methodName);


        [System.Runtime.InteropServices.DllImport(Kernel32, SetLastError = true, CallingConvention = System.Runtime.InteropServices.CallingConvention.Winapi)]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        private static extern bool IsWow64Process(
             [System.Runtime.InteropServices.In] Microsoft.Win32.SafeHandles.SafeHandleZeroOrMinusOneIsInvalid hProcess,
             [System.Runtime.InteropServices.Out, System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)] out bool wow64Process
        );


        [System.Security.SecurityCritical]
        private static bool DoesWin32MethodExist(string moduleName, string methodName)
        {
            System.IntPtr hModule = GetModuleHandle(moduleName);

            if (hModule == System.IntPtr.Zero)
            {
                System.Diagnostics.Debug.Assert(hModule != System.IntPtr.Zero, "GetModuleHandle failed.  Dll isn't loaded?");
                return false;
            }

            System.IntPtr functionPointer = GetProcAddress(hModule, methodName);
            return (functionPointer != System.IntPtr.Zero);
        }


        public static bool Is64BitOperatingSystem()
        {
            if (System.IntPtr.Size * 8 == 64)
                return true;

            if (!DoesWin32MethodExist(Kernel32, "IsWow64Process"))
                return false;

            bool isWow64;

            using (Microsoft.Win32.SafeHandles.SafeWaitHandle safeHandle = new Microsoft.Win32.SafeHandles.SafeWaitHandle(System.Diagnostics.Process.GetCurrentProcess().Handle, true))
            {
                IsWow64Process(safeHandle, out isWow64);
            }

            return isWow64;
        }


        // This doesn't work reliably
        public static string GetProcessorArchitecture()
        {
            string strProcessorArchitecture = null;

            try
            {
                strProcessorArchitecture = System.Convert.ToString(System.Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE"));

                switch (typeof(string).Assembly.GetName().ProcessorArchitecture)
                {
                    case System.Reflection.ProcessorArchitecture.Amd64:
                        return "x86-64";
                    case System.Reflection.ProcessorArchitecture.X86:
                        strProcessorArchitecture = "x86";
                        break;
                    case System.Reflection.ProcessorArchitecture.Arm:
                        strProcessorArchitecture = "ARM";
                        break;
                }

                if (Is64BitOperatingSystem())
                {
                    return strProcessorArchitecture += "-64";
                }

                return strProcessorArchitecture += "-32";
            }
            catch (System.Exception ex)
            {
                strProcessorArchitecture = ex.Message;
            }

            return strProcessorArchitecture;
        } // End Function GetProcessorArchitecture


    } // End Class DetermineOsBitness 


} // End Namespace RamMonitorPrototype 
