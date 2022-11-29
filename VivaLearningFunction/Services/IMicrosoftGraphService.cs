using Microsoft.Graph;
using System.Collections.Generic;
using System.Threading.Tasks;
using VivaLearningApp.Models;

namespace VivaLearningFunction.Services
{
    public interface IMicrosoftGraphService
    {
        Task AcquireAccessTokenAsync();
        Task AddLearningContent(string providerId, LearningContentModel learningContent);
        Task<IList<LearningContent>> GetLearningContentAsync(string id);
    }
}