
namespace RamMonitorPrototype
{


    public enum HealthStatus_t
    {
        Healthy,
        Degraded,
        Unhealthy
    }


    public class SystemMemoryMetrics
    {
        public ulong TotalPhys;
        public ulong AvailPhys;
        public ulong TotalPageFile;
        public ulong AvailPageFile;
        public ulong TotalVirtual;
        public ulong AvailVirtual;
        public ulong AvailExtendedVirtual;


        public double Total;
        public uint Used;
        public double Free;


        public HealthStatus_t HealthStatus
        {
            // https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.builder.healthcheckapplicationbuilderextensions.usehealthchecks?view=aspnetcore-3.1
            // https://gunnarpeipman.com/aspnet-core-memory-health-check/

            get
            {
                if (this.Used > 89)
                {
                    return HealthStatus_t.Unhealthy;
                }

                if (this.Used > 79)
                {
                    return HealthStatus_t.Degraded;
                }

                return HealthStatus_t.Healthy;
            }
        }

    }


}
