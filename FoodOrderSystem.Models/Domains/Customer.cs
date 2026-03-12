using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodOrderSystem.Models.Domains
{
    public class Customer
    {
        [Key]
        public Guid CustomerId { get; set; }

        public string UserId { get; set; } = null!;
        [ForeignKey("UserId")] public virtual ApplicationUser ApplicationUser { get; set; } = null!;

    }
}
