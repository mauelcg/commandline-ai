// -------------------------------------------------------------------------------------------------
// <copyright file="RetrieveCommand.cs" company="Kyocera Document Solutions Development Philippines, Inc.">
// Copyright (c) Kyocera Document Solutions Development Philippines, Inc.. All rights reserved.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System.CommandLine;

namespace Application.Commands
{
    public class RetrieveCommand : Command
    {
        public RetrieveCommand() : base("retrieve", "")
        {
            // Add alias
            AddAlias("get");

            // Add `certificates` subcommand
            Add(new CertificatesCommand());
        }
    }
}
