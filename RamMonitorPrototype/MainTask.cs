
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Options;

namespace RamMonitorPrototype
{

    public class foo
        : Microsoft.Extensions.Hosting.IHostLifetime
    {
        Task IHostLifetime.StopAsync(CancellationToken cancellationToken)
        {
            TestMessage.MsgBox("StopAsync");
            return System.Threading.Tasks.Task.CompletedTask;
        }

        Task IHostLifetime.WaitForStartAsync(CancellationToken cancellationToken)
        {
            TestMessage.MsgBox("WaitForStartAsync");
            return System.Threading.Tasks.Task.CompletedTask;
        }
    }


    public class MainTask
    {


        // nuget add: Microsoft.Extensions.Hosting, System.ServiceProcess.ServiceController
        // https://www.stevejgordon.co.uk/running-net-core-generic-host-applications-as-a-windows-service
        // https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/windows-service?view=aspnetcore-2.2
        // sc create MyFileService binPath="D:\inetpub\MyService\LdapService.exe"
        // sc [<ServerName>] delete [<ServiceName>]
        // sc delete MyFileService
        public static async System.Threading.Tasks.Task Start(string[] args)
        {
            // https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/windows-service?view=aspnetcore-2.2&tabs=visual-studio
            bool isService = !(System.Diagnostics.Debugger.IsAttached || (System.Array.IndexOf(args, "--console") >= 0));



            // https://wakeupandcode.com/generic-host-builder-in-asp-net-core/
            Microsoft.Extensions.Hosting.IHostBuilder builder =
                new Microsoft.Extensions.Hosting.HostBuilder()
                    // .UseMyStartup(args)
                    // https://github.com/aspnet/AspNetCore.Docs/issues/9278
                    //.ConfigureHostConfiguration(configHost =>
                    .ConfigureHostConfiguration(delegate (IConfigurationBuilder configHost)
                    {
                        configHost.SetBasePath(System.IO.Directory.GetCurrentDirectory());
                        configHost.AddJsonFile("hostsettings.json", optional: true);
                        configHost.AddEnvironmentVariables(prefix: "PREFIX_");
                        configHost.AddCommandLine(args);
                    })
                    //.ConfigureAppConfiguration((hostContext, configApp) =>
                    .ConfigureAppConfiguration(delegate (HostBuilderContext hostContext, IConfigurationBuilder configApp)
                    {
                        string dir = System.IO.Path.GetDirectoryName(typeof(Program).Assembly.Location);
                        // dir = System.IO.Directory.GetCurrentDirectory();
                        configApp.SetBasePath(dir);

                        configApp.AddJsonFile("appsettings.json", optional: true);
                        configApp.AddJsonFile(
                            $"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json",
                            optional: true);
                        configApp.AddEnvironmentVariables(prefix: "PREFIX_");
                        configApp.AddCommandLine(args);
                    })
                    //.ConfigureLogging((hostContext, configLogging) =>
                    .ConfigureLogging(delegate (HostBuilderContext hostContext, ILoggingBuilder configLogging)
                    {
                        // https://www.codeproject.com/Articles/1556475/How-to-Write-a-Custom-Logging-Provider-in-ASP-NET

                        // string dir = System.IO.Path.GetDirectoryName(typeof(Program).Assembly.Location);
                        // string logDir = System.IO.Directory.GetCurrentDirectory();
                        string logDir = System.IO.Path.GetDirectoryName(typeof(Program).Assembly.Location);

                        logDir = System.IO.Path.Combine(logDir, "Log");
                        if (!System.IO.Directory.Exists(logDir))
                            System.IO.Directory.CreateDirectory(logDir);


                        configLogging.AddProvider(new FooLoggerProvider());


                        configLogging.AddFileLogger(
                            delegate (FileLoggerOptions fo)
                            {
                                // fo.Folder = logDir;
                                fo.LogLevel = LogLevel.Trace;
                            }
                        );
                        

                        configLogging.SetMinimumLevel(LogLevel.Information);
                        configLogging.AddFilter(x => x >= LogLevel.Trace);

                        // configLogging.AddConsole();
                        // configLogging.AddDebug();
                        // configLogging.AddEventSourceLogger();
                        // configLogging.AddEventLog();
                    })
                    .ConfigureServices(delegate (HostBuilderContext hostContext, IServiceCollection services)
                    {
                        // System.IServiceProvider isp = services.BuildServiceProvider();

                        services.Configure(
                            delegate(ConsoleLifetimeOptions options ) 
                            {
                                options.SuppressStatusMessages = true;
                            }
                        ); 

                        services.AddSingleton<Microsoft.Extensions.Hosting.IHostLifetime, foo>();
                        services.AddHostedService<TheService>();
                    }
            )
            .UseConsoleLifetime()
            // .UseServiceBaseLifetime()
            ;

            System.AppDomain.CurrentDomain.ProcessExit += OnProcessExit;


            if (isService)
            {
                await builder.RunAsServiceAsync();
            }
            else
            {
                await builder.RunConsoleAsync();
            }

            /*
            System.Console.WriteLine(System.Environment.NewLine);
            System.Console.WriteLine(" --- Press any key to continue --- ");
            System.Console.ReadKey();
            */
        } // End Sub MainTask 



        // https://andrewlock.net/introducing-ihostlifetime-and-untangling-the-generic-host-startup-interactions/
        public static void OnProcessExit(object sender, System.EventArgs e)
        {
        }


    }
    


}
