
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


// http://www.dotnetspeak.com/net-core/creating-schedule-driven-windows-service-in-net-core-3-0/
// https://andrewlock.net/new-in-aspnetcore-3-structured-logging-for-startup-messages/
// https://andrewlock.net/suppressing-the-startup-and-shutdown-messages-in-asp-net-core/


// dotnet restore -r win-x86
// dotnet build -r win-x86
// dotnet publish -f netcoreapp3.1 -c Release -r win-x86
// dotnet publish -f netcoreapp3.1 -c Release -r win-x86 -o D:\inetpub\RamMonitor
// dotnet publish -f netcoreapp3.1 -c Release -r win-x86 -o D:\inetpub\RamMonitor
// dotnet publish -r linux-x64 -c Release /p:PublishSingleFile=true
// dotnet publish -r win-x86 -c Release /p:PublishSingleFile=true -o D:\inetpub\RamMonitor

namespace RamMonitor
{
    
    
    public class Worker 
        : Microsoft.Extensions.Hosting.BackgroundService
    {
        
        
        private readonly ILogger<Worker> m_logger;
        private IConfiguration m_configuration;
        private int m_maxLoadRatioBeforeKill;
        private int m_measureInterval;
        private bool m_ignoreChange;
        
        
        public Worker(ILogger<Worker> logger, IConfiguration conf)
        {
            this.m_logger = logger;
            this.m_configuration = conf;
            IConfigurationSection section = conf.GetSection("KillSettings");
            
            this.m_maxLoadRatioBeforeKill = section.TryGetValue<int>("KillChromeLoadRatio", 79);
            this.m_measureInterval = section.TryGetValue<int>("MeasureInterval", 5000);


            m_logger.LogInformation("Worker Constructor");
            m_logger.LogInformation("m_maxLoadRatioBeforeKill: {0}, m_measureInterval: {1}", this.m_maxLoadRatioBeforeKill, this.m_measureInterval);



            Microsoft.Extensions.Primitives.ChangeToken.OnChange(
                  delegate() { return this.m_configuration.GetReloadToken(); } 
                , delegate(IConfigurationSection state) { InvokeChanged(state); }
                , section
            );
            
        } // End Constructor 
        
        
        
        private async System.Threading.Tasks.Task IgnoreIrrelevantFileAttributeUpdate()
        {
            await System.Threading.Tasks.Task.Delay(1000);
            this.m_ignoreChange = false;
        }


        private void InvokeChanged(IConfigurationSection section)
        {
            if (this.m_ignoreChange)
                return;
            
            this.m_ignoreChange = true;
            
            this.m_maxLoadRatioBeforeKill = section.TryGetValue<int>("KillChromeLoadRatio", 79);
            this.m_measureInterval = section.TryGetValue<int>("MeasureInterval", 5000);

            m_logger.LogInformation("InvokeChanged");
            m_logger.LogInformation("m_maxLoadRatioBeforeKill: {0}, m_measureInterval: {1}", this.m_maxLoadRatioBeforeKill, this.m_measureInterval);


            // #pragma warning disable 1998
            IgnoreIrrelevantFileAttributeUpdate();
            // #pragma warning restore 1998
            
        }
        
        
        protected override async System.Threading.Tasks.Task ExecuteAsync(System.Threading.CancellationToken stoppingToken)
        {
            IConfigurationSection section = this.m_configuration.GetSection("KillSettings");
            CollectingTextWriter ctw = new CollectingTextWriter();
            
            while (!stoppingToken.IsCancellationRequested)
            {
                
#if true // for debugging appsettings.json update
                
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
#else
                System.Console.WriteLine(this.m_measureInterval);
                System.Console.WriteLine(this.m_maxLoadRatioBeforeKill);
#endif
                
                await System.Threading.Tasks.Task.Delay(this.m_measureInterval, stoppingToken);
            } // Whend 
            
        } // End Task ExecuteAsync 
        
        
    } // End Class Worker  
    
    
}  // End Namespace RamMonitor 
