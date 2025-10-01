// -------------------------------------------------------------------------------------------------
// <copyright file="APIResponse.cs" company="Kyocera Document Solutions Development Philippines, Inc.">
// Copyright (c) Kyocera Document Solutions Development Philippines, Inc.. All rights reserved.
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Domain.Models
{
    public class APIResponse
    {
        public int StatusCode { get; set; }
        public bool IsSuccess { get; set; }
        public List<string> ErrorMessages { get; set; } = new List<string>();
        public object Result { get; set; }
    }
}
