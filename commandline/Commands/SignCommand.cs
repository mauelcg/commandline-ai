// -------------------------------------------------------------------------------------------------
// <copyright file="SignCommand.cs" company="Kyocera Document Solutions Development Philippines, Inc.">
// Copyright (c) Kyocera Document Solutions Development Philippines, Inc.. All rights reserved.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System.CommandLine;
using System.CommandLine.Invocation;
using Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Application.Commands
{
    public class SignCommand : Command
    {
        // private readonly string[] _signable_files = new[] { ".exe", ".dl_", ".dll", ".sys", ".msi", "ps1", ".cat", ".hlkx", ".hckx", ".devicemanifest-ms" };

        public SignCommand() : base("sign", "Initiate a signing task.")
        {
            // Add argument
            var filesArgument = new Argument<string>("files", "The location of the file(s) to sign. If the location passed is a directory, then it will recursively sign all the signable files inside the directory.");
            AddArgument(filesArgument);

            // Add options
            var certificateOption = new Option<string>("--certificate", "The name of the certificate to use, you can use the command \"get certificates\" to retrieve all the available certificates. If none is set, then the default certificate will be used.");
            AddOption(certificateOption);

            var descriptionOption = new Option<string>("--description", "The description of the signing task, used for easier tracking and auditing.");
            AddOption(descriptionOption);
        }

        public new class Handler : ICommandHandler
        {
            // private readonly ILogger<SignCommand> _logger;

            public Handler(ILogger<SignCommand> logger)
            {
                // _logger = logger;
            }

            public string? Files { get; set; }
            public string? Certificate { get; set; }
            public string? Description { get; set; }

            public int Invoke(InvocationContext context)
            {
                return InvokeAsync(context).GetAwaiter().GetResult();
            }

            public Task<int> InvokeAsync(InvocationContext context)
            {
                return Task.Run(() =>
                {
                    string originalFile = Files;

                    // If path is not absolute then get the full path
                    if (!Path.IsPathRooted(Files))
                    {
                        Files = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), Files));
                        originalFile = Files;
                    }

                    // Set the current directory to the basedirectory
                    // So that it will be able to find the tools
                    Environment.CurrentDirectory = AppContext.BaseDirectory;

                    return ExitCodes.Success;
                });
            }
        }
    }
}
