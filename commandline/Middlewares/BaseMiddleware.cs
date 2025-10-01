// -------------------------------------------------------------------------------------------------
// <copyright file="BaseMiddleware.cs" company="Kyocera Document Solutions Development Philippines, Inc.">
// Copyright (c) Kyocera Document Solutions Development Philippines, Inc.. All rights reserved.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System.CommandLine.Hosting;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using Application.Middlewares.IMiddlewares;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Application.Middlewares
{
    public abstract class BaseMiddleware : IBaseMiddleware
    {
        public InvocationContext _context;
        public IHostApplicationLifetime _hostApplicationLifetime;
        public ParseResult _parseResult;
        public Func<InvocationContext, Task> _next;

        protected abstract Task Run();

        protected BaseMiddleware(InvocationContext context, Func<InvocationContext, Task> next)
        {
            _context = context;
            _parseResult = context.GetHost().Services.GetRequiredService<ParseResult>();
            _hostApplicationLifetime = context.GetHost().Services.GetRequiredService<IHostApplicationLifetime>();
            _next = next;
        }

        // Can be overridden or called from extending class
        public virtual async Task InvokeAsync()
        {
            // Generate cancellation token in case request fails
            var token = _hostApplicationLifetime.ApplicationStopping.Register(() => { }).Token;

            // Implemented by extended class
            await Run();

            // Cancellation will be invoked after StopApplication() is executed
            if (!token.IsCancellationRequested)
            {
                // Continue processing arguments during success
                await _next(_context);
            }
        }
    }
}
