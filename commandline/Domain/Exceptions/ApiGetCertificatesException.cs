// -------------------------------------------------------------------------------------------------
// <copyright file="ApiGetCertificatesException.cs" company="Kyocera Document Solutions Development Philippines, Inc.">
// Copyright (c) Kyocera Document Solutions Development Philippines, Inc.. All rights reserved.
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Domain.Exceptions
{
    public class ApiGetCertificatesException : ApiException
    {
        public ApiGetCertificatesException(string message) : base(message) { }

        public ApiGetCertificatesException(string message, Exception inner) : base(message, inner) { }

        public ApiGetCertificatesException(string message, IEnumerable<string> errorMessages) : base(message, errorMessages) { }
    }
}
