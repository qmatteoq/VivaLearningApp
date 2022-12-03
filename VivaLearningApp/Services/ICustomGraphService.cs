using Microsoft.Graph;

namespace VivaLearningApp.Services
{
    public interface ICustomGraphService
    {
        Task AcquireAccessTokenAsync(string tenantId);
        Task<LearningProvider> CreateLearningProviderAsync(string name, string squareLogoLight, string logoLogoLight, string squareLogoDark, string longLogoDark);
        Task<IList<LearningContent>?> GetLearningContentAsync(string id);
        Task<IList<LearningProvider>> GetLearningProvidersAsync();
        Task AddLearningContent(string providerId, string contentId, string title, string contentUrl, string language);
    }
}