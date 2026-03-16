namespace FoodOrderSystem.Models.DTOs.Category
{
    /// <summary>
    /// Category response for listing
    /// Android calls: GET /api/v1/categories
    /// HomeFragment.java:132 → CategoryViewModel.getAllCategories()
    /// </summary>
    public class CategoryResponseDto
    {
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
    }
}
