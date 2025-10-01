// -------------------------------------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="Kyocera Document Solutions Development Philippines, Inc.">
// Copyright (c) Kyocera Document Solutions Development Philippines, Inc.. All rights reserved.
// </copyright>
// -------------------------------------------------------------------------------------------------

using Infrastructure.Factory;
using Infrastructure.Factory.IFactory;
using Infrastructure.Models;
using Infrastructure.Services;
using Infrastructure.Services.IServices;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            // Add services
            services.AddSingleton<IApiService, ApiService>();
            services.AddSingleton<IHttpFactory, HttpFactory>();

            // Configure HttpClient options
            services.Configure<HttpOptions>(options => options.BaseUrl = Constants.BaseUrl);

            return services;
        }
    }
}
