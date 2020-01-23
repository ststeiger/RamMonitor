
using System.Collections.Generic;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.EventLog;


namespace RamMonitor
{
    
    public interface IBaseDAL
    {
        public string Con { get; set; }
    }
    
    
    public interface IReadDal
        :IBaseDAL
    {

        public string GetData()
        {
            System.Console.Write("Get Data: ");
            return "abc";
        }

    }
    
    public interface IWriteDal
        :IBaseDAL
    {

        public void WriteData(string str)
        {
            System.Console.WriteLine("Write Data: " + str);
        }

    }

    public interface IReadWriteDal
        : IReadDal, IWriteDal
    {
    }


    public static class mmmm
    {
        
        public static void Log(this IWriteDal foo, System.Exception ex)
        {
            foo.WriteData( ex.ToString());        
        }
        
    }

    public class DalFields
    {
        protected string m_con;
        protected string m_ReadCon;
        protected string m_WriteCon;
        
        
        public string Con 
        {
            get { return m_con;}
            set {
                m_con = value;
            }
        }
        
    }
    
    public class MsSqlReadWriteDal
        :DalFields, IReadWriteDal
    {
        
        public MsSqlReadWriteDal()
        {
            this.m_con ="Hi";
            this.m_ReadCon ="Helloo";
            this.m_WriteCon ="Howdy";
        }

        // string IBaseDAL.Con { get; set; }
    }
    
    
    public static class Program
    {
        
        
        public static void FactoryTest()
        {
            MsSqlReadWriteDal omg = new MsSqlReadWriteDal();
            
            
            // System.Console.WriteLine(omg.Con);
            System.Console.WriteLine(((IReadDal)omg).Con);
            System.Console.WriteLine(((IWriteDal)omg).Con);
            
            System.Console.WriteLine(((IReadDal)omg).GetData());
            ((IWriteDal)omg).WriteData("foobar");
            
            System.Console.WriteLine(((IReadWriteDal)omg).GetData());
            ((IReadWriteDal)omg).WriteData("foobar");
            
            
            MsSqlReadWriteDal omg2 = (MsSqlReadWriteDal) ((IReadWriteDal)omg);
            System.Console.WriteLine("omg2: ");
            System.Console.WriteLine(omg2.Con);
            
            // ((IReadWriteDal)omg).Log(1, "Hello");
            System.Console.ReadKey();
        }


        public static void Main(string[] args)
        {
            FactoryTest();
            
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
