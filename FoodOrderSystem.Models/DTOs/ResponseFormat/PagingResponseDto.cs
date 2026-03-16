namespace FoodOrderSystem.Models.DTOs.ResponseFormat
{
    /// <summary>
    /// For paginated endpoints (shop list, menu list, etc.)
    /// </summary>
    public class PagingResponseDto<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int TotalItems { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
