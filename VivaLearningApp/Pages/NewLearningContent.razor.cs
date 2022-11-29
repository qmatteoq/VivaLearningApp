using Microsoft.AspNetCore.Components;
using Microsoft.Graph;
using VivaLearningApp.Models;
using VivaLearningApp.Services;

namespace VivaLearningApp.Pages
{
    public class NewLearningContentBase: ComponentBase
    {
        [Parameter]
        public string? LearningProviderId { get; set; }

        [Inject]
        ICustomGraphService graphService { get; set; }

        public LearningContentModel learningContentModel = new();

        public async Task HandleSubmit()
        {
            if (learningContentModel != null)
            {
                await graphService.AddLearningContent(LearningProviderId, learningContentModel.ExternalId, learningContentModel.Title, learningContentModel.ContentWebUrl, learningContentModel.LanguageTag);
            }
        }
    }
}
