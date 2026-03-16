namespace FoodOrderSystem.Models.DTOs.ResponseFormat
{
    /// <summary>
    /// Generic response wrapper that matches Android app expectations
    /// Android expects: { success, messageId, message, data }
    /// </summary>
    public class ApiResponseDto<T>
    {
        public bool Success { get; set; } = true;
        public string MessageId { get; set; } = string.Empty;
        public string Message { get; set; } = "Success";
        public T? Data { get; set; }
    }
}
