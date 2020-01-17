
namespace RamMonitorPrototype
{


    public abstract class GlobalMemoryMetrics
    {

        public abstract ulong TotalRAM { get; }
        public abstract ulong FreeRAM { get; }

        public abstract ulong TotalSwap { get; }
        public abstract ulong FreeSwap { get; }


        public virtual ulong UsedRAM
        {
            get
            {
                return this.TotalRAM - this.FreeRAM;
            }
        }


        public virtual int Load
        {
            get
            {
                return (int)((this.UsedRAM * 100.0d) / (double)this.TotalRAM);
            }
        }
        


        public virtual ulong UsedSwap
        {
            get
            {
                return this.TotalSwap - this.FreeSwap;
            }
        }

        public virtual int SwapLoad
        {
            get
            {
                return (int)((this.UsedSwap * 100.0d) / (double)this.TotalSwap);
            }
        }


        public virtual HealthStatus_t RamHealthStatus
        {
            // https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.builder.healthcheckapplicationbuilderextensions.usehealthchecks?view=aspnetcore-3.1
            // https://gunnarpeipman.com/aspnet-core-memory-health-check/

            get
            {
                if (this.Load > 89)
                {
                    return HealthStatus_t.Unhealthy;
                }

                if (this.Load > 79)
                {
                    return HealthStatus_t.Degraded;
                }

                return HealthStatus_t.Healthy;
            }
        }


        public virtual HealthStatus_t SwapHealthStatus
        {
            // https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.builder.healthcheckapplicationbuilderextensions.usehealthchecks?view=aspnetcore-3.1
            // https://gunnarpeipman.com/aspnet-core-memory-health-check/

            get
            {
                if (this.SwapLoad > 89)
                {
                    return HealthStatus_t.Unhealthy;
                }

                if (this.SwapLoad > 79)
                {
                    return HealthStatus_t.Degraded;
                }

                return HealthStatus_t.Healthy;
            }
        } // End Property 





        protected readonly string[] SizeSuffixes =
              { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };


        protected string SizeSuffix(ulong value, int decimalPlaces = 1)
        {
            // if (value < 0) { return "-" + SizeSuffix(-value); }

            int i = 0;
            decimal dValue = (decimal)value;
            while (System.Math.Round(dValue, decimalPlaces) >= 1000)
            {
                dValue /= 1024;
                i++;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}", dValue, SizeSuffixes[i]);
        }

        protected string GetSize(ulong amount)
        {
            return "  ( " + SizeSuffix(amount) + " )";
        }


        public void WriteMemory(System.IO.TextWriter output)
        {
            {
                output.Write("Load:\t\t\t");
                output.Write(this.Load.ToString("N2"));
                output.WriteLine("%");
            }

            System.Type t = this.GetType();

            System.Reflection.FieldInfo[] fis = t.GetFields();

            for (int i = 0; i < fis.Length; ++i)
            {
                ulong value = (ulong)fis[i].GetValue(this);
                output.Write(fis[i].Name + ":\t\t\t");
                // output.Write(value.ToString("N0"));
                output.WriteLine(GetSize(value));
            }

        }


        public static GlobalMemoryMetrics Instance
        {
            get
            {
                GlobalMemoryMetrics metrics = null;

                // X-Box supports GlobalMemoryStatusEx 
                // https://github.com/microsoft/Xbox-ATG-Samples/blob/master/UWPSamples/System/SystemInfoUWP/SystemInfo.cpp
                if (System.Environment.OSVersion.Platform != System.PlatformID.Unix)
                {
                    metrics = new WindowsMemoryMetrics();
                }
                else if (System.IO.File.Exists("/proc/meminfo"))
                {
                    metrics = new LinuxMemoryMetrics();
                }
                else
                {
                    throw new System.NotSupportedException("GetMetrics not supported for current operating system.");
                }

                return metrics;
            }
        } // End Property Instance 


    }

}