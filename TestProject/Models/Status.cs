using System.ComponentModel.DataAnnotations;

namespace TestProject.Models
{
    public enum Status
    {
        [Display(Name = "Обработва се")]
        Pending,
        [Display(Name = "Прието")]
        Accepted,
        [Display(Name = "Отказано")]
        Rejected
    }
}
