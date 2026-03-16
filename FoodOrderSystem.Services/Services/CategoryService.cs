using AutoMapper;
using FoodOrderSystem.DataAccess.IRepositories;
using FoodOrderSystem.Models.Domains;
using FoodOrderSystem.Models.DTOs.Category;
using FoodOrderSystem.Models.DTOs.ResponseFormat;
using FoodOrderSystem.Services.IServices;
using FoodOrderSystem.Utilities.Constants;

namespace FoodOrderSystem.Services.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// GET /api/v1/categories
        /// Get all categories for home screen
        /// Called from: HomeFragment.java:132 → CategoryViewModel.getAllCategories()
        /// </summary>
        public async Task<ApiResponseDto<List<CategoryResponseDto>>> GetAllCategoriesAsync()
        {
            try
            {
                var categories = await _unitOfWork.Category.GetAllActiveCategoriesAsync();

                var categoryList = categories
                    .Select(c => new CategoryResponseDto
                    {
                        CategoryId = c.CategoryId,
                        CategoryName = c.CategoryName,
                        Description = c.Description,
                        ImageUrl = c.ImageUrl
                    })
                    .ToList();

                return new ApiResponseDto<List<CategoryResponseDto>>
                {
                    Success = true,
                    Message = "Categories retrieved successfully",
                    Data = categoryList
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<List<CategoryResponseDto>>
                {
                    Success = false,
                    Message = $"Failed to retrieve categories: {ex.Message}",
                    MessageId = "CATEGORIES_FETCH_FAILED"
                };
            }
        }

        /// <summary>
        /// GET /api/v1/categories/{categoryId}
        /// Get single category by ID
        /// </summary>
        public async Task<ApiResponseDto<CategoryResponseDto>> GetCategoryByIdAsync(Guid categoryId)
        {
            try
            {
                if (categoryId == Guid.Empty)
                {
                    return new ApiResponseDto<CategoryResponseDto>
                    {
                        Success = false,
                        Message = "Invalid category ID",
                        MessageId = "INVALID_CATEGORY_ID"
                    };
                }

                var category = await _unitOfWork.Category.GetAsync(c => c.CategoryId == categoryId);
                if (category == null)
                {
                    return new ApiResponseDto<CategoryResponseDto>
                    {
                        Success = false,
                        Message = "Category not found",
                        MessageId = "CATEGORY_NOT_FOUND"
                    };
                }

                var response = new CategoryResponseDto
                {
                    CategoryId = category.CategoryId,
                    CategoryName = category.CategoryName,
                    Description = category.Description,
                    ImageUrl = category.ImageUrl
                };

                return new ApiResponseDto<CategoryResponseDto>
                {
                    Success = true,
                    Message = "Category retrieved successfully",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<CategoryResponseDto>
                {
                    Success = false,
                    Message = $"Failed to retrieve category: {ex.Message}",
                    MessageId = "CATEGORY_FETCH_FAILED"
                };
            }
        }

        /// <summary>
        /// POST /api/v1/categories
        /// Create new category (Admin only)
        /// </summary>
        public async Task<ApiResponseDto<CategoryResponseDto>> CreateCategoryAsync(CreateCategoryDto createDto)
        {
            try
            {
                if (string.IsNullOrEmpty(createDto.CategoryName))
                {
                    return new ApiResponseDto<CategoryResponseDto>
                    {
                        Success = false,
                        Message = "Category name is required",
                        MessageId = "INVALID_CATEGORY_NAME"
                    };
                }

                var category = new Category
                {
                    CategoryId = Guid.NewGuid(),
                    CategoryName = createDto.CategoryName,
                    Description = createDto.Description,
                    ImageUrl = createDto.ImageUrl,
                    CreatedDate = DateTime.UtcNow,
                    Status = "Active"
                };

                await _unitOfWork.Category.AddAsync(category);
                await _unitOfWork.SaveAsync();

                var response = new CategoryResponseDto
                {
                    CategoryId = category.CategoryId,
                    CategoryName = category.CategoryName,
                    Description = category.Description,
                    ImageUrl = category.ImageUrl
                };

                return new ApiResponseDto<CategoryResponseDto>
                {
                    Success = true,
                    Message = "Category created successfully",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<CategoryResponseDto>
                {
                    Success = false,
                    Message = $"Failed to create category: {ex.Message}",
                    MessageId = "CATEGORY_CREATE_FAILED"
                };
            }
        }

        /// <summary>
        /// PUT /api/v1/categories/{categoryId}
        /// Update category (Admin only)
        /// </summary>
        public async Task<ApiResponseDto<string>> UpdateCategoryAsync(Guid categoryId, UpdateCategoryDto updateDto)
        {
            try
            {
                if (categoryId == Guid.Empty)
                {
                    return new ApiResponseDto<string>
                    {
                        Success = false,
                        Message = "Invalid category ID",
                        MessageId = "INVALID_CATEGORY_ID"
                    };
                }

                var category = await _unitOfWork.Category.GetAsync(c => c.CategoryId == categoryId);
                if (category == null)
                {
                    return new ApiResponseDto<string>
                    {
                        Success = false,
                        Message = "Category not found",
                        MessageId = "CATEGORY_NOT_FOUND"
                    };
                }

                category.CategoryName = updateDto.CategoryName;
                category.Description = updateDto.Description;
                category.ImageUrl = updateDto.ImageUrl;
                category.UpdatedDate = DateTime.UtcNow;

                _unitOfWork.Category.Update(category);
                await _unitOfWork.SaveAsync();

                return new ApiResponseDto<string>
                {
                    Success = true,
                    Message = "Category updated successfully"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<string>
                {
                    Success = false,
                    Message = $"Failed to update category: {ex.Message}",
                    MessageId = "CATEGORY_UPDATE_FAILED"
                };
            }
        }

        /// <summary>
        /// DELETE /api/v1/categories/{categoryId}
        /// Delete category (Admin only)
        /// </summary>
        public async Task<ApiResponseDto<string>> DeleteCategoryAsync(Guid categoryId)
        {
            try
            {
                if (categoryId == Guid.Empty)
                {
                    return new ApiResponseDto<string>
                    {
                        Success = false,
                        Message = "Invalid category ID",
                        MessageId = "INVALID_CATEGORY_ID"
                    };
                }

                var category = await _unitOfWork.Category.GetAsync(c => c.CategoryId == categoryId);
                if (category == null)
                {
                    return new ApiResponseDto<string>
                    {
                        Success = false,
                        Message = "Category not found",
                        MessageId = "CATEGORY_NOT_FOUND"
                    };
                }

                _unitOfWork.Category.Remove(category);
                await _unitOfWork.SaveAsync();

                return new ApiResponseDto<string>
                {
                    Success = true,
                    Message = "Category deleted successfully"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<string>
                {
                    Success = false,
                    Message = $"Failed to delete category: {ex.Message}",
                    MessageId = "CATEGORY_DELETE_FAILED"
                };
            }
        }
    }
}
