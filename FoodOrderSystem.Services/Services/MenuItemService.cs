using FoodOrderSystem.DataAccess.IRepositories;
using FoodOrderSystem.Models.Domains;
using FoodOrderSystem.Models.DTOs.MenuItem;
using FoodOrderSystem.Models.DTOs.ResponseFormat;
using FoodOrderSystem.Services.IServices;
using FoodOrderSystem.Utilities.Templates.FileUpload;
using Microsoft.AspNetCore.Http;

namespace FoodOrderSystem.Services.Services
{
    public class MenuItemService : IMenuItemService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly FileUploadService _fileUploadService;

        public MenuItemService(
            IUnitOfWork unitOfWork,
            FileUploadService fileUploadService)
        {
            _unitOfWork = unitOfWork;
            _fileUploadService = fileUploadService;
        }

        /// <summary>
        /// GET /api/v1/MenuItem?shopId={guid}
        /// Get all menu items for a shop
        /// Called from: ShopDetailFragment.java:181 (when fetching shop detail)
        /// </summary>
        public async Task<ApiResponseDto<List<GetMenuItemResponseDto>>> GetMenuItemsByShopAsync(Guid shopId)
        {
            try
            {
                if (shopId == Guid.Empty)
                {
                    return new ApiResponseDto<List<GetMenuItemResponseDto>>
                    {
                        Success = false,
                        Message = "Shop ID is required",
                        MessageId = "INVALID_SHOP_ID"
                    };
                }

                // Verify shop exists
                var shop = await _unitOfWork.Shop.GetShopByIdAsync(shopId);
                if (shop == null)
                {
                    return new ApiResponseDto<List<GetMenuItemResponseDto>>
                    {
                        Success = false,
                        Message = "Shop not found",
                        MessageId = "SHOP_NOT_FOUND"
                    };
                }

                var menuItems = await _unitOfWork.MenuItem.GetMenuItemsByShopAsync(shopId);

                var menuItemDtos = menuItems
                    .Select(m => new GetMenuItemResponseDto
                    {
                        MenuItemId = m.MenuItemId,
                        MenuItemName = m.MenuItemName,
                        Description = m.Description,
                        Price = m.Price,
                        ImageUrl = m.ImageUrl,
                        IsAvailable = m.IsAvailable,
                        CategoryId = m.CategoryId,
                        ShopId = m.ShopId,
                        QuantitySold = m.QuantitySold,
                        CreatedDate = m.CreatedDate
                    })
                    .ToList();

                return new ApiResponseDto<List<GetMenuItemResponseDto>>
                {
                    Success = true,
                    Message = "Menu items retrieved successfully",
                    Data = menuItemDtos
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<List<GetMenuItemResponseDto>>
                {
                    Success = false,
                    Message = $"Failed to retrieve menu items: {ex.Message}",
                    MessageId = "MENU_ITEMS_FETCH_FAILED"
                };
            }
        }

        /// <summary>
        /// POST /api/v1/MenuItem (Multipart)
        /// Create new menu item with image
        /// Called from: ShopFormFragment.java - when ShopOwner adds menu item
        /// Only shop owner can add to their shop
        /// </summary>
        public async Task<ApiResponseDto<GetMenuItemResponseDto>> CreateMenuItemAsync(
            string ownerId, CreateMenuItemDto createDto, IFormFile? image)
        {
            try
            {
                if (string.IsNullOrEmpty(ownerId))
                {
                    return new ApiResponseDto<GetMenuItemResponseDto>
                    {
                        Success = false,
                        Message = "Owner ID is required",
                        MessageId = "INVALID_OWNER_ID"
                    };
                }

                // Verify shop exists and owner owns it
                var shop = await _unitOfWork.Shop.GetShopByIdAsync(createDto.ShopId);
                if (shop == null)
                {
                    return new ApiResponseDto<GetMenuItemResponseDto>
                    {
                        Success = false,
                        Message = "Shop not found",
                        MessageId = "SHOP_NOT_FOUND"
                    };
                }

                var shopOwner = await _unitOfWork.ShopOwner.GetAsync(so => so.UserId == ownerId);
                if (shopOwner == null || shop.ShopOwnerId != shopOwner.ShopOwnerId)
                {
                    return new ApiResponseDto<GetMenuItemResponseDto>
                    {
                        Success = false,
                        Message = "Unauthorized to add items to this shop",
                        MessageId = "UNAUTHORIZED"
                    };
                }

                // Verify category exists
                var category = await _unitOfWork.Category.GetAsync(c => c.CategoryId == createDto.CategoryId);
                if (category == null)
                {
                    return new ApiResponseDto<GetMenuItemResponseDto>
                    {
                        Success = false,
                        Message = "Category not found",
                        MessageId = "CATEGORY_NOT_FOUND"
                    };
                }

                // Upload image if provided
                string? imageUrl = null;
                try
                {
                    if (image != null)
                    {
                        imageUrl = await _fileUploadService.SaveFileAsync(image, "menu");
                    }
                }
                catch (Exception ex)
                {
                    return new ApiResponseDto<GetMenuItemResponseDto>
                    {
                        Success = false,
                        Message = $"Image upload failed: {ex.Message}",
                        MessageId = "IMAGE_UPLOAD_FAILED"
                    };
                }

                // Create menu item
                var menuItem = new MenuItem
                {
                    MenuItemId = Guid.NewGuid(),
                    MenuItemName = createDto.MenuItemName,
                    Description = createDto.Description,
                    Price = createDto.Price,
                    ImageUrl = imageUrl,
                    IsAvailable = createDto.IsAvailable,
                    CategoryId = createDto.CategoryId,
                    ShopId = createDto.ShopId,
                    CreatedDate = DateTime.UtcNow,
                    Status = "Active"
                };

                await _unitOfWork.MenuItem.AddAsync(menuItem);
                await _unitOfWork.SaveAsync();

                var response = new GetMenuItemResponseDto
                {
                    MenuItemId = menuItem.MenuItemId,
                    MenuItemName = menuItem.MenuItemName,
                    Description = menuItem.Description,
                    Price = menuItem.Price,
                    ImageUrl = menuItem.ImageUrl,
                    IsAvailable = menuItem.IsAvailable,
                    CategoryId = menuItem.CategoryId,
                    ShopId = menuItem.ShopId,
                    QuantitySold = 0,
                    CreatedDate = menuItem.CreatedDate
                };

                return new ApiResponseDto<GetMenuItemResponseDto>
                {
                    Success = true,
                    Message = "Menu item created successfully",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<GetMenuItemResponseDto>
                {
                    Success = false,
                    Message = $"Failed to create menu item: {ex.Message}",
                    MessageId = "MENU_ITEM_CREATE_FAILED"
                };
            }
        }

        /// <summary>
        /// PUT /api/v1/MenuItem/{menuItemId} (Multipart)
        /// Update menu item - image is optional
        /// Called from: ShopFormFragment.java - when ShopOwner updates menu item
        /// Only shop owner can update
        /// </summary>
        public async Task<ApiResponseDto<string>> UpdateMenuItemAsync(
            string ownerId, UpdateMenuItemDto updateDto, IFormFile? image)
        {
            try
            {
                if (string.IsNullOrEmpty(ownerId))
                {
                    return new ApiResponseDto<string>
                    {
                        Success = false,
                        Message = "Owner ID is required",
                        MessageId = "INVALID_OWNER_ID"
                    };
                }

                // Get menu item
                var menuItem = await _unitOfWork.MenuItem.GetAsync(m => m.MenuItemId == updateDto.MenuItemId);
                if (menuItem == null)
                {
                    return new ApiResponseDto<string>
                    {
                        Success = false,
                        Message = "Menu item not found",
                        MessageId = "MENU_ITEM_NOT_FOUND"
                    };
                }

                // Verify ownership (owner must own the shop)
                var shop = await _unitOfWork.Shop.GetShopByIdAsync(menuItem.ShopId);
                if (shop == null)
                {
                    return new ApiResponseDto<string>
                    {
                        Success = false,
                        Message = "Shop not found",
                        MessageId = "SHOP_NOT_FOUND"
                    };
                }

                var shopOwner = await _unitOfWork.ShopOwner.GetAsync(so => so.UserId == ownerId);
                if (shopOwner == null || shop.ShopOwnerId != shopOwner.ShopOwnerId)
                {
                    return new ApiResponseDto<string>
                    {
                        Success = false,
                        Message = "Unauthorized to update this item",
                        MessageId = "UNAUTHORIZED"
                    };
                }

                // Verify category exists
                var category = await _unitOfWork.Category.GetAsync(c => c.CategoryId == updateDto.CategoryId);
                if (category == null)
                {
                    return new ApiResponseDto<string>
                    {
                        Success = false,
                        Message = "Category not found",
                        MessageId = "CATEGORY_NOT_FOUND"
                    };
                }

                // Update image if provided
                try
                {
                    if (image != null)
                    {
                        // Delete old image
                        if (!string.IsNullOrEmpty(menuItem.ImageUrl))
                        {
                            _fileUploadService.DeleteFile(menuItem.ImageUrl);
                        }

                        menuItem.ImageUrl = await _fileUploadService.SaveFileAsync(image, "menu");
                    }
                }
                catch (Exception ex)
                {
                    return new ApiResponseDto<string>
                    {
                        Success = false,
                        Message = $"Image upload failed: {ex.Message}",
                        MessageId = "IMAGE_UPLOAD_FAILED"
                    };
                }

                // Update fields
                menuItem.MenuItemName = updateDto.MenuItemName;
                menuItem.Description = updateDto.Description;
                menuItem.Price = updateDto.Price;
                menuItem.CategoryId = updateDto.CategoryId;
                menuItem.IsAvailable = updateDto.IsAvailable;
                menuItem.UpdatedDate = DateTime.UtcNow;

                _unitOfWork.MenuItem.Update(menuItem);
                await _unitOfWork.SaveAsync();

                return new ApiResponseDto<string>
                {
                    Success = true,
                    Message = "Menu item updated successfully"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<string>
                {
                    Success = false,
                    Message = $"Failed to update menu item: {ex.Message}",
                    MessageId = "MENU_ITEM_UPDATE_FAILED"
                };
            }
        }

        /// <summary>
        /// DELETE /api/v1/MenuItem/{menuItemId}
        /// Delete menu item (owner only)
        /// Called from: ShopFormFragment.java - when ShopOwner removes menu item
        /// </summary>
        public async Task<ApiResponseDto<string>> DeleteMenuItemAsync(string ownerId, Guid menuItemId)
        {
            try
            {
                if (string.IsNullOrEmpty(ownerId) || menuItemId == Guid.Empty)
                {
                    return new ApiResponseDto<string>
                    {
                        Success = false,
                        Message = "Owner ID and Menu Item ID are required",
                        MessageId = "INVALID_REQUEST"
                    };
                }

                // Get menu item
                var menuItem = await _unitOfWork.MenuItem.GetAsync(m => m.MenuItemId == menuItemId);
                if (menuItem == null)
                {
                    return new ApiResponseDto<string>
                    {
                        Success = false,
                        Message = "Menu item not found",
                        MessageId = "MENU_ITEM_NOT_FOUND"
                    };
                }

                // Verify ownership
                var shop = await _unitOfWork.Shop.GetShopByIdAsync(menuItem.ShopId);
                if (shop == null)
                {
                    return new ApiResponseDto<string>
                    {
                        Success = false,
                        Message = "Shop not found",
                        MessageId = "SHOP_NOT_FOUND"
                    };
                }

                var shopOwner = await _unitOfWork.ShopOwner.GetAsync(so => so.UserId == ownerId);
                if (shopOwner == null || shop.ShopOwnerId != shopOwner.ShopOwnerId)
                {
                    return new ApiResponseDto<string>
                    {
                        Success = false,
                        Message = "Unauthorized to delete this item",
                        MessageId = "UNAUTHORIZED"
                    };
                }

                // Delete image if exists
                if (!string.IsNullOrEmpty(menuItem.ImageUrl))
                {
                    _fileUploadService.DeleteFile(menuItem.ImageUrl);
                }

                _unitOfWork.MenuItem.Remove(menuItem);
                await _unitOfWork.SaveAsync();

                return new ApiResponseDto<string>
                {
                    Success = true,
                    Message = "Menu item deleted successfully"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<string>
                {
                    Success = false,
                    Message = $"Failed to delete menu item: {ex.Message}",
                    MessageId = "MENU_ITEM_DELETE_FAILED"
                };
            }
        }
    }
}
