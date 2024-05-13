using Microsoft.AspNetCore.Components;
using Microsoft.Graph.Models;
using VivaLearningApp.Models;
using VivaLearningApp.Services;

namespace VivaLearningApp.Pages
{
    public class AddAssignmentBase: ComponentBase
    {
        [SupplyParameterFromQuery(Name = "LearningContentId")]
        public string LearningContentId { get; set; }

        [SupplyParameterFromQuery(Name = "LearningProviderId")]
        public string LearningProviderId { get; set; }

        [Inject]
        ICustomGraphService graphService { get; set; }

        public LearningAssignmentModel learningAssignmentModel = new LearningAssignmentModel
        {
            DueDate = DateTime.Now
        };

        public async Task HandleSubmit()
        {
            if (learningAssignmentModel != null) 
            {
                var userId = await graphService.GetUserId(learningAssignmentModel.UserMail);
                DateTimeTimeZone dueDate = new()
                {
                    DateTime = learningAssignmentModel.DueDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                    TimeZone = "UTC"
                };

                await graphService.AddAssignment(LearningProviderId, LearningContentId, userId, dueDate);
            }
        }
    }
}
