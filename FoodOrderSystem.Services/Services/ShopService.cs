using AutoMapper;
using FoodOrderSystem.DataAccess.IRepositories;
using FoodOrderSystem.Models.Domains;
using FoodOrderSystem.Models.DTOs.ResponseFormat;
using FoodOrderSystem.Models.DTOs.Shop;
using FoodOrderSystem.Services.IServices;
using FoodOrderSystem.Utilities.Templates.FileUpload;
using Microsoft.AspNetCore.Http;

namespace FoodOrderSystem.Services.Services
{
    public class ShopService : IShopService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly FileUploadService _fileUploadService;

        public ShopService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            FileUploadService fileUploadService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _fileUploadService = fileUploadService;
        }

        /// <summary>
        /// GET /api/v1/shops/popular?currentTime={HH:mm:ss}
        /// Get popular shops for home screen
        /// Called from: HomeFragment.java:246
        /// NOTE: Returns LIST directly (NOT wrapped in ApiResponse per Android expectations)
        /// </summary>
        public async Task<ApiResponseDto<List<PopularShopResponseDto>>> GetPopularShopsAsync()
        {
            try
            {
                var shops = await _unitOfWork.Shop.GetPopularShopsAsync();

                var shopList = shops
                    .Select(s => new PopularShopResponseDto
                    {
                        ShopId = s.ShopId,
                        ShopName = s.ShopName,
                        ImageUrl = s.ImageUrl,
                        Address = s.Address,
                        Latitude = s.Latitude,
                        Longitude = s.Longitude,
                        OpenHours = s.OpenHours.ToString(@"hh\:mm\:ss"),
                        CloseHours = s.CloseHours.ToString(@"hh\:mm\:ss"),
                        TotalOrders = s.TotalOrders,
                        AverageRating = s.AverageRating,
                        Description = s.Description
                    })
                    .ToList();

                return new ApiResponseDto<List<PopularShopResponseDto>>
                {
                    Success = true,
                    Message = "Popular shops retrieved successfully",
                    Data = shopList
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<List<PopularShopResponseDto>>
                {
                    Success = false,
                    Message = $"Failed to retrieve shops: {ex.Message}",
                    MessageId = "SHOPS_FETCH_FAILED"
                };
            }
        }

        /// <summary>
        /// GET /api/v1/shops/detail/{shopId}
        /// Get shop detail with nested categories and menu items
        /// Called from: ShopDetailFragment.java:181
        /// Must include: categories[] with nested menuItems[]
        /// </summary>
        public async Task<ApiResponseDto<ShopDetailResponseDto>> GetShopDetailAsync(Guid shopId)
        {
            try
            {
                if (shopId == Guid.Empty)
                {
                    return new ApiResponseDto<ShopDetailResponseDto>
                    {
                        Success = false,
                        Message = "Invalid shop ID",
                        MessageId = "INVALID_SHOP_ID"
                    };
                }

                var shop = await _unitOfWork.Shop.GetShopWithMenuAsync(shopId);
                if (shop == null)
                {
                    return new ApiResponseDto<ShopDetailResponseDto>
                    {
                        Success = false,
                        Message = "Shop not found",
                        MessageId = "SHOP_NOT_FOUND"
                    };
                }

                // Group menu items by category
                var categoriesWithItems = new List<CategoryWithMenuItemsDto>();

                if (shop.MenuItems != null && shop.MenuItems.Any())
                {
                    var groupedByCategory = shop.MenuItems
                        .GroupBy(m => m.Category)
                        .OrderBy(g => g.Key.CategoryName);

                    foreach (var group in groupedByCategory)
                    {
                        var categoryDto = new CategoryWithMenuItemsDto
                        {
                            CategoryId = group.Key.CategoryId,
                            CategoryName = group.Key.CategoryName,
                            ImageUrl = group.Key.ImageUrl,
                            MenuItems = group
                                .Select(m => new MenuItemResponseDto
                                {
                                    MenuItemId = m.MenuItemId,
                                    MenuItemName = m.MenuItemName,
                                    Description = m.Description,
                                    Price = m.Price,
                                    ImageUrl = m.ImageUrl,
                                    IsAvailable = m.IsAvailable,
                                    CategoryId = m.CategoryId,
                                    QuantitySold = m.QuantitySold
                                })
                                .ToList()
                        };
                        categoriesWithItems.Add(categoryDto);
                    }
                }

                var response = new ShopDetailResponseDto
                {
                    ShopId = shop.ShopId,
                    ShopName = shop.ShopName,
                    Description = shop.Description,
                    Address = shop.Address,
                    ImageUrl = shop.ImageUrl,
                    BusinessLicenseImageUrl = shop.BusinessLicenseImageUrl,
                    Latitude = shop.Latitude,
                    Longitude = shop.Longitude,
                    OpenHours = shop.OpenHours.ToString(@"hh\:mm\:ss"),
                    CloseHours = shop.CloseHours.ToString(@"hh\:mm\:ss"),
                    TotalOrders = shop.TotalOrders,
                    AverageRating = shop.AverageRating,
                    Categories = categoriesWithItems
                };

                return new ApiResponseDto<ShopDetailResponseDto>
                {
                    Success = true,
                    Message = "Shop detail retrieved successfully",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<ShopDetailResponseDto>
                {
                    Success = false,
                    Message = $"Failed to retrieve shop detail: {ex.Message}",
                    MessageId = "SHOP_DETAIL_FETCH_FAILED"
                };
            }
        }

        /// <summary>
        /// GET /api/v1/shops/shop-owner?PageIndex={int}&PageSize={int}
        /// Get shops owned by current user (ShopOwner)
        /// Called from: MyShopListFragment.java:139
        /// </summary>
        public async Task<ApiResponseDto<PagingResponseDto<GetShopResponseDto>>> GetShopsByOwnerAsync(
            string ownerId, int pageIndex, int pageSize)
        {
            try
            {
                if (string.IsNullOrEmpty(ownerId))
                {
                    return new ApiResponseDto<PagingResponseDto<GetShopResponseDto>>
                    {
                        Success = false,
                        Message = "Owner ID is required",
                        MessageId = "INVALID_OWNER_ID"
                    };
                }

                // Get ShopOwner to verify exists
                var shopOwner = await _unitOfWork.ShopOwner.GetAsync(so => so.UserId == ownerId);
                if (shopOwner == null)
                {
                    return new ApiResponseDto<PagingResponseDto<GetShopResponseDto>>
                    {
                        Success = false,
                        Message = "Shop owner not found",
                        MessageId = "SHOP_OWNER_NOT_FOUND"
                    };
                }

                var shops = await _unitOfWork.Shop.GetShopsByOwnerAsync(shopOwner.ShopOwnerId);

                // Apply pagination
                var total = shops.Count;
                var paginated = shops
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var shopDtos = paginated
                    .Select(s => new GetShopResponseDto
                    {
                        ShopId = s.ShopId,
                        ShopName = s.ShopName,
                        ImageUrl = s.ImageUrl,
                        Address = s.Address,
                        Status = s.Status,
                        Latitude = s.Latitude,
                        Longitude = s.Longitude,
                        OpenHours = s.OpenHours.ToString(@"hh\:mm\:ss"),
                        CloseHours = s.CloseHours.ToString(@"hh\:mm\:ss"),
                        CreatedDate = s.CreatedDate,
                        ApprovedDate = s.ApprovedDate,
                        TotalOrders = s.TotalOrders,
                        AverageRating = s.AverageRating
                    })
                    .ToList();

                var response = new PagingResponseDto<GetShopResponseDto>
                {
                    Items = shopDtos,
                    TotalItems = total,
                    PageIndex = pageIndex,
                    PageSize = pageSize
                };

                return new ApiResponseDto<PagingResponseDto<GetShopResponseDto>>
                {
                    Success = true,
                    Message = "Shops retrieved successfully",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<PagingResponseDto<GetShopResponseDto>>
                {
                    Success = false,
                    Message = $"Failed to retrieve shops: {ex.Message}",
                    MessageId = "SHOPS_FETCH_FAILED"
                };
            }
        }

        /// <summary>
        /// GET /api/v1/shops/status/{status}?PageIndex={int}&PageSize={int}
        /// Get shops by status (Admin only)
        /// Status values: "Pending", "Approved", "Rejected"
        /// Called from: AdminShopListFragment.java:108
        /// </summary>
        public async Task<ApiResponseDto<PagingResponseDto<GetShopResponseDto>>> GetShopsByStatusAsync(
            string status, int pageIndex, int pageSize)
        {
            try
            {
                // Validate status
                var validStatuses = new[] { "Pending", "Approved", "Rejected" };
                if (!validStatuses.Contains(status))
                {
                    return new ApiResponseDto<PagingResponseDto<GetShopResponseDto>>
                    {
                        Success = false,
                        Message = $"Invalid status. Must be: {string.Join(", ", validStatuses)}",
                        MessageId = "INVALID_STATUS"
                    };
                }

                var (shops, total) = await _unitOfWork.Shop.GetShopsByStatusAsync(status, pageIndex, pageSize);

                var shopDtos = shops
                    .Select(s => new GetShopResponseDto
                    {
                        ShopId = s.ShopId,
                        ShopName = s.ShopName,
                        ImageUrl = s.ImageUrl,
                        Address = s.Address,
                        Status = s.Status,
                        Latitude = s.Latitude,
                        Longitude = s.Longitude,
                        OpenHours = s.OpenHours.ToString(@"hh\:mm\:ss"),
                        CloseHours = s.CloseHours.ToString(@"hh\:mm\:ss"),
                        CreatedDate = s.CreatedDate,
                        ApprovedDate = s.ApprovedDate,
                        TotalOrders = s.TotalOrders,
                        AverageRating = s.AverageRating
                    })
                    .ToList();

                var response = new PagingResponseDto<GetShopResponseDto>
                {
                    Items = shopDtos,
                    TotalItems = total,
                    PageIndex = pageIndex,
                    PageSize = pageSize
                };

                return new ApiResponseDto<PagingResponseDto<GetShopResponseDto>>
                {
                    Success = true,
                    Message = $"Shops with status '{status}' retrieved successfully",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<PagingResponseDto<GetShopResponseDto>>
                {
                    Success = false,
                    Message = $"Failed to retrieve shops: {ex.Message}",
                    MessageId = "SHOPS_FETCH_FAILED"
                };
            }
        }

        /// <summary>
        /// POST /api/v1/shops (Multipart)
        /// Create new shop with image upload
        /// Called from: ShopFormFragment.java:251
        /// The ShopOwner record converts UserId to ShopOwnerId internally
        /// </summary>
        public async Task<ApiResponseDto<GetShopResponseDto>> CreateShopAsync(
            string ownerId, CreateShopDto createDto, IFormFile? image, IFormFile? businessLicense)
        {
            try
            {
                if (string.IsNullOrEmpty(ownerId))
                {
                    return new ApiResponseDto<GetShopResponseDto>
                    {
                        Success = false,
                        Message = "Owner ID is required",
                        MessageId = "INVALID_OWNER_ID"
                    };
                }

                // Get ShopOwner record
                var shopOwner = await _unitOfWork.ShopOwner.GetAsync(so => so.UserId == ownerId);
                if (shopOwner == null)
                {
                    return new ApiResponseDto<GetShopResponseDto>
                    {
                        Success = false,
                        Message = "Shop owner not found",
                        MessageId = "SHOP_OWNER_NOT_FOUND"
                    };
                }

                // Validate hours format
                if (!TimeSpan.TryParse(createDto.OpenHours, out var openHours) ||
                    !TimeSpan.TryParse(createDto.CloseHours, out var closeHours))
                {
                    return new ApiResponseDto<GetShopResponseDto>
                    {
                        Success = false,
                        Message = "Invalid time format. Use HH:mm:ss",
                        MessageId = "INVALID_TIME_FORMAT"
                    };
                }

                // Upload images if provided
                string? imageUrl = null;
                string? businessLicenseUrl = null;

                try
                {
                    if (image != null)
                    {
                        imageUrl = await _fileUploadService.SaveFileAsync(image, "shop");
                    }

                    if (businessLicense != null)
                    {
                        businessLicenseUrl = await _fileUploadService.SaveFileAsync(businessLicense, "shop-license");
                    }
                }
                catch (Exception ex)
                {
                    return new ApiResponseDto<GetShopResponseDto>
                    {
                        Success = false,
                        Message = $"Image upload failed: {ex.Message}",
                        MessageId = "IMAGE_UPLOAD_FAILED"
                    };
                }

                // Create Shop entity
                var shop = new Shop
                {
                    ShopId = Guid.NewGuid(),
                    ShopName = createDto.ShopName,
                    Description = createDto.Description,
                    Address = createDto.Address,
                    ImageUrl = imageUrl,
                    BusinessLicenseImageUrl = businessLicenseUrl,
                    Latitude = createDto.Latitude,
                    Longitude = createDto.Longitude,
                    OpenHours = openHours,
                    CloseHours = closeHours,
                    Status = "Pending",  // New shops start as Pending
                    ShopOwnerId = shopOwner.ShopOwnerId,
                    CreatedDate = DateTime.UtcNow
                };

                await _unitOfWork.Shop.AddAsync(shop);
                await _unitOfWork.SaveAsync();

                var response = new GetShopResponseDto
                {
                    ShopId = shop.ShopId,
                    ShopName = shop.ShopName,
                    ImageUrl = shop.ImageUrl,
                    Address = shop.Address,
                    Status = shop.Status,
                    Latitude = shop.Latitude,
                    Longitude = shop.Longitude,
                    OpenHours = shop.OpenHours.ToString(@"hh\:mm\:ss"),
                    CloseHours = shop.CloseHours.ToString(@"hh\:mm\:ss"),
                    CreatedDate = shop.CreatedDate,
                    TotalOrders = 0,
                    AverageRating = 0
                };

                return new ApiResponseDto<GetShopResponseDto>
                {
                    Success = true,
                    Message = "Shop created successfully. Waiting for admin approval.",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<GetShopResponseDto>
                {
                    Success = false,
                    Message = $"Failed to create shop: {ex.Message}",
                    MessageId = "SHOP_CREATE_FAILED"
                };
            }
        }

        /// <summary>
        /// PUT /api/v1/shops (Multipart)
        /// Update shop - files are optional
        /// Called from: ShopFormFragment.java:249
        /// Only the owner can update their shop
        /// </summary>
        public async Task<ApiResponseDto<string>> UpdateShopAsync(
            string ownerId, UpdateShopDto updateDto, IFormFile? image, IFormFile? businessLicense)
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

                // Get shop
                var shop = await _unitOfWork.Shop.GetShopByIdAsync(updateDto.ShopId);
                if (shop == null)
                {
                    return new ApiResponseDto<string>
                    {
                        Success = false,
                        Message = "Shop not found",
                        MessageId = "SHOP_NOT_FOUND"
                    };
                }

                // Verify ownership
                var shopOwner = await _unitOfWork.ShopOwner.GetAsync(so => so.UserId == ownerId);
                if (shopOwner == null || shop.ShopOwnerId != shopOwner.ShopOwnerId)
                {
                    return new ApiResponseDto<string>
                    {
                        Success = false,
                        Message = "Unauthorized to update this shop",
                        MessageId = "UNAUTHORIZED"
                    };
                }

                // Validate hours format
                if (!TimeSpan.TryParse(updateDto.OpenHours, out var openHours) ||
                    !TimeSpan.TryParse(updateDto.CloseHours, out var closeHours))
                {
                    return new ApiResponseDto<string>
                    {
                        Success = false,
                        Message = "Invalid time format. Use HH:mm:ss",
                        MessageId = "INVALID_TIME_FORMAT"
                    };
                }

                // Update images if provided
                try
                {
                    if (image != null)
                    {
                        // Delete old image
                        if (!string.IsNullOrEmpty(shop.ImageUrl))
                        {
                            _fileUploadService.DeleteFile(shop.ImageUrl);
                        }

                        shop.ImageUrl = await _fileUploadService.SaveFileAsync(image, "shop");
                    }

                    if (businessLicense != null)
                    {
                        // Delete old license
                        if (!string.IsNullOrEmpty(shop.BusinessLicenseImageUrl))
                        {
                            _fileUploadService.DeleteFile(shop.BusinessLicenseImageUrl);
                        }

                        shop.BusinessLicenseImageUrl = await _fileUploadService.SaveFileAsync(businessLicense, "shop-license");
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

                // Update shop fields
                shop.ShopName = updateDto.ShopName;
                shop.Description = updateDto.Description;
                shop.Address = updateDto.Address;
                shop.Latitude = updateDto.Latitude;
                shop.Longitude = updateDto.Longitude;
                shop.OpenHours = openHours;
                shop.CloseHours = closeHours;
                shop.UpdatedDate = DateTime.UtcNow;

                _unitOfWork.Shop.Update(shop);
                await _unitOfWork.SaveAsync();

                return new ApiResponseDto<string>
                {
                    Success = true,
                    Message = "Shop updated successfully"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<string>
                {
                    Success = false,
                    Message = $"Failed to update shop: {ex.Message}",
                    MessageId = "SHOP_UPDATE_FAILED"
                };
            }
        }

        /// <summary>
        /// DELETE /api/v1/shops?shopId={shopId}
        /// Delete shop (owner only)
        /// Called from: MyShopListFragment.java:71
        /// </summary>
        public async Task<ApiResponseDto<string>> DeleteShopAsync(Guid shopId, string ownerId)
        {
            try
            {
                if (shopId == Guid.Empty || string.IsNullOrEmpty(ownerId))
                {
                    return new ApiResponseDto<string>
                    {
                        Success = false,
                        Message = "Shop ID and Owner ID are required",
                        MessageId = "INVALID_REQUEST"
                    };
                }

                var shop = await _unitOfWork.Shop.GetShopByIdAsync(shopId);
                if (shop == null)
                {
                    return new ApiResponseDto<string>
                    {
                        Success = false,
                        Message = "Shop not found",
                        MessageId = "SHOP_NOT_FOUND"
                    };
                }

                // Verify ownership
                var shopOwner = await _unitOfWork.ShopOwner.GetAsync(so => so.UserId == ownerId);
                if (shopOwner == null || shop.ShopOwnerId != shopOwner.ShopOwnerId)
                {
                    return new ApiResponseDto<string>
                    {
                        Success = false,
                        Message = "Unauthorized to delete this shop",
                        MessageId = "UNAUTHORIZED"
                    };
                }

                // Delete associated images
                if (!string.IsNullOrEmpty(shop.ImageUrl))
                {
                    _fileUploadService.DeleteFile(shop.ImageUrl);
                }

                if (!string.IsNullOrEmpty(shop.BusinessLicenseImageUrl))
                {
                    _fileUploadService.DeleteFile(shop.BusinessLicenseImageUrl);
                }

                _unitOfWork.Shop.Remove(shop);
                await _unitOfWork.SaveAsync();

                return new ApiResponseDto<string>
                {
                    Success = true,
                    Message = "Shop deleted successfully"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<string>
                {
                    Success = false,
                    Message = $"Failed to delete shop: {ex.Message}",
                    MessageId = "SHOP_DELETE_FAILED"
                };
            }
        }

        /// <summary>
        /// POST /api/v1/shops/approve-reject
        /// Approve or reject shop (Admin only)
        /// Called from: AdminShopListFragment.java:112, 116
        /// </summary>
        public async Task<ApiResponseDto<string>> ApproveRejectShopAsync(Guid shopId, bool isApproved)
        {
            try
            {
                if (shopId == Guid.Empty)
                {
                    return new ApiResponseDto<string>
                    {
                        Success = false,
                        Message = "Shop ID is required",
                        MessageId = "INVALID_SHOP_ID"
                    };
                }

                var shop = await _unitOfWork.Shop.GetShopByIdAsync(shopId);
                if (shop == null)
                {
                    return new ApiResponseDto<string>
                    {
                        Success = false,
                        Message = "Shop not found",
                        MessageId = "SHOP_NOT_FOUND"
                    };
                }

                // Only Pending shops can be approved/rejected
                if (shop.Status != "Pending")
                {
                    return new ApiResponseDto<string>
                    {
                        Success = false,
                        Message = $"Cannot change status of {shop.Status} shop",
                        MessageId = "INVALID_STATUS_CHANGE"
                    };
                }

                shop.Status = isApproved ? "Approved" : "Rejected";
                shop.ApprovedDate = DateTime.UtcNow;
                shop.UpdatedDate = DateTime.UtcNow;

                _unitOfWork.Shop.Update(shop);
                await _unitOfWork.SaveAsync();

                return new ApiResponseDto<string>
                {
                    Success = true,
                    Message = $"Shop {(isApproved ? "approved" : "rejected")} successfully"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<string>
                {
                    Success = false,
                    Message = $"Failed to approve/reject shop: {ex.Message}",
                    MessageId = "APPROVAL_FAILED"
                };
            }
        }
    }
}
