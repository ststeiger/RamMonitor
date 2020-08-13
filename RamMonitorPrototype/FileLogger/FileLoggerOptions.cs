
namespace RamMonitorPrototype
{

    public class FileLoggerOptions
    {
        public FileLoggerOptions()
        { }

        public Microsoft.Extensions.Logging.LogLevel LogLevel { get; set; } =
            Microsoft.Extensions.Logging.LogLevel.Information;

    }

}
