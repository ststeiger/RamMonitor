
using System.Collections.Generic;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.EventLog;


namespace RamMonitor
{
    
    
            public static class Program
    {
        

        public static void Main(string[] args) 
        {
            CreateHostBuilder(args).Build().Run();
        } // End Sub Main 



        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            IHostBuilder builder = new HostBuilder();

            builder.UseContentRoot(System.IO.Directory.GetCurrentDirectory());
            builder.ConfigureHostConfiguration(config =>
            {    
                config.AddEnvironmentVariables(prefix: "DOTNET_");
                if (args != null)
                {
                    config.AddCommandLine(args);
                }
            });

            builder.ConfigureAppConfiguration((hostingContext, config) =>
            {
                IHostEnvironment env = hostingContext.HostingEnvironment;

                config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                      .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

                if (env.IsDevelopment() && !string.IsNullOrEmpty(env.ApplicationName))
                {
                    System.Reflection.Assembly appAssembly = System.Reflection.Assembly.Load(new System.Reflection.AssemblyName(env.ApplicationName));
                    if (appAssembly != null)
                    {
                        config.AddUserSecrets(appAssembly, optional: true);
                    }
                }

                config.AddEnvironmentVariables();

                if (args != null)
                {
                    config.AddCommandLine(args);
                }
            })
            .ConfigureLogging((hostingContext, logging) =>
            {
                bool isWindows = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows);

                // IMPORTANT: This needs to be added *before* configuration is loaded, this lets
                // the defaults be overridden by the configuration.
                if (isWindows)
                {
                    // Default the EventLogLoggerProvider to warning or above
                    logging.AddFilter<EventLogLoggerProvider>(level => level >= LogLevel.Warning);
                }

                logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                logging.AddConsole();
                logging.AddDebug();
                logging.AddEventSourceLogger();

                if (isWindows)
                {
                    // Add the EventLogLoggerProvider on windows machines
                    logging.AddEventLog();
                }
            })
            .ConfigureServices((hostingContext, services) =>
            {
                // AWSSDK.Extensions.NETCore.Setup
                // AWSSDK.SQS
                // Microsoft.VisualStudio.Azure.Containers.Tools.Targets

                // AWS Configuration
                // AWSOptions options = hostingContext.Configuration.GetAWSOptions();
                // services.AddDefaultAWSOptions(options);
                // services.AddAWSService<IAmazonSQS>();

                // Worker Service
                services.AddHostedService<Worker>();
            })
            .UseDefaultServiceProvider((context, options) =>
            {
                bool isDevelopment = context.HostingEnvironment.IsDevelopment();
                options.ValidateScopes = isDevelopment;
                options.ValidateOnBuild = isDevelopment;
            });

            return builder;
        }
        
        
        private static IHostBuilder OldCreateHostBuilder(string[] args)
        {
            // return Host.CreateDefaultBuilder(args).ConfigureServices((hostContext, services) => { services.AddHostedService<Worker>(); });
            
            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(
                    delegate(HostBuilderContext hostingContext, IConfigurationBuilder config)
                    {
                        IConfigurationRoot builder = new ConfigurationBuilder()
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
                        // AWSOptions options = hostingContext.Configuration.GetAWSOptions();
                        // services.AddDefaultAWSOptions(options);
                        // services.AddAWSService<IAmazonSQS>();

                        // Worker Service
                        services.AddHostedService<Worker>();
                    }
                );

        } // End Function OldCreateHostBuilder 
        
        
    } // End Class Program 
    
    
} // End Namespace RamMonitor 
