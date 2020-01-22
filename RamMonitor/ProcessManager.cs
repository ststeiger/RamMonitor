
namespace RamMonitorPrototype
{


    public class ProcessManager
    {


        public static ProcessMemoryMetrics GetProcessGroupMemory(string processName)
        {
            ProcessMemoryMetrics mem = new ProcessMemoryMetrics();

            System.Diagnostics.Process[] proc = System.Diagnostics.Process.GetProcessesByName(processName);

            foreach (System.Diagnostics.Process pro in proc)
            {
                // mem.MaxWorkingSet += pro.MaxWorkingSet;
                // mem.MinWorkingSet += pro.MinWorkingSet;

                mem.WorkingSet += pro.WorkingSet;
                mem.WorkingSet64 += pro.WorkingSet64;

                mem.PeakWorkingSet += pro.PeakWorkingSet;
                mem.PeakWorkingSet64 += pro.PeakWorkingSet64;

                mem.VirtualMemorySize += pro.VirtualMemorySize;
                mem.VirtualMemorySize64 += pro.VirtualMemorySize64;

                mem.PeakVirtualMemorySize += pro.PeakVirtualMemorySize;
                mem.PeakVirtualMemorySize64 += pro.PeakVirtualMemorySize64;

                mem.PrivateMemorySize += pro.PrivateMemorySize;
                mem.PrivateMemorySize64 += pro.PrivateMemorySize64;

                mem.PagedMemorySize += pro.PagedMemorySize;
                mem.PagedMemorySize64 += pro.PagedMemorySize64;

                mem.PeakPagedMemorySize += pro.PeakPagedMemorySize;
                mem.PeakPagedMemorySize64 += pro.PeakPagedMemorySize64;

                mem.PagedSystemMemorySize += pro.PagedSystemMemorySize;
                mem.PagedSystemMemorySize64 += pro.PagedSystemMemorySize64;

                mem.NonpagedSystemMemorySize += pro.NonpagedSystemMemorySize;
                mem.NonpagedSystemMemorySize64 += pro.NonpagedSystemMemorySize64;
            } // Next pro 

            return mem;
        } // End Function GetProcessGroupMemory 


        public static void KillProcessGroup(string processName)
        {
            System.Diagnostics.Process[] proc = System.Diagnostics.Process.GetProcessesByName(processName);

            foreach (System.Diagnostics.Process pro in proc)
            {
                try
                {
                    pro.CloseMainWindow();
                }
                catch (System.Exception) { }

                try
                {
                    pro.Close();
                }
                catch (System.Exception) { }
            } // Next pro 


            System.Diagnostics.Process[] procs = System.Diagnostics.Process.GetProcessesByName(processName);

            foreach (System.Diagnostics.Process pro in procs)
            {
                try
                {
                    pro.Kill();
                }
                catch (System.Exception) { }

            } // Next pro 
        } // End Sub KillProcessGroup 


    } // End Class ProcessManager 


} // End Namespace RamMonitorPrototype
