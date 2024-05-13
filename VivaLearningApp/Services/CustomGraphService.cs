using Microsoft.Graph;
using Microsoft.Identity.Web;
using Azure.Identity;
using Microsoft.Graph.Models;


namespace VivaLearningApp.Services
{
    public class CustomGraphService : ICustomGraphService
    {
        private readonly IConfiguration configuration;
        private GraphServiceClient delegatedClient;
        private GraphServiceClient applicationClient;
        private readonly MicrosoftIdentityConsentAndConditionalAccessHandler consentHandler;

        public CustomGraphService(GraphServiceClient delegatedClient, IConfiguration configuration, MicrosoftIdentityConsentAndConditionalAccessHandler consentHandler)
        {
            this.configuration = configuration;
            this.consentHandler = consentHandler;
            this.delegatedClient = delegatedClient;
        }

        public void AcquireApplicatonAccessToken(string tenantId)
        {
            var aadConfig = configuration.GetSection("AzureAd");
            var clientId = aadConfig["ClientId"];
            var clientSecret = aadConfig["ClientSecret"];

            var clientSecretCredential = new ClientSecretCredential(tenantId, clientId, clientSecret);
        
            applicationClient = new GraphServiceClient(clientSecretCredential);
        }

        public async Task<IList<LearningProvider>> GetLearningProvidersAsync()
        {
            try
            {
                var result = await delegatedClient.EmployeeExperience.LearningProviders.GetAsync();
                return result.Value;
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
                LongLogoWebUrlForDarkTheme = longLogoDark,
                IsCourseActivitySyncEnabled = true
            };

            try
            {
                var learningProvider = await delegatedClient.EmployeeExperience.LearningProviders.PostAsync(provider);
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
            var response = await applicationClient.EmployeeExperience.LearningProviders[id].LearningContents.GetAsync();
            return response.Value;
        }

        public async Task AddLearningContent(string providerId, string contentId, string title, string contentUrl, string language)
        {
            await applicationClient.EmployeeExperience.LearningProviders[providerId].LearningContentsWithExternalId(contentId).PatchAsync(new LearningContent
            {
                Title = title,
                ContentWebUrl = contentUrl,
                ExternalId = contentId,
                LanguageTag = language
            });
        }

        public async Task AddAssignment(string providerId, string contentId, string userId, DateTimeTimeZone dueDateTime)
        {
            LearningAssignment assignment = new LearningAssignment
            {
                LearningProviderId = providerId,
                LearningContentId = contentId,
                LearnerUserId = userId,
                Status = CourseStatus.NotStarted,
                AssignmentType = AssignmentType.Required,
                DueDateTime = dueDateTime,
                AssignedDateTime = DateTimeOffset.Now,
                OdataType = "#microsoft.graph.learningAssignment"
            };

            await applicationClient.EmployeeExperience.LearningProviders[providerId].LearningCourseActivities.PostAsync(assignment);
        }

        public async Task<string> GetUserId(string usermail)
        {
            var users = await applicationClient.Users.GetAsync(requestConfig =>
            {
                requestConfig.QueryParameters.Filter = $"mail eq '{usermail}'";
                requestConfig.QueryParameters.Select = ["id"];
            });

            return users.Value.FirstOrDefault().Id;
        }
    }
}
