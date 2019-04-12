using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using Serilog.AspNetCore;
using Steeltoe.Extensions.Logging;
using System;
using System.Linq;

namespace Dell.Infrastructure.Logging
{
    public static class LoggingBuilderExtensions
    {
        public static ILoggingBuilder AddSerilogDynamicConsole(this ILoggingBuilder builder)
        {
            // these lines come from SerilogWebHostBuilderExtensions.UseSerilog(this IWebHostBuilder builder, Serilog.ILogger logger = null, bool dispose = false)
            builder.Services.AddSingleton<ILoggerFactory>(services => new SerilogLoggerFactory(logger: null, dispose: false));

            // these lines come from DynamicLoggingBuilder.AddDynamicConsole(this ILoggingBuilder builder)
            //builder.AddFilter<DynamicLoggerProvider>(null, LogLevel.Trace);
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, SerilogDynamicLoggerProvider>());
            //builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureOptions<ConsoleLoggerOptions>, ConsoleLoggerOptionsSetup>());
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IOptionsChangeTokenSource<ConsoleLoggerOptions>, LoggerProviderOptionsChangeTokenSource<ConsoleLoggerOptions, ConsoleLoggerProvider>>());
            builder.Services.AddSingleton<IDynamicLoggerProvider>((p) => p.GetServices<ILoggerProvider>().OfType<IDynamicLoggerProvider>().SingleOrDefault());
            return builder;
        }
    }
}
