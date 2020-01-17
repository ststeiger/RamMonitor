
namespace RamMonitorPrototype
{


    internal class FileLoggerOptionsSetup
        : Microsoft.Extensions.Options.ConfigureFromConfigurationOptions<FileLoggerOptions>
    {

        public FileLoggerOptionsSetup(
            Microsoft.Extensions.Logging.Configuration.ILoggerProviderConfiguration<FooLoggerProvider>
            providerConfiguration
        )
            : base(providerConfiguration.Configuration)
        { }

    }


}
