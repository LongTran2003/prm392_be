using FoodOrderSystem.Models.DTOs.Category;
using FoodOrderSystem.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrderSystem.API.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// GET /api/v1/categories
        /// Get all categories for home screen
        /// Called from: HomeFragment.java:132 → CategoryViewModel.getAllCategories()
        /// Returns: { success, messageId, message, data: [ { categoryId, categoryName, description, imageUrl }, ... ] }
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllCategories()
        {
            var result = await _categoryService.GetAllCategoriesAsync();
            return StatusCode(result.Success ? 200 : 500, result);
        }

        /// <summary>
        /// GET /api/v1/categories/{categoryId}
        /// Get single category by ID
        /// </summary>
        [HttpGet("{categoryId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategoryById(Guid categoryId)
        {
            var result = await _categoryService.GetCategoryByIdAsync(categoryId);
            return StatusCode(result.Success ? 200 : 404, result);
        }

        /// <summary>
        /// POST /api/v1/categories
        /// Create new category (Admin only)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto createDto)
        {
            var result = await _categoryService.CreateCategoryAsync(createDto);
            return StatusCode(result.Success ? 201 : 400, result);
        }

        /// <summary>
        /// PUT /api/v1/categories/{categoryId}
        /// Update category (Admin only)
        /// </summary>
        [HttpPut("{categoryId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCategory(Guid categoryId, [FromBody] UpdateCategoryDto updateDto)
        {
            var result = await _categoryService.UpdateCategoryAsync(categoryId, updateDto);
            return StatusCode(result.Success ? 200 : 400, result);
        }

        /// <summary>
        /// DELETE /api/v1/categories/{categoryId}
        /// Delete category (Admin only)
        /// </summary>
        [HttpDelete("{categoryId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategory(Guid categoryId)
        {
            var result = await _categoryService.DeleteCategoryAsync(categoryId);
            return StatusCode(result.Success ? 200 : 404, result);
        }
    }
}
