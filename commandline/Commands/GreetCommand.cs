// -------------------------------------------------------------------------------------------------
// <copyright file="GreetCommand.cs" company="Kyocera Document Solutions Development Philippines, Inc.">
// Copyright (c) Kyocera Document Solutions Development Philippines, Inc.. All rights reserved.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System.CommandLine;
using System.CommandLine.Invocation;
using Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Application.Commands
{
    public class GreetCommand : Command
    {
        public GreetCommand() : base("greet", "Greet your fans.")
        {
            // Add argument
            var messageArgument = new Argument<string>("message", "Message you want to convey.");
            AddArgument(messageArgument);

            var isColoredOption = new Option<bool>(
                name: "--colored",
                description: "Set message as colored.");

            AddOption(isColoredOption);
        }

        public new class Handler : ICommandHandler
        {
            private readonly ILogger<GreetCommand> _logger;

            public Handler(ILogger<GreetCommand> logger)
            {
                _logger = logger;
            }

            public string? Message { get; set; }
            public bool Colored { get; set; }

            public int Invoke(InvocationContext context)
            {
                return InvokeAsync(context).GetAwaiter().GetResult();
            }

            public Task<int> InvokeAsync(InvocationContext context)
            {
                return Task.Run(() =>
                {
                    if (Colored)
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                    }

                    _logger.LogInformation($"This is the sample value: {Message}");
                    return ExitCodes.Success;
                });
            }
        }
    }
}
