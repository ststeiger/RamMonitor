
namespace RamMonitorPrototype
{


    class Program
    {


        static void Main(string[] args)
        {
            System.Data.DataTable dt = Linux.ProcFS.GetMemInfo();
            System.Console.WriteLine(dt.Rows.Count);

            Utsname uts = new Utsname();
            uts.Write(System.Console.Out);

            System.Console.WriteLine(OsInfo.OSFullName);
            System.Console.WriteLine(DetermineOsBitness.GetProcessorArchitecture());
            System.Console.WriteLine(DetermineOsBitness.Is64BitOperatingSystem());
            
            System.Console.Write(@"OS Description: ");
            System.Console.WriteLine(System.Runtime.InteropServices.RuntimeInformation.OSDescription);
            System.Console.WriteLine(System.Runtime.InteropServices.RuntimeInformation.OSArchitecture);
            System.Console.WriteLine(System.Runtime.InteropServices.RuntimeEnvironment.GetSystemVersion());

            // ProcessManager.KillProcessGroup("chrome");
            ProcessMemoryMetrics mem = ProcessManager.GetProcessGroupMemory("chrome");
            // mem.WriteMemory(System.Console.Out);


            // https://forums.freebsd.org/threads/getting-free-and-real-memory-with-c.38754/
            // https://stackoverflow.com/questions/2513505/how-to-get-available-memory-c-g
            // System.GC.GetTotalMemory(true);
            
            GlobalMemoryMetrics metrics = OsInfo.MemoryMetrics;
            System.Console.WriteLine(metrics.Load);
            System.Console.WriteLine(metrics.SwapLoad);

            GlobalMemoryMetrics procMem = new LinuxMemoryMetrics();
            System.Console.WriteLine(procMem);
            procMem.WriteMemory(System.Console.Out);
            
            GlobalMemoryMetrics winMem = new WindowsMemoryMetrics();
            System.Console.WriteLine(winMem);
            winMem.WriteMemory(System.Console.Out);
            


            System.Console.WriteLine(System.Environment.NewLine);
            System.Console.WriteLine(" --- Press any key to continue --- ");
            System.Console.ReadKey();
        } // End Sub Main(string[] args) 


    } // End Class Program 


} // End Namespace RamMonitorPrototype 
