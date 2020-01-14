
namespace RamMonitorPrototype.Trash
{


    class EquivalentVB
    {


        public static void foo()
        {
            //Microsoft.VisualBasic.Devices.ComputerInfo ci = New Microsoft.VisualBasic.Devices.ComputerInfo();
            dynamic ci = new string[] { };

            System.Console.WriteLine("OSFullName: ");
            System.Console.WriteLine(ci.OSFullName);
            System.Console.WriteLine(System.Environment.NewLine);


            System.Console.WriteLine("TotalPhysicalMemory: ");
            System.Console.WriteLine(ci.TotalPhysicalMemory.ToString() + " bytes");
            System.Console.WriteLine(System.Environment.NewLine);

            System.Console.WriteLine("AvailablePhysicalMemory: ");
            System.Console.WriteLine(ci.AvailablePhysicalMemory.ToString() + " bytes free");
            System.Console.WriteLine(System.Environment.NewLine);

            System.Console.WriteLine("TotalVirtualMemory: ");
            System.Console.WriteLine(ci.TotalVirtualMemory.ToString() + " bytes");
            System.Console.WriteLine(System.Environment.NewLine);

            System.Console.WriteLine("AvailableVirtualMemory: ");
            System.Console.WriteLine(ci.AvailableVirtualMemory.ToString() + " bytes free");
            System.Console.WriteLine(System.Environment.NewLine);

            System.Console.WriteLine("Memory assigned to process: ");
            System.Console.WriteLine(System.Environment.WorkingSet.ToString() + " bytes");
            System.Console.WriteLine(System.Environment.NewLine);




            System.Console.WriteLine(System.Environment.NewLine);
            System.Console.WriteLine(System.Environment.SystemPageSize);
            System.Console.WriteLine(System.Environment.WorkingSet);
            System.Console.WriteLine(System.Environment.Is64BitProcess);
            System.Console.WriteLine(System.Environment.Is64BitOperatingSystem);

            System.Console.WriteLine(System.Diagnostics.Process.GetCurrentProcess().PeakWorkingSet);
            System.Console.WriteLine(System.Diagnostics.Process.GetCurrentProcess().PeakWorkingSet64);
            System.Console.WriteLine(System.Diagnostics.Process.GetCurrentProcess().WorkingSet64);
            System.Console.WriteLine(System.Diagnostics.Process.GetCurrentProcess().PeakWorkingSet64);

            // var performance = new System.Diagnostics.PerformanceCounter("Memory", "Available MBytes");
            // var memory = performance.NextValue();

        }

    }
}
