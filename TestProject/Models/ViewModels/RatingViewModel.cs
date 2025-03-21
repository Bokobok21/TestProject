using System.ComponentModel.DataAnnotations;

namespace TestProject.Models.ViewModels
{
    public class RatingViewModel
    {
        [Range(1, 5)]
        public int Score { get; set; } = 5;

        public string? Comment { get; set; } = string.Empty;
    }
}
