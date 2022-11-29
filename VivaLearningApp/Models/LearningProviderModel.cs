using System.ComponentModel.DataAnnotations;

namespace VivaLearningApp.Models
{
    public class LearningProviderModel
    {
        [Required]
        public string? DisplayName { get; set; }

        [Required]
        public string? SquareLogoLight { get; set; }

        [Required]
        public string? LongLogoLight { get; set; }

        [Required]
        public string? SquareLogoDark { get; set; }

        [Required]
        public string? LongLogoDark { get; set; }
    }
}
