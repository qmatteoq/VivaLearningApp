using System.ComponentModel.DataAnnotations;

namespace VivaLearningApp.Models
{
    public class LearningContentModel
    {
        [Required]
        public string? ExternalId { get; set; }

        [Required]
        public string? Title { get; set; }

        [Required]
        public string? ContentWebUrl { get; set; }

        [Required]
        public string? LanguageTag { get; set; }
    }
}
