
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


// http://www.dotnetspeak.com/net-core/creating-schedule-driven-windows-service-in-net-core-3-0/
// https://andrewlock.net/new-in-aspnetcore-3-structured-logging-for-startup-messages/
// https://andrewlock.net/suppressing-the-startup-and-shutdown-messages-in-asp-net-core/


// dotnet restore -r win-x86
// dotnet build -r win-x86
// dotnet publish -f netcoreapp3.1 -c Release -r win-x86
// dotnet publish -f netcoreapp3.1 -c Release -r win-x86 -o D:\inetpub\RamMonitor


namespace RamMonitor
{
    
    
    public class Worker 
        : Microsoft.Extensions.Hosting.BackgroundService
    {
        
        private readonly ILogger<Worker> m_logger;
        private readonly int m_maxLoadRatioBeforeKill;
        private readonly int m_measureInterval;
        
        
        public Worker(ILogger<Worker> logger, IConfiguration conf)
        {
            IConfigurationSection section = conf.GetSection("KillSettings");
            
            this.m_maxLoadRatioBeforeKill = section.TryGetValue<int>("KillChromeLoadRatio", 79);
            this.m_measureInterval = section.TryGetValue<int>("MeasureInterval", 5000);
            
            this.m_logger = logger;
        } // End Constructor 
        
        
        protected override async System.Threading.Tasks.Task ExecuteAsync(System.Threading.CancellationToken stoppingToken)
        {
            CollectingTextWriter ctw = new CollectingTextWriter();
            
            while (!stoppingToken.IsCancellationRequested)
            {
                GlobalMemoryMetrics metrics = OsInfo.MemoryMetrics;
                metrics.WriteMemory(ctw);
                
                if (metrics.Load > this.m_maxLoadRatioBeforeKill)
                {
                    this.m_logger.LogInformation("Killing chrome processes at: {time}", System.DateTimeOffset.Now);
                    ProcessManager.KillProcessGroup("chrome");
                    this.m_logger.LogInformation("Killed chrome processes at: {time}", System.DateTimeOffset.Now);
                } // End if (metrics.Load > this.m_maxLoadRatioBeforeKill)
                
                this.m_logger.LogInformation("Worker running at: {time}", System.DateTimeOffset.Now);
                this.m_logger.LogInformation(ctw.StringValue);
                // KillChromeLoadRatio
                await System.Threading.Tasks.Task.Delay(this.m_measureInterval, stoppingToken);
            } // Whend 
            
        } // End Task ExecuteAsync 
        
        
    } // End Class Worker  
    
    
}  // End Namespace RamMonitor 
