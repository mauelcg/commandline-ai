// -------------------------------------------------------------------------------------------------
// <copyright file="IBaseMiddleware.cs" company="Kyocera Document Solutions Development Philippines, Inc.">
// Copyright (c) Kyocera Document Solutions Development Philippines, Inc.. All rights reserved.
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Application.Middlewares.IMiddlewares
{
    public interface IBaseMiddleware
    {
        // BaseMiddleware should always have its own InvokeAsync method in case extending classes do not have their own implementations
        Task InvokeAsync();
    }
}
