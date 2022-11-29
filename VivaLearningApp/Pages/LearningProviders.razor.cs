using Microsoft.AspNetCore.Components;
using Microsoft.Graph;
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

        protected async override Task OnInitializedAsync()
        {
            await graphService.AcquireAccessTokenAsync();
            providers = await graphService.GetLearningProvidersAsync();
            contents = await graphService.GetLearningContentAsync(providers?.FirstOrDefault().Id);
        }

        public void CreateNewLearningContent()
        {
            navigationManager.NavigateTo($"/newLearningContent/{providers?.FirstOrDefault().Id}");
        }
    }
}
