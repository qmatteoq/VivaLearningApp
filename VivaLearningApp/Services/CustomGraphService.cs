using Microsoft.Graph;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;
using System.Net.Http.Headers;
using VivaLearningApp.Models;

namespace VivaLearningApp.Services
{
    public class CustomGraphService : ICustomGraphService
    {
        private readonly IConfiguration configuration;
        private readonly GraphServiceClient delegatedClient;
        private readonly MicrosoftIdentityConsentAndConditionalAccessHandler consentHandler;
        private string? accessToken;
        private GraphServiceClient? applicationClient;

        public CustomGraphService(IConfiguration configuration, GraphServiceClient delegatedClient, MicrosoftIdentityConsentAndConditionalAccessHandler consentHandler)
        {
            this.configuration = configuration;
            this.delegatedClient = delegatedClient;
            this.consentHandler = consentHandler;
        }

        public async Task AcquireAccessTokenAsync(string tenantId)
        {
            var scopes = new string[] { ".default" };

            var aadConfig = configuration.GetSection("AzureAd");
            var client = ConfidentialClientApplicationBuilder.Create(aadConfig["ClientId"])
                //.WithTenantId(aadConfig["TenantId"])
                .WithTenantId(tenantId)
                .WithClientSecret(aadConfig["ClientSecret"]).Build();

            var token = await client.AcquireTokenForClient(scopes).ExecuteAsync();
            accessToken = token.AccessToken;

            var authProvider = new DelegateAuthenticationProvider(async (request) =>
            {
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", accessToken);
            });

            applicationClient = new GraphServiceClient(authProvider);
        }

        public async Task<IList<LearningProvider>> GetLearningProvidersAsync()
        {
            try
            {
                var result = await delegatedClient.EmployeeExperience.LearningProviders.Request().GetAsync();
                return result.CurrentPage;
            }
            catch (Exception ex)
            {
                consentHandler.HandleException(ex);
                return null;
            }
        }

        public async Task<LearningProvider> CreateLearningProviderAsync(string name, string squareLogoLight, string logoLogoLight, string squareLogoDark, string longLogoDark)
        {
            LearningProvider provider = new LearningProvider
            {
                DisplayName = name,
                SquareLogoWebUrlForLightTheme = squareLogoLight,
                LongLogoWebUrlForLightTheme = logoLogoLight,
                SquareLogoWebUrlForDarkTheme = squareLogoDark,
                LongLogoWebUrlForDarkTheme = longLogoDark
            };

            try
            {
                var learningProvider = await delegatedClient.EmployeeExperience.LearningProviders.Request().AddAsync(provider);
                return learningProvider;
            }
            catch (Exception ex)
            {
                consentHandler.HandleException(ex);
                return null;
            }
        }

        public async Task<IList<LearningContent>?> GetLearningContentAsync(string id)
        {
            var response = await applicationClient.EmployeeExperience.LearningProviders[id].LearningContents.Request().GetAsync();
            return response.CurrentPage;
        }

        public async Task AddLearningContent(string providerId, string contentId, string title, string contentUrl, string language)
        {
            string baseUrl = applicationClient.EmployeeExperience.LearningProviders.RequestUrl;
            
            string url = $"{baseUrl}/{providerId}/learningContents(externalId='{contentId}')";
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            LearningContentModel content = new()
            {
                Title = title,
                ExternalId = contentId,
                ContentWebUrl = contentUrl,
                LanguageTag= language
            };

            JsonContent jsonContent = JsonContent.Create(content);

            var result = await client.PatchAsync(url, jsonContent);
        }
    }
}
