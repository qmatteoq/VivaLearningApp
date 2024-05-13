using System.ComponentModel.DataAnnotations;

namespace VivaLearningApp.Models
{
    public class LearningAssignmentModel
    {
        [Required]
        public string UserMail { get; set; }

        [Required]
        public DateTime DueDate { get; set; }
    }
}
