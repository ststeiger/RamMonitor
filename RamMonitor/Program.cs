
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.EventLog;


// Add package Microsoft.Extensions.Hosting.WindowsServices
// version 3.1.0


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
            
            // builder.UseContentRoot(System.IO.Directory.GetCurrentDirectory());
            // builder.UseContentRoot(System.IO.Path.GetDirectoryName(typeof(Program).Assembly.Location));
            // builder.UseContentRoot(System.AppContext.BaseDirectory);

            string executablePath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            string executable = System.IO.Path.GetFileNameWithoutExtension(executablePath);

            System.Console.WriteLine(System.Environment.NewLine);
            System.Console.Write("Executable Path: ");
            System.Console.WriteLine(executablePath);
            System.Console.WriteLine("BaseDirectory: ");
            System.Console.WriteLine(System.AppDomain.CurrentDomain.BaseDirectory);
            System.Console.WriteLine(System.Environment.NewLine);
            System.Environment.Exit(1);


            if ("dotnet".Equals(executable, System.StringComparison.InvariantCultureIgnoreCase))
            {
                builder.UseContentRoot(System.IO.Path.GetDirectoryName(typeof(Program).Assembly.Location));
            }
            else
            {
                builder.UseContentRoot(System.IO.Path.GetDirectoryName(executablePath));   
            }
            
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
            {
                // https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/windows-service?view=aspnetcore-3.1&tabs=visual-studio#app-configuration
                // Requires Microsoft.Extensions.Hosting.WindowsServices
                builder.UseWindowsService();
            } 
            else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux))
            {
                // https://devblogs.microsoft.com/dotnet/net-core-and-systemd/
                // Requires Microsoft.Extensions.Hosting.WindowsServices
                builder.UseSystemd(); // Add: Microsoft.Extensions.Hosting.Systemd
            }
            else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices
                .OSPlatform.OSX))
            {
                throw new System.NotImplementedException("Service for OSX Platform is NOT implemented.");
            }
            else
            {
                throw new System.NotSupportedException("This Platform is NOT supported.");
            }

            builder.ConfigureHostConfiguration(
                delegate(IConfigurationBuilder config)
                {
                    // string bd = System.AppDomain.CurrentDomain.BaseDirectory;
                    // config.SetBasePath(bd);

                    config.AddEnvironmentVariables(prefix: "DOTNET_");
                    if (args != null)
                    {
                        config.AddCommandLine(args);
                    }
                }
            );

            builder.ConfigureAppConfiguration(
                    delegate (HostBuilderContext hostingContext, IConfigurationBuilder config)
                    {
                        IHostEnvironment env = hostingContext.HostingEnvironment;

                        config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

                        if (env.IsDevelopment() && !string.IsNullOrEmpty(env.ApplicationName))
                        {
                            System.Reflection.Assembly appAssembly =
                                System.Reflection.Assembly.Load(new System.Reflection.AssemblyName(env.ApplicationName));
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
                    }
                )
                .ConfigureLogging(
                    delegate(HostBuilderContext hostingContext, ILoggingBuilder logging)
                    {
                        
                        bool isWindows = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(
                                System.Runtime.InteropServices.OSPlatform.Windows
                        );
                    
                        // IMPORTANT: This needs to be added *before* configuration is loaded, this lets
                        // the defaults be overridden by the configuration.
                        if (isWindows)
                        {
                            // Default the EventLogLoggerProvider to warning or above
                            logging.AddFilter<EventLogLoggerProvider>(level => level >= LogLevel.Warning);
                        } // End if (isWindows) 
                    
                        logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                        logging.AddConsole();
                        logging.AddDebug();
                        logging.AddEventSourceLogger();

                        if (isWindows)
                        {
                            // Add the EventLogLoggerProvider on windows machines
                            logging.AddEventLog();
                        } // End if (isWindows) 
                    }
                )
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
                )
                .UseDefaultServiceProvider(
                    delegate(HostBuilderContext hostingContext, ServiceProviderOptions options)
                    {
                        bool isDevelopment = hostingContext.HostingEnvironment.IsDevelopment();
                        options.ValidateScopes = isDevelopment;
                        options.ValidateOnBuild = isDevelopment;
                    }
                );

            return builder;
        } // End Function CreateHostBuilder 
        
        
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
                            .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json",
                                optional: true, reloadOnChange: true)
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
