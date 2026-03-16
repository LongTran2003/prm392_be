using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodOrderSystem.Models.Domains
{
    public class Student
    {
        [Key]
        public Guid StudentId { get; set; }

        [Required]
        [StringLength(10)]
        public string StudentCode { get; set; } = null!;

        public string UserId { get; set; } = null!;

        [ForeignKey("UserId")] 
        public virtual ApplicationUser ApplicationUser { get; set; } = null!;

        public DateTime CreatedTime { get; set; }

        public string Status { get; set; } = null!;

    }
}
