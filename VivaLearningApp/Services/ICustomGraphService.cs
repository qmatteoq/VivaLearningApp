using Microsoft.Graph.Models;


namespace VivaLearningApp.Services
{
    public interface ICustomGraphService
    {
        void AcquireApplicationAccessToken(string tenantId);
        Task<LearningProvider> CreateLearningProviderAsync(string name, string squareLogoLight, string logoLogoLight, string squareLogoDark, string longLogoDark);
        Task<IList<LearningContent>?> GetLearningContentAsync(string id);
        Task<IList<LearningProvider>> GetLearningProvidersAsync();
        Task AddLearningContent(string providerId, string contentId, string title, string contentUrl, string language);
        Task AddAssignment(string providerId, string contentId, string userId, DateTimeTimeZone dueDateTime);
        Task<string> GetUserId(string usermail);
    }
}