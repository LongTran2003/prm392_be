using System.ComponentModel.DataAnnotations;

namespace FoodOrderSystem.Models.DTOs.Rating
{
    /// <summary>
    /// Create rating/review
    /// POST /api/v1/Ratings
    /// Android: After order completed, customer can rate shop
    /// </summary>
    public class CreateRatingDto
    {
        [Required]
        public Guid ShopId { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int StarRating { get; set; }

        [StringLength(500)]
        public string? Comment { get; set; }

        // Images will be sent as multipart files
    }
}
