using System.ComponentModel.DataAnnotations;

namespace TestProject.Models
{
    public enum TripStatus
    {
        [Display(Name = "Предстоящо")]
        Upcoming,
        [Display(Name = "В процес")]
        Ongoing,
        [Display(Name = "Завършено")]
        Finished
    }
}
