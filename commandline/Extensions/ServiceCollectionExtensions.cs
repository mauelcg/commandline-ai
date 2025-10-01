// -------------------------------------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="Kyocera Document Solutions Development Philippines, Inc.">
// Copyright (c) Kyocera Document Solutions Development Philippines, Inc.. All rights reserved.
// </copyright>
// -------------------------------------------------------------------------------------------------

using Application.Models;
using Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomSerilog(this IServiceCollection services, IHostEnvironment hostEnvironment)
        {
            Log.Logger = CreateLogger(hostEnvironment);
            return services;
        }

        private static Logger CreateLogger(IHostEnvironment hostEnvironment)
        {
            var loggerConfiguration = new LoggerConfiguration().MinimumLevel.Verbose();

            // Write only to console if verbosity is not silent
            if (Globals.VerbosityLevel != VerbosityLevel.Silent)
            {
                loggerConfiguration = loggerConfiguration
                    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3} {SourceContext}] {Message}{NewLine}{Exception}")
                    .MinimumLevel.Debug();
            }

            // Write only to file if not integration test
            if (!hostEnvironment.EnvironmentName.Equals("IntegrationTest", StringComparison.InvariantCultureIgnoreCase))
            {
                var logLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "kyosigntool", "logs", "applogs.txt");
                loggerConfiguration = loggerConfiguration
                    .WriteTo.File(logLocation,
                                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{SourceContext}] {Message}{NewLine}{Exception}",
                                rollingInterval: RollingInterval.Day)
                    .MinimumLevel.Debug();
            }

            // Override the verbosity levels
            LogEventLevel serilogLevel;
            if (Globals.VerbosityLevel == VerbosityLevel.Info)
            {
                serilogLevel = LogEventLevel.Information;
            }
            else if (Globals.VerbosityLevel == VerbosityLevel.Silent)
            {
                serilogLevel = LogEventLevel.Error;
            }
            else
            {
                serilogLevel = LogEventLevel.Verbose;
            }

            loggerConfiguration.MinimumLevel.Override("Application", serilogLevel);
            loggerConfiguration.MinimumLevel.Override("Microsoft", serilogLevel);

            return loggerConfiguration.CreateLogger();
        }
    }
}
