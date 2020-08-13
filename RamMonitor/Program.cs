
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
        private static Logging.TrivialLogger s_logger;


        static Program()
        {
            // s_logger = new Logging.FileLogger(@"D:\IDGLog.txt");
            s_logger = new Logging.HtmlLogger(@"D:\IDGLog.htm");
        }


        public static void Main(string[] args)
        {
            try
            {
                CreateHostBuilder(args).Build().Run();
            }
            catch (System.Exception ex)
            {
                s_logger.Log(Logging.LogLevel_t.Configuration, "EXXX: {0}", ex);

                System.Console.WriteLine(System.Environment.NewLine);
                System.Console.WriteLine(System.Environment.NewLine);

                System.Console.WriteLine(ex.Message);
                System.Console.WriteLine(ex.StackTrace);
                System.Console.WriteLine(System.Environment.NewLine);
                System.Console.WriteLine(System.Environment.NewLine);
                System.Environment.Exit(ex.HResult);
            }

        } // End Sub Main 


        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            IHostBuilder builder = new HostBuilder();

            try
            {

                // builder.UseContentRoot(System.IO.Directory.GetCurrentDirectory());
                // builder.UseContentRoot(System.IO.Path.GetDirectoryName(typeof(Program).Assembly.Location));
                // builder.UseContentRoot(System.AppContext.BaseDirectory);


                string programDirectory = System.IO.Path.GetDirectoryName(typeof(Program).Assembly.Location);
                string curDir = System.IO.Directory.GetCurrentDirectory();
                string baseDir = System.AppDomain.CurrentDomain.BaseDirectory;
                string executablePath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
                string executable = System.IO.Path.GetFileNameWithoutExtension(executablePath);


                s_logger.Log(Logging.LogLevel_t.Information, "Program Directory: {0}", programDirectory);
                s_logger.Log(Logging.LogLevel_t.Information, "Current Directory: {0}", curDir);
                s_logger.Log(Logging.LogLevel_t.Information, "Base Directory: {0}", baseDir);
                s_logger.Log(Logging.LogLevel_t.Information, "Executable Path: {0}", executablePath);
                s_logger.Log(Logging.LogLevel_t.Information, "Executable: {0}", executable);


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
                    delegate (IConfigurationBuilder config)
                    {
                        // string bd = System.AppDomain.CurrentDomain.BaseDirectory;
                        // string bd = @"C:\";
                        // config.SetBasePath(bd);

                        config.AddEnvironmentVariables(prefix: "DOTNET_");
                        if (args != null)
                        {
                            config.AddCommandLine(args);
                        } // End if (args != null) 

                    } // End Delegate 
                );

                builder.ConfigureAppConfiguration(
                        delegate (HostBuilderContext hostingContext, IConfigurationBuilder config)
                        {
                            IHostEnvironment env = hostingContext.HostingEnvironment;

                            config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

                            if (env.IsDevelopment() && !string.IsNullOrEmpty(env.ApplicationName))
                            {
                                System.Reflection.Assembly appAssembly =
                                    System.Reflection.Assembly.Load(new System.Reflection.AssemblyName(env.ApplicationName));

                                if (appAssembly != null)
                                {
                                    config.AddUserSecrets(appAssembly, optional: true);
                                } // End if (appAssembly != null) 

                            } // End if (env.IsDevelopment() && !string.IsNullOrEmpty(env.ApplicationName))

                            config.AddEnvironmentVariables();

                            if (args != null)
                            {
                                config.AddCommandLine(args);
                            } // End if (args != null) 

                        } // End Delegate 
                    )
                    .ConfigureLogging(
                        delegate (HostBuilderContext hostingContext, ILoggingBuilder logging)
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

                            // logging.ClearProviders();
                            logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));

                            // https://www.codeproject.com/Articles/5255953/Use-Trace-and-TraceSource-in-NET-Core-Logging
                            // https://github.com/nreco/logging/tree/master/src/NReco.Logging.File
                            // https://github.com/adams85/filelogger
                            // https://docs.microsoft.com/en-us/dotnet/framework/debug-trace-profile/tracing-and-instrumenting-applications
                            logging.AddFileLogger(
                                delegate (FileLoggerOptions options)
                                {
                                    string postFix = System.DateTime.Now.ToString("yyyy'-'MM'-'dd'_'HH'-'mm'-'ss");

                                    options.LogFilePath = @"D:\LogFile_RamMonitor_"+ postFix + ".txt";
                                    options.LogLevel = LogLevel.Information;
                                }
                            );

                            logging.AddConsole();
                            logging.AddDebug();
                            logging.AddEventSourceLogger(); // is for Event Tracing.
                            // this uses Event Tracing for Windows (ETW)
                            // https://docs.microsoft.com/en-us/windows/win32/etw/event-tracing-portal

                            if (isWindows)
                            {
                                // Add the EventLogLoggerProvider on windows machines
                                logging.AddEventLog(); // this is for Windows Event Log 
                            } // End if (isWindows) 

                        } // End Delegate 
                    )
                    .ConfigureServices(
                        delegate (HostBuilderContext hostingContext, IServiceCollection services)
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
                        } // End Delegate 
                    )
                    .UseDefaultServiceProvider(
                        delegate (HostBuilderContext hostingContext, ServiceProviderOptions options)
                        {
                            bool isDevelopment = hostingContext.HostingEnvironment.IsDevelopment();
                            options.ValidateScopes = isDevelopment;
                            options.ValidateOnBuild = isDevelopment;
                        } // End Delegate 
                    );

                // throw new System.Exception("wade", new System.Exception("hade"));
            }
            catch (System.Exception ex)
            {
                s_logger.Log(Logging.LogLevel_t.Configuration, "EXXX: {0}", ex);

                System.Console.WriteLine(ex.Message);
                System.Console.WriteLine(ex.StackTrace);
                System.Console.WriteLine();
                System.Console.WriteLine(System.Environment.NewLine);
                System.Environment.Exit(1);
            }

            return builder;
        } // End Function CreateHostBuilder 


        private static IHostBuilder OldCreateHostBuilder(string[] args)
        {
            // return Host.CreateDefaultBuilder(args).ConfigureServices((hostContext, services) => { services.AddHostedService<Worker>(); });

            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(
                    delegate (HostBuilderContext hostingContext, IConfigurationBuilder config)
                    {
                        IConfigurationRoot builder = new ConfigurationBuilder()
                            .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                            .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json",
                                optional: true, reloadOnChange: true)
                            .AddEnvironmentVariables()
                            .Build();
                    } // End Delegate 
                )
                .ConfigureServices(
                    delegate (HostBuilderContext hostingContext, IServiceCollection services)
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
                    } // End Delegate 
                );
        } // End Function OldCreateHostBuilder 


    } // End Class Program 


} // End Namespace RamMonitor 
