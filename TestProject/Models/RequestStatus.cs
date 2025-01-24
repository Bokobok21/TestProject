using System.ComponentModel.DataAnnotations;

namespace TestProject.Models
{
    public enum RequestStatus
    {
        [Display(Name = "Обработва се")]
        Pending,
        [Display(Name = "Прието")]
        Accepted,
        [Display(Name = "Отказано")]
        Rejected
    }
}
