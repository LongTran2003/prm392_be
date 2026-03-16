using FoodOrderSystem.Models.DTOs.Category;
using FoodOrderSystem.Models.DTOs.ResponseFormat;

namespace FoodOrderSystem.Services.IServices
{
    public interface ICategoryService
    {
        // GET /api/v1/categories
        Task<ApiResponseDto<List<CategoryResponseDto>>> GetAllCategoriesAsync();

        // GET /api/v1/categories/{categoryId}
        Task<ApiResponseDto<CategoryResponseDto>> GetCategoryByIdAsync(Guid categoryId);

        // POST /api/v1/categories (Admin only)
        Task<ApiResponseDto<CategoryResponseDto>> CreateCategoryAsync(CreateCategoryDto createDto);

        // PUT /api/v1/categories/{categoryId} (Admin only)
        Task<ApiResponseDto<string>> UpdateCategoryAsync(Guid categoryId, UpdateCategoryDto updateDto);

        // DELETE /api/v1/categories/{categoryId} (Admin only)
        Task<ApiResponseDto<string>> DeleteCategoryAsync(Guid categoryId);
    }
}
