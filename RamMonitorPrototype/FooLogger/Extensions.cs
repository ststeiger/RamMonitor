﻿
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;

using Microsoft.Extensions.Options;


namespace RamMonitorPrototype
{


    public static class FileLoggerExtensions
    {


        public static Microsoft.Extensions.Logging.ILoggingBuilder AddFileLogger( 
              this Microsoft.Extensions.Logging.ILoggingBuilder builder
            , System.Action<FileLoggerOptions> configure)
        {
            builder.AddConfiguration();

            builder.Services.TryAddEnumerable(Microsoft.Extensions.DependencyInjection.ServiceDescriptor.Singleton<
                    Microsoft.Extensions.Logging.ILoggerProvider,
                    FooLoggerProvider
                >()
            );

            builder.Services.TryAddEnumerable(Microsoft.Extensions.DependencyInjection.ServiceDescriptor.Singleton
                <IConfigureOptions<FileLoggerOptions>, FileLoggerOptionsSetup>());

            builder.Services.TryAddEnumerable(
                Microsoft.Extensions.DependencyInjection.ServiceDescriptor.Singleton
                <
                    IOptionsChangeTokenSource<FileLoggerOptions>,
                    LoggerProviderOptionsChangeTokenSource<FileLoggerOptions
                    , FooLoggerProvider>
                >());

            builder.Services.Configure(configure);

            return builder;
        }


    }
}
