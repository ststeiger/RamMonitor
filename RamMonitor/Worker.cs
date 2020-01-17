
using Microsoft.Extensions.Logging;
using RamMonitorPrototype;
using Quartz;

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
        
    }
    
    
    public class Job1 
        :  Quartz.IJob
    {
        public async System.Threading.Tasks.Task Execute( Quartz.IJobExecutionContext context)
        {
            await System.IO.File.AppendAllLinesAsync(@"c:\temp\job1.txt", new[] { System.DateTime.Now.ToLongTimeString() });
        }
    }
    
    public class Job2 
    :  Quartz.IJob
    {
        public async System.Threading.Tasks.Task Execute( Quartz.IJobExecutionContext context)
        {
            await System.IO.File.AppendAllLinesAsync(@"c:\temp\job2.txt", new[] { System.DateTime.Now.ToLongTimeString() });
        }
    }
    
    
    public class Worker 
        : Microsoft.Extensions.Hosting.BackgroundService
    {
        
        private readonly ILogger<Worker> m_logger;
        
        
        public Worker(ILogger<Worker> logger)
        {
            m_logger = logger;
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
                
                m_logger.LogInformation("Worker running at: {time}", System.DateTimeOffset.Now);
                m_logger.LogInformation(ctw.StringValue);
                
                
                await System.Threading.Tasks.Task.Delay(1000, stoppingToken);
            } // Whend 
        } // End Task ExecuteAsync 
        
        
        private Quartz.Impl.StdSchedulerFactory _schedulerFactory;
        private  Quartz.IScheduler _scheduler;
        private System.Threading.CancellationToken _stopppingToken;
        
        protected async System.Threading.Tasks.Task StartJobs()
        {
            _schedulerFactory = new Quartz.Impl.StdSchedulerFactory();
 
            _scheduler = await _schedulerFactory.GetScheduler();
            await _scheduler.Start();
            
            Quartz.IJobDetail job1 =  Quartz.JobBuilder.Create<Job1>()
                .WithIdentity("job1", "gtoup")
                .Build();
            
            Quartz.ITrigger trigger1 =  Quartz.TriggerBuilder.Create()
                .WithIdentity("trigger_10_sec", "group")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(10)
                    .RepeatForever())
                .Build();
            
            Quartz.IJobDetail job2 =  Quartz.JobBuilder.Create<Job2>()
                .WithIdentity("job2", "group")
                .Build();
            
            Quartz.ITrigger trigger2 =  Quartz.TriggerBuilder.Create()
                .WithIdentity("trigger_20_sec", "group")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(20)
                    .RepeatForever())
                .Build();
            
            await _scheduler.ScheduleJob(job1, trigger1, _stopppingToken);
            await _scheduler.ScheduleJob(job2, trigger2, _stopppingToken);
        }
        
        
    } // End Class Worker  
    
    
}  // End Namespace RamMonitor 
