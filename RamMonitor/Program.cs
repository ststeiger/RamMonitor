
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace RamMonitor
{
    
    
    public class Program
    {
        
        
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        } // End Sub Main 
        
        
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            // return Host.CreateDefaultBuilder(args).ConfigureServices((hostContext, services) => { services.AddHostedService<Worker>(); });
            
            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(
                    delegate(HostBuilderContext hostingContext, IConfigurationBuilder config)
                    {
                        var builder = new ConfigurationBuilder()
                            .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                            .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                            .AddEnvironmentVariables()
                            .Build();
                    })
                .ConfigureServices(
                    delegate(HostBuilderContext hostingContext, IServiceCollection services) 
                    {
                        // AWSSDK.Extensions.NETCore.Setup
                        // AWSSDK.SQS
                        // Microsoft.VisualStudio.Azure.Containers.Tools.Targets
                        
                        // AWS Configuration
                        // var options = hostingContext.Configuration.GetAWSOptions();
                        // services.AddDefaultAWSOptions(options);
                        // services.AddAWSService<IAmazonSQS>();

                        // Worker Service
                        services.AddHostedService<Worker>();
                    }
                );
            
        } // End Function CreateHostBuilder 
        
        
    } // End Class Program 
    
    
} // End Namespace RamMonitor 
