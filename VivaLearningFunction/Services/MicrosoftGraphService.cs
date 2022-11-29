using Azure.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using VivaLearningApp.Models;

namespace VivaLearningFunction.Services
{
    public class MicrosoftGraphService : IMicrosoftGraphService
    {
        private GraphServiceClient graphClient;
        private HttpClient applicationGraphClient;
        private readonly IConfiguration _configuration;

        public MicrosoftGraphService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task AcquireAccessTokenAsync()
        {
            var scopes = new string[] { ".default" };

            string clientId = _configuration["AzureAd:ClientId"];
            string tenantId = _configuration["AzureAd:TenantId"];
            string clientSecret = _configuration["AzureAd:ClientSecret"];

            var client = ConfidentialClientApplicationBuilder.Create(clientId)
                .WithTenantId(tenantId)
                .WithClientSecret(clientSecret).Build();

            var token = await client.AcquireTokenForClient(scopes).ExecuteAsync();
            var authProvider = new DelegateAuthenticationProvider(async (request) =>
            {
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", token.AccessToken);
            });

            graphClient = new GraphServiceClient(authProvider);

            applicationGraphClient = new HttpClient();
            applicationGraphClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

        }

        public async Task<IList<User>> GetUsers()
        {
            var response = await graphClient.Users.Request().GetAsync();
            return response.CurrentPage;
        }

        public async Task<IList<LearningContent>> GetLearningContentAsync(string id)
        {
            string baseUrl = graphClient.EmployeeExperience.LearningProviders.RequestUrl;
            string content = $"{baseUrl}/{id}/learningContents";
            var result = await applicationGraphClient.GetFromJsonAsync<LearningProviderLearningContentsCollectionResponse>(content);
            return result?.Value.CurrentPage;
        }

        public async Task AddLearningContent(string providerId, LearningContentModel learningContent)
        {
            string baseUrl = graphClient.EmployeeExperience.LearningProviders.RequestUrl;
            string url = $"{baseUrl}/{providerId}/learningContents(externalId='{learningContent.ExternalId}')";

            JsonContent jsonContent = JsonContent.Create(learningContent);
            var result = await applicationGraphClient.PatchAsync(url, jsonContent);
        }
    }
}
