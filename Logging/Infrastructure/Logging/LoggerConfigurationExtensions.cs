using Serilog;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Generic;

namespace Dell.Infrastructure.Logging
{
    public static class LoggerConfigurationExtensions
    {
        public static LoggerConfiguration AddSource(this LoggerConfiguration instance, IDictionary<string, (LoggingLevelSwitch LevelSwitch, LogEventLevel DefaultLevel)> sources, string source, LogEventLevel minimumLevel)
        {
            sources.Add(source, (new LoggingLevelSwitch(minimumLevel), minimumLevel));
            if (string.Equals("Default", source, StringComparison.OrdinalIgnoreCase))
            {
                instance.MinimumLevel.ControlledBy(sources[source].LevelSwitch);
            }
            else
            {
                instance.MinimumLevel.Override(source, sources[source].LevelSwitch);
            }

            return instance;
        }
    }
}
