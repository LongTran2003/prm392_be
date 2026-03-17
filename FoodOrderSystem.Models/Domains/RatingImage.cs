using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodOrderSystem.Models.Domains
{
    public class RatingImage
    {
        [Key]
        public Guid RatingImageId { get; set; }

        [Required]
        public Guid RatingId { get; set; }

        [ForeignKey("RatingId")]
        public virtual Rating Rating { get; set; } = null!;

        [StringLength(200)]
        public string ImageUrl { get; set; } = null!;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
