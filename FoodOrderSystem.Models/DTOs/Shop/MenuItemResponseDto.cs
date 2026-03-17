namespace FoodOrderSystem.Models.DTOs.Shop
{
    public class MenuItemResponseDto
    {
        public Guid MenuItemId { get; set; }
        public string MenuItemName { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsAvailable { get; set; }
        public Guid CategoryId { get; set; }
        public int QuantitySold { get; set; }
    }
}
