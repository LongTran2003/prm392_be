namespace FoodOrderSystem.Models.DTOs.ResponseFormat
{
    /// <summary>
    /// Exception response when operation fails
    /// </summary>
    public class ErrorResponseDto
    {
        public bool Success { get; set; } = false;
        public string MessageId { get; set; } = "ERROR";
        public string Message { get; set; } = "An error occurred";
        public object? Data { get; set; }
    }
}
