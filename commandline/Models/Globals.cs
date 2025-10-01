// -------------------------------------------------------------------------------------------------
// <copyright file="Globals.cs" company="Kyocera Document Solutions Development Philippines, Inc.">
// Copyright (c) Kyocera Document Solutions Development Philippines, Inc.. All rights reserved.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System.CommandLine;
using Domain.Enums;

namespace Application.Models
{
    public class Globals
    {
        public static VerbosityLevel VerbosityLevel { get; set; }

        // Options
        public static readonly Option<string> ServerOption = new("--server", "The server URL to use https://[ipAddress]:[portNumber]. If none is set, then the default server URL will be used (i.e. https://10.191.21.30:7779)");
        public static readonly Option<VerbosityLevel> VerbosityOption = new("--verbosity", "How quiet or loud you would like this to be.");
        public static readonly Option<string> EmailOption = new("--email", "User email address");
        public static readonly Option<string> PasswordOption = new("--password", "User password");
    }
}
