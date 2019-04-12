using Microsoft.Extensions.Logging;
using Serilog.Events;
using Steeltoe.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Dell.Infrastructure.Logging
{
    public class SerilogDynamicLoggerProvider : IDynamicLoggerProvider
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger _logger;

        public SerilogDynamicLoggerProvider(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            _logger = _loggerFactory.CreateLogger<SerilogDynamicLoggerProvider>();
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggerFactory.CreateLogger(categoryName);
        }

        public void Dispose()
        {
        }

        public ICollection<ILoggerConfiguration> GetLoggerConfigurations()
        {
            var sources = SerilogConfiguration.GetSources();
            var list = new List<ILoggerConfiguration>();
            foreach (var (k, v) in sources)
            {
                list.Add(new Steeltoe.Extensions.Logging.LoggerConfiguration(k, FromSerilog(v.Default), FromSerilog(v.LevelSwitch.MinimumLevel)));
            }
            return list;
        }

        public void SetLogLevel(string category, LogLevel? level)
        {
            try
            {
                var serilogLevel = level.HasValue ? ToSerilog(level.Value) : SerilogConfiguration.GetDefaultLevel(category);
                SerilogConfiguration.SetLogLevel(category, serilogLevel);
                _logger.LogInformation($"SetLogLevel({category}, {serilogLevel}");
            }
            catch (Exception ex)
            {
                var dotnetLevel = level.HasValue ? level.Value.ToString() : "";
                _logger.LogError(ex, $"Failed SetLogLevel({category}, {dotnetLevel})");
                throw;
            }
        }

        private LogLevel FromSerilog(LogEventLevel level)
        {
            if (level >= LogEventLevel.Verbose && level <= LogEventLevel.Fatal)
            {
                return (LogLevel)level;
            }

            return LogLevel.None;
        }

        private LogEventLevel ToSerilog(LogLevel level)
        {
            if (level >= LogLevel.Trace && level <= LogLevel.Critical)
            {
                return (LogEventLevel)level;
            }

            return LogEventLevel.Warning;
        }
    }
}
