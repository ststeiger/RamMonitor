
using Microsoft.Extensions.DependencyInjection; // For AddSingleton 
using Microsoft.Extensions.Hosting; // For RunAsync


namespace RamMonitorPrototype
{


    public static class ServiceBaseLifetimeHostExtensions
    {


        public static Microsoft.Extensions.Hosting.IHostBuilder UseServiceBaseLifetime(
            this Microsoft.Extensions.Hosting.IHostBuilder hostBuilder)
        {
            return hostBuilder.ConfigureServices(
                (hostContext, services) => services.AddSingleton<Microsoft.Extensions.Hosting.IHostLifetime, ServiceBaseLifetime>()
            );
        }


        public static System.Threading.Tasks.Task RunAsServiceAsync(
              this Microsoft.Extensions.Hosting.IHostBuilder hostBuilder
            , System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            return hostBuilder.UseServiceBaseLifetime().Build().RunAsync(cancellationToken);
        }


    }


}
