using Microsoft.AspNetCore.Components;
using VivaLearningApp.Models;
using VivaLearningApp.Services;

namespace VivaLearningApp.Pages
{
    public class NewLearningProviderBase : ComponentBase
    {
        [Inject]
        ICustomGraphService graphService { get; set; }

        public LearningProviderModel learningProviderModel = new();

        public string? learningProviderId;

        public async Task HandleSubmit()
        {
            if (learningProviderModel != null)
            {
                var learningProvider = await graphService.CreateLearningProviderAsync(learningProviderModel.DisplayName, learningProviderModel.SquareLogoLight, learningProviderModel.LongLogoLight,
                    learningProviderModel.SquareLogoDark, learningProviderModel.LongLogoDark);
                learningProviderId = learningProvider.Id;
            }
        }
    }
}
