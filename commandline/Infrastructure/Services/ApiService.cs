// -------------------------------------------------------------------------------------------------
// <copyright file="ApiService.cs" company="Kyocera Document Solutions Development Philippines, Inc.">
// Copyright (c) Kyocera Document Solutions Development Philippines, Inc.. All rights reserved.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System.Net.Http.Headers;
using System.Net.Http.Json;
using Domain.Exceptions;
using Domain.Models;
using Domain.Models.DTO;
using Infrastructure.Factory.IFactory;
using Infrastructure.Services.IServices;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Infrastructure.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _client;
        private readonly ILogger<ApiService> _logger;
        public ApiService(ILogger<ApiService> logger, IHttpFactory factory)
        {
            _logger = logger;
            _client = factory.GetClient();
        }

        public async Task<string> Login(string userName, string password)
        {
            _logger.LogInformation("Logging in. Please wait.");

            // Create the login request
            var user = new LoginRequestDTO
            {
                UserName = userName,
                Password = password
            };
            // Call the login API
            // NOTE: Add try-catch to handle "request timed-out" scenario, as request won't get resolved even by "awaiting" _client.PostAsJsonAsync
            try
            {
                var response = await _client.PostAsJsonAsync("api/v1/auth/login", user);
                var responseModel = await response.Content.ReadFromJsonAsync<APIResponse>();

                // Check if failed
                if (!responseModel.IsSuccess)
                {
                    _logger.LogDebug("Unable to login. More information: {messages}", responseModel.ErrorMessages);
                    _logger.LogCritical("Unable to login.");

                    throw new ApiLoginException("Unable to login.", responseModel.ErrorMessages);
                }

                // Cast real result
                var result = JsonConvert.DeserializeObject<LoginResponseDTO>(responseModel.Result.ToString());
                var token = result.Token;

                // Set the token to the client headers
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                return token;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<CertificateDTO>> GetCertificates()
        {
            _logger.LogInformation("Getting available certificates. Please wait.\n");

            // Call the `get certificates` API
            var response = await _client.GetAsync("api/v1/certificates");
            var responseModel = await response.Content.ReadFromJsonAsync<APIResponse>();

            // Check if failed
            if (!responseModel.IsSuccess)
            {
                _logger.LogDebug("Error retrieving certificates. More information: {messages}", responseModel.ErrorMessages);
                _logger.LogCritical("Error retrieving certificates.");

                throw new ApiGetCertificatesException("Error retrieving certificates.", responseModel.ErrorMessages);
            }

            // Cast real result
            var result = JsonConvert.DeserializeObject<List<CertificateDTO>>(responseModel.Result.ToString());
            return result;
        }

        public async Task<string> DownloadPublicCertificate(int id)
        {
            _logger.LogDebug("Fetching public certificate. Please wait.\n");

            var response = await _client.GetAsync($"api/v1/certificates/{id}/download");
            var certificateLocation = @"Resources\certificate.cer";

            if (File.Exists(certificateLocation))
            {
                File.Delete(certificateLocation);
            }

            Directory.CreateDirectory(Path.GetFullPath("Resources"));

            using var resultStream = await response.Content.ReadAsStreamAsync();
            using var fileStream = File.Create(certificateLocation);
            resultStream.CopyTo(fileStream);

            return certificateLocation;
        }
    }
}
