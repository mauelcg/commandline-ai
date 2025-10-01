// -------------------------------------------------------------------------------------------------
// <copyright file="ApiLoginException.cs" company="Kyocera Document Solutions Development Philippines, Inc.">
// Copyright (c) Kyocera Document Solutions Development Philippines, Inc.. All rights reserved.
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Domain.Exceptions
{
    public class ApiLoginException : ApiException
    {
        public ApiLoginException(string message) : base(message) { }

        public ApiLoginException(string message, Exception inner) : base(message, inner) { }

        public ApiLoginException(string message, IEnumerable<string> errorMessages) : base(message, errorMessages) { }
    }
}
