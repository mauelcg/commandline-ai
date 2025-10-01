// -------------------------------------------------------------------------------------------------
// <copyright file="CertificatesCommand.cs" company="Kyocera Document Solutions Development Philippines, Inc.">
// Copyright (c) Kyocera Document Solutions Development Philippines, Inc.. All rights reserved.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System.CommandLine;
using System.CommandLine.Invocation;
using Domain.Enums;
using Domain.Exceptions;
using Infrastructure.Services.IServices;
using Microsoft.Extensions.Logging;

namespace Application.Commands
{
    public class CertificatesCommand : Command
    {
        public CertificatesCommand() : base("certificates", "Get available certificates for signing") { }

        public new class Handler : ICommandHandler
        {
            private readonly ILogger<CertificatesCommand> _logger;
            private readonly IApiService _apiService;

            public Handler(ILogger<CertificatesCommand> logger, IApiService apiService)
            {
                _logger = logger;
                _apiService = apiService;
            }

            public int Invoke(InvocationContext context)
            {
                return InvokeAsync(context).GetAwaiter().GetResult();
            }

            public Task<int> InvokeAsync(InvocationContext context)
            {
                return Task.Run(async () =>
                {
                    try
                    {
                        var result = await _apiService.GetCertificates();

                        if (result.Count == 0)
                        {
                            _logger.LogCritical("No certificate found.");
                        }

                        foreach (var certificate in result)
                        {
                            _logger.LogInformation(certificate.Name);
                            // Use Spectre.Console for showing certificates
                        }
                    }
                    catch (ApiGetCertificatesException exception)
                    {
                        _logger.LogTrace("An exception has occured: \n\n{exception}\n", exception);
                        return ExitCodes.ErrorRetrievingCertificates;
                    }

                    return ExitCodes.Success;
                });
            }
        }
    }
}
