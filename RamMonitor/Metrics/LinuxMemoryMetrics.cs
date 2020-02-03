
namespace RamMonitor
{


    [System.AttributeUsage(System.AttributeTargets.Field | System.AttributeTargets.Property
        , Inherited = true, AllowMultiple = false)]
    public class MemInfoNameAttribute
        : System.Attribute
    {

        public string Name;

        public MemInfoNameAttribute(string name)
        {
            this.Name = name;
        }

    }


    // https://www.thegeekdiary.com/understanding-proc-meminfo-file-analyzing-memory-utilization-in-linux/
    // https://docs.fedoraproject.org/en-US/Fedora/18/html/System_Administrators_Guide/s2-proc-meminfo.html
    // https://linux-audit.com/understanding-memory-information-on-linux-systems/
    public class LinuxMemoryMetrics
        : GlobalMemoryMetrics
    {


        public override ulong TotalRAM
        {
            get
            {
                return this.MemTotal;
            }
        }

        public override ulong FreeRAM
        {
            get
            {
                return this.MemAvailable;
            }
        }


        public override ulong TotalSwap
        {
            get
            {
                return this.SwapTotal;
            }
        }

        public override ulong FreeSwap
        {
            get
            {
                return this.SwapFree;
            }
        }


        // MemTotal: Total usable ram 
        // (i.e. physical ram minus a few reserved bits and the kernel binary code)
        // MemTotal: Total amount of physical RAM, in kilobytes.
        public ulong MemTotal;

        // MemFree: Is sum of LowFree+HighFree (overall stat)
        // MemFree: The amount of physical RAM, in kilobytes, left unused by the system.
        public ulong MemFree;

        // MemShared: 0; is here for compat reasons but always zero.


        public ulong MemAvailable;

        // Buffers: The amount of physical RAM, in kilobytes, used for file buffers.
        // Buffers: Memory in buffer cache. 
        // mostly useless as metric nowadays 
        // Relatively temporary storage for raw disk blocks shouldn’t get tremendously large 
        // (20MB or so)
        public ulong Buffers;
        // Cached: Memory in the pagecache (diskcache) minus SwapCache, Doesn’t include SwapCached
        public ulong Cached;

        // SwapCache: Memory that once was swapped out, 
        // is swapped back in but still also is in the swapfile 
        // (if memory is needed it doesn’t need to be swapped out AGAIN 
        // because it is already in the swapfile. This saves I/O )
        public ulong SwapCached;

        // Active: Memory that has been used more recently and usually not reclaimed unless absolute necessary.
        public ulong Active;
        public ulong Inactive;

        [MemInfoName("Active(anon)")]
        public ulong Active_anon;

        [MemInfoName("Inactive(anon)")]
        public ulong Inactive_anon;

        [MemInfoName("Active(file)")]
        public ulong Active_file;

        [MemInfoName("Inactive(file)")]
        public ulong Inactive_file;

        public ulong Unevictable;
        public ulong Mlocked;

        // SwapTotal: The total amount of swap available, in kilobytes
        public ulong SwapTotal; // Total amount of physical swap memory.
        // SwapFree: Total amount of swap memory free. 
        // Memory which has been evicted from RAM, and is temporarily on the disk
        // SwapFree: The total amount of swap free, in kilobytes.
        public ulong SwapFree;

        public ulong Dirty;
        public ulong Writeback;
        public ulong AnonPages;
        public ulong Mapped;

        // Shared memory: 
        public ulong Shmem;

        public ulong KReclaimable;
        public ulong Slab; // Slab: in-kernel data structures cache
        public ulong SReclaimable;
        public ulong SUnreclaim;
        public ulong KernelStack;
        public ulong PageTables;
        public ulong NFS_Unstable;
        public ulong Bounce;
        public ulong WritebackTmp;
        public ulong CommitLimit;
        public ulong Committed_AS;

        // VMallocTotal: The total amount of memory, in kilobytes, of total allocated virtual address space.
        public ulong VmallocTotal;
        // VMallocUsed: The total amount of memory, in kilobytes, of used virtual address space.
        public ulong VmallocUsed;
        // VMallocChunk: The largest contiguous block of memory, in kilobytes, of available virtual address space.
        public ulong VmallocChunk;

        public ulong Percpu;
        public ulong HardwareCorrupted;
        public ulong AnonHugePages;
        public ulong ShmemHugePages;
        public ulong ShmemPmdMapped;
        public ulong CmaTotal;
        public ulong CmaFree;
        public ulong HugePages_Total;
        public ulong HugePages_Free;
        public ulong HugePages_Rsvd;
        public ulong HugePages_Surp;
        public ulong Hugepagesize;
        public ulong Hugetlb;
        public ulong DirectMap4k;
        public ulong DirectMap2M;
        public ulong DirectMap1G;


        // Pre: b >= 0 
        private static ulong Pow(ulong a, uint b)
        {
            ulong answer = 1;

            for (int i = 0; i < b; i++)
                answer *= a;

            return answer;
        }


        public LinuxMemoryMetrics()
        {
            FromMemInfo(this);
        }


        private void FromMemInfo(LinuxMemoryMetrics mem)
        {
            System.Type t = typeof(LinuxMemoryMetrics);

            System.Reflection.FieldInfo[] fis = t.GetFields();

            System.Collections.Generic.Dictionary<string, System.Reflection.FieldInfo> fields =
                new System.Collections.Generic.Dictionary<string, System.Reflection.FieldInfo>(
                    System.StringComparer.InvariantCultureIgnoreCase
            );

            System.Collections.Generic.Dictionary<string, ulong> units =
                new System.Collections.Generic.Dictionary<string, ulong>(
                    System.StringComparer.InvariantCultureIgnoreCase
            );


            for (int i = 0; i < fis.Length; ++i)
            {
                object[] attrs = fis[i].GetCustomAttributes(typeof(MemInfoNameAttribute), false);
                if (attrs != null && attrs.Length > 0)
                {
                    MemInfoNameAttribute a = (MemInfoNameAttribute)attrs[0];
                    fields.Add(a.Name, fis[i]);
                }
                else
                {
                    fields.Add(fis[i].Name, fis[i]);
                }
            } // Next i 


            for (uint i = 1; i < SizeSuffixes.Length; ++i)
            {
                units.Add(SizeSuffixes[i], Pow(1024, i));
            } // Next i 


            int counter = -1;
            string line;

            string memFile = "/proc/meminfo";

            if (!System.IO.File.Exists(memFile))
                memFile = "meminfo.txt";

            // Read the file and display it line by line.
            using (System.IO.StreamReader file = new System.IO.StreamReader(memFile))
            {

                while ((line = file.ReadLine()) != null)
                {
                    counter++;

                    if (line == null)
                        continue;

                    line = line.Trim();
                    if (line == string.Empty)
                        continue;

                    int iPos = line.IndexOf(':');
                    if (iPos == -1)
                        continue;

                    string strKey = line.Substring(0, iPos);
                    string strValue = line.Substring(iPos + 1);

                    if (strKey != null)
                        strKey = strKey.Trim();

                    if (strValue != null)
                        strValue = strValue.Trim();

                    ulong value = 0;
                    string strUnit = null;


                    System.Text.RegularExpressions.Match ma =
                        System.Text.RegularExpressions.Regex.Match(strValue, @"(\d+)\s*(.*)\s*");

                    if (ma.Success)
                    {
                        string strValueNumber = ma.Groups[1].Value;
                        ulong.TryParse(strValueNumber, out value);
                        strUnit = ma.Groups[2].Value;

                        if (!string.IsNullOrEmpty(strUnit) && units.ContainsKey(strUnit))
                        {
                            value *= units[strUnit];
                        }

                    } // End if (ma.Success)


                    //dr["i"] = counter;
                    //dr["Line"] = line;
                    //dr["Key"] = strKey;
                    //dr["Value"] = iAmount;
                    //dr["Unit"] = strUnit;

                    System.Reflection.FieldInfo fi = fields[strKey];
                    if (fi != null)
                        fi.SetValue(mem, value);
                    else
                        throw new System.NotSupportedException("Unknown field.");
                } // Whend 

                file.Close();
            } // End Using file 

        } // End Function FromMemInfo 


    } // End Class LinuxMemoryMetrics 


} // End Namespace RamMonitorPrototype 
