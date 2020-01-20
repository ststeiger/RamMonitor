
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.EventLog;



namespace RamMonitor
{
    
    
    public class Program
    {

        
        public static uint ConvertFromIpAddressToInteger(string ipAddress)
        {
            var address = System.Net.IPAddress.Parse(ipAddress);
            byte[] bytes = address.GetAddressBytes();

            // flip big-endian(network order) to little-endian
            if (System.BitConverter.IsLittleEndian)
            {
                System.Array.Reverse(bytes);
            }

            return System.BitConverter.ToUInt32(bytes, 0);
        }
        
        public static string ConvertFromIntegerToIpAddress(uint ipAddress)
        {
            byte[] bytes = System.BitConverter.GetBytes(ipAddress);

            // flip little-endian to big-endian(network order)
            if (System.BitConverter.IsLittleEndian)
            {
                System.Array.Reverse(bytes);
            }

            // IP addresses are in network order (big-endian), while ints are little-endian on Windows,
            // so to get a correct value, you must reverse the bytes before converting on a little-endian system.
            
            return new System.Net.IPAddress(bytes).ToString();
        }
        
        public static void Ip2Num(int first, int second, int third, int fourth)
        {
            // int num =  (first << 24) | (second << 16) | (third << 8) | (fourth);
            int num =  (fourth << 24) | (third << 16) | (second << 8) | (first);

            long numIP = System.Net.IPAddress.Parse($"{first}.{second}.{third}.{fourth}").Address;

            // Here's a pair of methods to convert from IPv4 to a correct integer and back:
            uint foo = ConvertFromIpAddressToInteger($"{first}.{second}.{third}.{fourth}");
            
            
            System.Console.WriteLine(num);
            System.Console.WriteLine(numIP);
            System.Console.WriteLine(foo);
        }
        
        
        public static void Main(string[] args)
        {
            // Ip2Num(127, 0, 0, 1);
            CreateHostBuilder(args).Build().Run();
        } // End Sub Main 
        
        
        public static IHostBuilder CreateHostBuilder(string[] args)
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
        
        
        public static IHostBuilder OldCreateHostBuilder(string[] args)
        {
            // return Host.CreateDefaultBuilder(args).ConfigureServices((hostContext, services) => { services.AddHostedService<Worker>(); });
            
            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    IConfigurationRoot builder = new ConfigurationBuilder()
                        .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", optional: false, reloadOnChange: true)
                        .AddEnvironmentVariables()
                        .Build();
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
                });
            
        } // End Function CreateHostBuilder 
        
        
    } // End Class Program 
    
    
} // End Namespace RamMonitor 
