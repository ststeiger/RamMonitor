
namespace RamMonitorPrototype
{




    class Program
    {


        public static void bar()
        {
            PerfMapper.LoadNames();
            System.Console.WriteLine(PerfMapper.English);
            System.Console.WriteLine(PerfMapper.Localized);
            System.Console.WriteLine(PerfMapper.ReverseLocalized);



            System.Diagnostics.PerformanceCounterCategory[] categories = System.Diagnostics.PerformanceCounterCategory.GetCategories();
            foreach (System.Diagnostics.PerformanceCounterCategory category in categories)
            {
                System.Console.WriteLine(category.CategoryName);
                // System.Console.WriteLine(category.CategoryType);

                int ind = PerfMapper.ReverseLocalized[category.CategoryName];

                if (PerfMapper.ReverseEnglish.ContainsKey(ind))
                {
                    string eng = PerfMapper.ReverseEnglish[ind];
                    System.Console.WriteLine(eng);
                }
                else
                    System.Console.WriteLine("Kaboom");


                //string[] instanceNames = category.GetInstanceNames();
                //foreach (string instanceName in instanceNames)
                //{
                //    if (category.CounterExists(instanceName))
                //    {
                //        System.Diagnostics.PerformanceCounter[] counters = category.GetCounters(instanceName);
                //    }
                //}   
            }
        }

        public static void foo()
        {
            System.Diagnostics.Process proc = System.Diagnostics.Process.GetCurrentProcess(); //  assign your process here: -)
            
            int memsize = 0; // memsize in Megabyte
            using (System.Diagnostics.PerformanceCounter pc = new System.Diagnostics.PerformanceCounter())
            {
                pc.CategoryName = "Process";
                pc.CounterName = "Working Set - Private";
                pc.InstanceName = proc.ProcessName;
                memsize = System.Convert.ToInt32(pc.NextValue()) / (int)(1024);
                pc.Close();
            }

        }

        public static long GetProcessPrivateWorkingSet64Size(int process_id)
        {
            long process_size = 0;
            System.Diagnostics.Process process = System.Diagnostics.Process.GetProcessById(process_id);
            if (process == null) return process_size;
            string instanceName = GetProcessInstanceName(process.Id);
            var counter = new System.Diagnostics.PerformanceCounter("Process", "Working Set - Private", instanceName, true);
            process_size = System.Convert.ToInt32(counter.NextValue()) / 1024;
            return process_size;
        }

        public static string GetProcessInstanceName(int process_id)
        {
            System.Diagnostics.PerformanceCounterCategory cat = new System.Diagnostics.PerformanceCounterCategory("Process");
            string[] instances = cat.GetInstanceNames();
            foreach (string instance in instances)
            {
                using (System.Diagnostics.PerformanceCounter cnt = new System.Diagnostics.PerformanceCounter("Process", "ID Process", instance, true))
                {
                    int val = (int)cnt.RawValue;
                    if (val == process_id)
                        return instance;
                }
            }
            throw new System.Exception("Could not find performance counter ");
        }

        public static long GetPrivateWorkingSetForAllProcesses(string ProcessName)
        {
            long totalMem = 0;
            System.Diagnostics.Process[] process = System.Diagnostics.Process.GetProcessesByName(ProcessName);
            foreach (System.Diagnostics.Process proc in process)
            {
                long memsize = GetProcessPrivateWorkingSet64Size(proc.Id);
                totalMem += memsize;
            }
            return totalMem;
        }

        public int MonitorProcess(string instanceName)
        {

            int count = 0, sum = 0, buff = 0;
            while (true)
            {

                try
                {
                    System.Diagnostics.Process[] proc = System.Diagnostics.Process.GetProcessesByName(instanceName);
                    System.Diagnostics.PerformanceCounter PC;
                    sum = proc.Length;
                    int k = 0;
                    foreach (System.Diagnostics.Process pro in proc)
                    {
                        if (k == 0)
                        {
                            PC = new System.Diagnostics.PerformanceCounter("Process", "Working Set - Private", instanceName, true);
                        }
                        else
                        {
                            PC = new System.Diagnostics.PerformanceCounter("Process", "Working Set - Private", instanceName + "#" + (k).ToString(), true);
                        }
                        int memsize = System.Convert.ToInt32(PC.NextValue()) / 1024;
                        System.Console.WriteLine(memsize);
                        buff = buff + memsize; //adding memsize of current running instance
                        PC.Dispose();
                        PC.Close();
                        count++;
                        k++;
                    }
                    System.Threading.Thread.Sleep(800);
                    return buff;
                }
                catch (System.Exception e)
                {
                    System.Console.WriteLine(e.Message);

                    System.Threading.Thread.Sleep(800);
                    return 0;
                }
            }
        }

        static void Main(string[] args)
        {
            // https://forums.freebsd.org/threads/getting-free-and-real-memory-with-c.38754/
            // https://stackoverflow.com/questions/2513505/how-to-get-available-memory-c-g
            // System.GC.GetTotalMemory(true);
            bar();

            MicrophoneTest.MemoryMetrics metrics = MicrophoneTest.OsInfo.GetMetrics();
            System.Console.WriteLine(metrics.Free);



            //Microsoft.VisualBasic.Devices.ComputerInfo ci = New Microsoft.VisualBasic.Devices.ComputerInfo();
            dynamic ci = new string[] { };

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


            System.Console.WriteLine(System.Runtime.InteropServices.RuntimeInformation.OSDescription);
            System.Console.WriteLine(System.Runtime.InteropServices.RuntimeEnvironment.GetSystemVersion());
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux))

                System.Console.WriteLine(" --- Press any key to continue --- ");
            System.Console.ReadKey();
        }
    }
}
