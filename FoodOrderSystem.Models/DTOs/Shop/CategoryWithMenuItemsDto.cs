namespace FoodOrderSystem.Models.DTOs.Shop
{
    public class CategoryWithMenuItemsDto
    {
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public List<MenuItemResponseDto> MenuItems { get; set; } = new List<MenuItemResponseDto>();
    }
}
