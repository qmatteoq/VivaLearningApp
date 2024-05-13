using Microsoft.AspNetCore.Components;
using Microsoft.Graph.Models;
using VivaLearningApp.Services;

namespace VivaLearningApp.Pages
{
    public class LearningProvidersBase : ComponentBase
    {
        [Inject]
        ICustomGraphService graphService { get; set; }

        [Inject]
        NavigationManager navigationManager { get; set; }


        public IList<LearningProvider> providers = new List<LearningProvider>();
        public IList<LearningContent> contents = null;
        public string tenantId;

        public void CreateNewLearningContent()
        {
            navigationManager.NavigateTo($"/newLearningContent/{providers?.FirstOrDefault().Id}");
        }

        public void AddAssignment(string learningContentId)
        {
            navigationManager.NavigateTo($"/addAssignment?learningContentId={learningContentId}&learningProviderId={providers?.FirstOrDefault().Id}");
        }

        public async Task LoadProvider()
        {
            if (!string.IsNullOrEmpty(tenantId))
            {
                graphService.AcquireApplicatonAccessToken(tenantId);
                providers = await graphService.GetLearningProvidersAsync();
                if (providers != null && providers.Count > 0)
                {
                    contents = await graphService.GetLearningContentAsync(providers?.FirstOrDefault().Id);
                }
            }
        }
    }
}
