using System;
using System.Collections.Generic;
using System.Text;

namespace RamMonitorPrototype.Trash
{
    class TestPerformanceCounters
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



        public static void TestProcess()
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


        public static int MonitorProcess(string instanceName)
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



    }
}
