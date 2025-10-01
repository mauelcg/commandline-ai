// -------------------------------------------------------------------------------------------------
// <copyright file="ExitCodes.cs" company="Kyocera Document Solutions Development Philippines, Inc.">
// Copyright (c) Kyocera Document Solutions Development Philippines, Inc.. All rights reserved.
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Domain.Enums
{
    public static class ExitCodes
    {
        public const int Success = 0;
        public const int ErrorCredentials = 401;
        public const int ErrorRetrievingCertificates = 500;
        public const int ErrorRequestTimeout = 408;
        public const int ErrorGeneral = 1;
    }
}
