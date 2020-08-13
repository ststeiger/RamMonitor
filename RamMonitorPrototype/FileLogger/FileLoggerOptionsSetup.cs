
namespace RamMonitorPrototype
{


    internal class FileLoggerOptionsSetup
        : Microsoft.Extensions.Options.ConfigureFromConfigurationOptions<FileLoggerOptions>
    {

        public FileLoggerOptionsSetup(
            Microsoft.Extensions.Logging.Configuration.ILoggerProviderConfiguration<FileLoggerProvider>
            providerConfiguration
        )
            : base(providerConfiguration.Configuration)
        { }

    }


}
