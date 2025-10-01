// -------------------------------------------------------------------------------------------------
// <copyright file="AuthenticationMiddleware.cs" company="Kyocera Document Solutions Development Philippines, Inc.">
// Copyright (c) Kyocera Document Solutions Development Philippines, Inc.. All rights reserved.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System.CommandLine.Hosting;
using System.CommandLine.Invocation;
using Application.Models;
using Domain.Enums;
using Domain.Exceptions;
using Infrastructure.Services.IServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Application.Middlewares
{
    public class AuthenticationMiddleware : BaseMiddleware
    {
        private readonly ILogger<AuthenticationMiddleware> _logger;
        private readonly IApiService _apiService;

        public AuthenticationMiddleware(InvocationContext context, Func<InvocationContext, Task> next) : base(context, next)
        {
            _logger = context.GetHost().Services.GetRequiredService<ILogger<AuthenticationMiddleware>>();
            _apiService = context.GetHost().Services.GetRequiredService<IApiService>();
        }

        public static async Task Handler(InvocationContext context, Func<InvocationContext, Task> next)
        {
            await new AuthenticationMiddleware(context, next).InvokeAsync();
        }

        protected override async Task Run()
        {
            try
            {
                // Execute API Login as base API request
                await _apiService.Login(_parseResult.GetValueForOption(Globals.EmailOption), _parseResult.GetValueForOption(Globals.PasswordOption));
            }
            catch (ApiLoginException exception)
            {
                // Set exit code to ErrorCredentials
                _context.ExitCode = ExitCodes.ErrorCredentials;

                // Show exception details
                _logger.LogCritical("An exception has occured: \n\n{exception}\n", exception);

                // Invoke cancellationToken
                _hostApplicationLifetime.StopApplication();
            }
            catch (HttpRequestException exception)
            {
                _context.ExitCode = ExitCodes.ErrorRequestTimeout;

                // Show exception details
                _logger.LogCritical("An exception has occured: \n\n{exception}\n", exception);

                // Invoke cancellationToken
                _hostApplicationLifetime.StopApplication();
            }
            catch (Exception exception)
            {
                _context.ExitCode = ExitCodes.ErrorGeneral;

                // Show exception details
                _logger.LogCritical("An exception has occured: \n\n{exception}\n", exception);

                // Invoke cancellationToken
                _hostApplicationLifetime.StopApplication();
            }
        }
    }
}
