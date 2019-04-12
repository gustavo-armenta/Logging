using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Compact;
using System;
using System.Collections.Generic;

namespace Dell.Infrastructure.Logging
{
    public class SerilogConfiguration
    {
        private static IDictionary<string, (LoggingLevelSwitch LevelSwitch, LogEventLevel Default)> _sources;
        private static Serilog.LoggerConfiguration _configuration;
        public static void Configure()
        {
            if (_sources == null)
            {
                _sources = new Dictionary<string, (LoggingLevelSwitch LevelSwitch, LogEventLevel Default)>();
            }
            if (_configuration == null)
            {
                _configuration = new Serilog.LoggerConfiguration()
                    .AddSource(_sources, "Default", LogEventLevel.Warning)
                    .AddSource(_sources, "Dell", LogEventLevel.Warning)
                    .AddSource(_sources, "Dse", LogEventLevel.Warning)
                    .AddSource(_sources, "EasyNetQ", LogEventLevel.Warning)
                    .AddSource(_sources, "Microsoft", LogEventLevel.Warning)
                    .AddSource(_sources, "RabbitMQ", LogEventLevel.Warning)
                    .AddSource(_sources, "Steeltoe", LogEventLevel.Warning)
                    .AddSource(_sources, "System", LogEventLevel.Warning)
                    .Enrich.FromLogContext()
                    .WriteTo.Console(new RenderedCompactJsonFormatter());
                Log.Logger = _configuration.CreateLogger();
            }
        }

        public static IDictionary<string, (LoggingLevelSwitch LevelSwitch, LogEventLevel Default)> GetSources()
        {
            return _sources;
        }

        public static LogEventLevel GetDefaultLevel(string source)
        {
            return _sources[source].Default;
        }

        public static void SetLogLevel(string source, LogEventLevel minimumLevel)
        {
            _sources[source].LevelSwitch.MinimumLevel = minimumLevel;
        }
    }
}
