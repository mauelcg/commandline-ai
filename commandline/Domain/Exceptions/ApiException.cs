// -------------------------------------------------------------------------------------------------
// <copyright file="ApiException.cs" company="Kyocera Document Solutions Development Philippines, Inc.">
// Copyright (c) Kyocera Document Solutions Development Philippines, Inc.. All rights reserved.
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Domain.Exceptions
{
    public class ApiException : Exception
    {
        public IEnumerable<string> ErrorMessages { get; }

        public ApiException(string message) : base(message)
        {
        }

        public ApiException(string message, Exception inner) : base(message, inner)
        {
        }

        public ApiException(string message, IEnumerable<string> errorMessages) : base(message)
        {
            ErrorMessages = errorMessages;
        }
    }
}
