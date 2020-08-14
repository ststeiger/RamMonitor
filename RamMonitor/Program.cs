
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

        private static string s_ProgramDirectory;
        private static string s_CurrentDirectory;
        private static string s_BaseDirectory;
        private static string s_ExecutablePath;
        private static string s_ExecutableDirectory;
        private static string s_Executable;
        private static string s_ContentRootDirectory;


        private static void DisplayError(System.Exception ex)
        {
            System.Console.WriteLine(System.Environment.NewLine);
            System.Console.WriteLine(System.Environment.NewLine);
                
            System.Exception thisError = ex;
            while (thisError != null)
            {
                System.Console.WriteLine(thisError.Message);
                System.Console.WriteLine(thisError.StackTrace);
                   
                if (thisError.InnerException != null)
                {
                    System.Console.WriteLine(System.Environment.NewLine);
                    System.Console.WriteLine("Inner Exception:");
                } // End if (thisError.InnerException != null) 
                
                thisError = thisError.InnerException;
            } // Whend 
                
            System.Console.WriteLine(System.Environment.NewLine);
            System.Console.WriteLine(System.Environment.NewLine);
        } // End Sub DisplayError 


        static Program()
        {
            try
            {
                s_ProgramDirectory = System.IO.Path.GetDirectoryName(typeof(Program).Assembly.Location);
                s_CurrentDirectory = System.IO.Directory.GetCurrentDirectory();
                s_BaseDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
                s_ExecutablePath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
                s_ExecutableDirectory = System.IO.Path.GetDirectoryName(s_ExecutablePath);
                s_Executable = System.IO.Path.GetFileNameWithoutExtension(s_ExecutablePath);
                
                string logFilePath = null;
                string fileName = @"ServiceStartupLog.htm";
                
                if ("dotnet".Equals(s_Executable, System.StringComparison.InvariantCultureIgnoreCase))
                {
                    s_ContentRootDirectory = s_ProgramDirectory;
                    logFilePath = System.IO.Path.Combine(s_ProgramDirectory, fileName);
                }
                else
                {
                    s_ContentRootDirectory = s_ExecutableDirectory;
                    logFilePath = System.IO.Path.Combine(s_ExecutableDirectory, fileName);
                }

                if (System.IO.File.Exists(logFilePath))
                    System.IO.File.Delete(logFilePath);

                // s_logger = new Logging.HtmlLogger(@"D:\IDGLog.htm");
                s_logger = new Logging.HtmlLogger(logFilePath);
                
                s_logger.Log(Logging.LogLevel_t.Information, "Program Directory: {0}", s_ProgramDirectory);
                s_logger.Log(Logging.LogLevel_t.Information, "Current Directory: {0}", s_CurrentDirectory);
                s_logger.Log(Logging.LogLevel_t.Information, "Base Directory: {0}", s_BaseDirectory);
                s_logger.Log(Logging.LogLevel_t.Information, "Logfile Directory: {0}", s_ContentRootDirectory);
                s_logger.Log(Logging.LogLevel_t.Information, "Executable Path: {0}", s_ExecutablePath);
                s_logger.Log(Logging.LogLevel_t.Information, "Executable Directory: {0}", s_ExecutableDirectory);
                s_logger.Log(Logging.LogLevel_t.Information, "Executable: {0}", s_Executable);
            } // End Try 
            catch (System.Exception ex)
            {
                DisplayError(ex);
                System.Environment.Exit(ex.HResult);
            } // End Catch 
            
        } // End Static Constructor 
        
        
        public static void Main(string[] args)
        {
            
            try
            {
                CreateHostBuilder(args).Build().Run();
            }
            catch (System.Exception ex)
            {
                DisplayError(ex);
                s_logger.Log(Logging.LogLevel_t.Configuration, "Configuration Error", ex);
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

                builder.UseContentRoot(s_ContentRootDirectory);

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
                else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.OSX))
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
                        // Has no effect ... 
                        // s_logger.Log(Logging.LogLevel_t.Information, "SetBasePath: {0}", s_ExecutableDirectory);
                        // config.SetBasePath(s_ExecutableDirectory);
                        
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
                            // Completely wrong ... 
                            // s_logger.Log(Logging.LogLevel_t.Information, "ContentRootPath: {0}", env.ContentRootPath);
                            
                            string appSettingsFile = System.IO.Path.Combine(s_ContentRootDirectory, "appsettings.json");
                            
                            // config.AddJsonFile("appsettings.json"
                            config.AddJsonFile(appSettingsFile, optional: false, reloadOnChange: true)
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

#if false
                            // https://www.codeproject.com/Articles/5255953/Use-Trace-and-TraceSource-in-NET-Core-Logging
                            // https://github.com/nreco/logging/tree/master/src/NReco.Logging.File
                            // https://github.com/adams85/filelogger
                            // https://docs.microsoft.com/en-us/dotnet/framework/debug-trace-profile/tracing-and-instrumenting-applications
                            // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-3.1
                            // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-3.1#azure-application-insights
                            logging.AddFileLogger(
                                delegate (FileLoggerOptions options)
                                {
                                    options.LogLevel = LogLevel.Information;
                                    options.LogFilePath = System.IO.Path.Combine(s_ContentRootDirectory, @"RamMonitor.log.txt");
                                    if (System.IO.File.Exists(options.LogFilePath))
                                        System.IO.File.Delete(options.LogFilePath);
                                }
                            );
#endif

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
