using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
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

        [Inject]
        AuthenticationStateProvider AuthenticationStateProvider { get; set; }


        public IList<LearningProvider> providers = new List<LearningProvider>();
        public IList<LearningContent> contents = null;
        public string tenantId;

        public void CreateNewLearningContent(string learningProviderId)
        {
            navigationManager.NavigateTo($"/newLearningContent/{learningProviderId}");
        }

        public void AddAssignment(string learningContentId)
        {
            navigationManager.NavigateTo($"/addAssignment?learningContentId={learningContentId}&learningProviderId={providers?.FirstOrDefault().Id}");
        }

        protected override async Task OnInitializedAsync()
        {
            var state = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = state.User;

            string tenantId = user.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid")?.Value;
            graphService.AcquireApplicationAccessToken(tenantId);
            providers = await graphService.GetLearningProvidersAsync();
        }
        public async Task LoadContent(string providerId)
        {
            if (providers != null && providers.Count > 0)
            {
                contents = await graphService.GetLearningContentAsync(providerId);
            }
        }

    }
}
