
using RamMonitorPrototype;
using Microsoft.Extensions.Logging;


// http://www.dotnetspeak.com/net-core/creating-schedule-driven-windows-service-in-net-core-3-0/
// https://andrewlock.net/new-in-aspnetcore-3-structured-logging-for-startup-messages/
// https://andrewlock.net/suppressing-the-startup-and-shutdown-messages-in-asp-net-core/
namespace RamMonitor
{
    
    
    public class CollectingTextWriter
        : System.IO.TextWriter
    {

        protected System.Text.StringBuilder m_sb;
        
        
        public string StringValue
        {
            get
            {
                string ret = this.m_sb.ToString();
                this.m_sb.Clear();
                return ret;
            }
        }
        
        
        public override System.Text.Encoding Encoding
        {
            get { return System.Text.Encoding.Default; }
        }
        

        public CollectingTextWriter()
        {
            this.m_sb = new System.Text.StringBuilder();
        }
        
        ~CollectingTextWriter()  
        {
            this.m_sb.Clear();
            this.m_sb = null;
        } // Destructor 
        
        
        public override void Write(char value)
        {
            this.m_sb.Append(value);
        }
        
    } // End Class CollectingTextWriter
    
    
    public class Worker 
        : Microsoft.Extensions.Hosting.BackgroundService
    {
        
        private readonly ILogger<Worker> m_logger;
        
        
        public Worker(ILogger<Worker> logger)
        {
            this.m_logger = logger;
        } // End Constructor 
        
        
        protected override async System.Threading.Tasks.Task ExecuteAsync(System.Threading.CancellationToken stoppingToken)
        {
            CollectingTextWriter ctw = new CollectingTextWriter();
            
            while (!stoppingToken.IsCancellationRequested)
            {
                GlobalMemoryMetrics metrics = OsInfo.MemoryMetrics;
                System.Console.WriteLine(metrics.Load);
                System.Console.WriteLine(metrics.SwapLoad);
                System.Console.WriteLine("Dump:");
                metrics.WriteMemory(ctw);
                
                this.m_logger.LogInformation("Worker running at: {time}", System.DateTimeOffset.Now);
                this.m_logger.LogInformation(ctw.StringValue);
                
                await System.Threading.Tasks.Task.Delay(1000, stoppingToken);
            } // Whend 
            
        } // End Task ExecuteAsync 
        
        
    } // End Class Worker  
    
    
}  // End Namespace RamMonitor 
