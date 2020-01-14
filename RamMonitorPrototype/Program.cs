
using System.Management;

namespace RamMonitorPrototype
{


    class Program
    {


        static void Main(string[] args)
        {
            System.Console.WriteLine(OsInfo.OSFullName);

            // ProcessManager.KillProcessGroup("chrome");
            ProcessMemoryMetrics mem = ProcessManager.GetProcessGroupMemory("chrome");
            mem.WriteMemory(System.Console.Out);


            // https://forums.freebsd.org/threads/getting-free-and-real-memory-with-c.38754/
            // https://stackoverflow.com/questions/2513505/how-to-get-available-memory-c-g
            // System.GC.GetTotalMemory(true);
            
            SystemMemoryMetrics metrics = OsInfo.GetMetrics();
            System.Console.WriteLine(metrics.Free);


            

            System.Console.WriteLine(System.Runtime.InteropServices.RuntimeInformation.OSDescription);
            System.Console.WriteLine(System.Runtime.InteropServices.RuntimeInformation.OSArchitecture);
            System.Console.WriteLine(System.Runtime.InteropServices.RuntimeEnvironment.GetSystemVersion());

            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux))
                System.Console.WriteLine("Linux.");

            System.Console.WriteLine(System.Environment.NewLine);
            System.Console.WriteLine(" --- Press any key to continue --- ");
            System.Console.ReadKey();
        }


    }


}
