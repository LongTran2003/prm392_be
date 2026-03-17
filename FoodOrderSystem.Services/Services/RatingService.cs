using AutoMapper;
using FoodOrderSystem.DataAccess.IRepositories;
using FoodOrderSystem.Models.Domains;
using FoodOrderSystem.Models.DTOs.Rating;
using FoodOrderSystem.Models.DTOs.ResponseFormat;
using FoodOrderSystem.Services.IServices;
using FoodOrderSystem.Utilities.Templates.FileUpload;
using Microsoft.AspNetCore.Http;

namespace FoodOrderSystem.Services.Services
{
    public class RatingService : IRatingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly FileUploadService _fileUploadService;

        public RatingService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            FileUploadService fileUploadService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _fileUploadService = fileUploadService;
        }

        /// <summary>
        /// POST /api/v1/Ratings
        /// Create rating/review with optional images
        /// Called from: CustomerFeedbackFragment.java
        /// </summary>
        public async Task<ApiResponseDto<RatingResponseDto>> CreateRatingAsync(
            string customerId, CreateRatingDto createDto, List<IFormFile>? images)
        {
            try
            {
                if (string.IsNullOrEmpty(customerId))
                {
                    return new ApiResponseDto<RatingResponseDto>
                    {
                        Success = false,
                        Message = "Customer ID is required",
                        MessageId = "INVALID_CUSTOMER_ID"
                    };
                }

                // Verify shop exists
                var shop = await _unitOfWork.Shop.GetShopByIdAsync(createDto.ShopId);
                if (shop == null)
                {
                    return new ApiResponseDto<RatingResponseDto>
                    {
                        Success = false,
                        Message = "Shop not found",
                        MessageId = "SHOP_NOT_FOUND"
                    };
                }

                // Check if customer already rated this shop
                var existingRating = await _unitOfWork.Rating.GetCustomerRatingAsync(createDto.ShopId, customerId);
                if (existingRating != null)
                {
                    return new ApiResponseDto<RatingResponseDto>
                    {
                        Success = false,
                        Message = "You have already rated this shop",
                        MessageId = "RATING_ALREADY_EXISTS"
                    };
                }

                // Get customer info
                var customer = await _unitOfWork.User.GetUserByIdAsync(customerId);
                if (customer == null)
                {
                    return new ApiResponseDto<RatingResponseDto>
                    {
                        Success = false,
                        Message = "Customer not found",
                        MessageId = "CUSTOMER_NOT_FOUND"
                    };
                }

                // Create rating
                var rating = new Rating
                {
                    RatingId = Guid.NewGuid(),
                    ShopId = createDto.ShopId,
                    CustomerId = customerId,
                    StarRating = createDto.StarRating,
                    Comment = createDto.Comment,
                    CreatedDate = DateTime.UtcNow
                };

                await _unitOfWork.Rating.AddAsync(rating);

                // Upload images if provided
                var imageUrls = new List<string>();
                if (images != null && images.Count > 0)
                {
                    foreach (var image in images)
                    {
                        if (image.Length > 0)
                        {
                            var uploadResult = await _fileUploadService.UploadFileAsync(image, "ratings");
                            if (uploadResult.Success)
                            {
                                var ratingImage = new RatingImage
                                {
                                    RatingImageId = Guid.NewGuid(),
                                    RatingId = rating.RatingId,
                                    ImageUrl = uploadResult.Url,
                                    CreatedDate = DateTime.UtcNow
                                };

                                await _unitOfWork.RatingImage.AddAsync(ratingImage);
                                imageUrls.Add(uploadResult.Url);
                            }
                        }
                    }
                }

                await _unitOfWork.SaveAsync();

                // Build response
                var response = new RatingResponseDto
                {
                    RatingId = rating.RatingId,
                    ShopId = rating.ShopId,
                    CustomerName = customer.FullName,
                    CustomerAvatar = customer.ImageUrl,
                    StarRating = rating.StarRating,
                    Comment = rating.Comment,
                    ImageUrls = imageUrls,
                    CreatedDate = rating.CreatedDate
                };

                return new ApiResponseDto<RatingResponseDto>
                {
                    Success = true,
                    Message = "Rating created successfully",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<RatingResponseDto>
                {
                    Success = false,
                    Message = $"Failed to create rating: {ex.Message}",
                    MessageId = "RATING_CREATE_FAILED"
                };
            }
        }

        /// <summary>
        /// GET /api/v1/Ratings/{shopId}
        /// Get all ratings for a shop with pagination
        /// Called from: ShopDetailFragment.java
        /// </summary>
        public async Task<ApiResponseDto<List<RatingResponseDto>>> GetShopRatingsAsync(Guid shopId, int pageIndex = 1, int pageSize = 10)
        {
            try
            {
                if (shopId == Guid.Empty)
                {
                    return new ApiResponseDto<List<RatingResponseDto>>
                    {
                        Success = false,
                        Message = "Invalid shop ID",
                        MessageId = "INVALID_SHOP_ID"
                    };
                }

                var ratings = await _unitOfWork.Rating.GetRatingsByShopAsync(shopId);

                // Apply pagination
                var paginatedRatings = ratings
                    .OrderByDescending(r => r.CreatedDate)
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var ratingDtos = new List<RatingResponseDto>();
                foreach (var rating in paginatedRatings)
                {
                    var customer = await _unitOfWork.User.GetUserByIdAsync(rating.CustomerId);
                    var images = await _unitOfWork.RatingImage.GetListAsync(ri => ri.RatingId == rating.RatingId);

                    ratingDtos.Add(new RatingResponseDto
                    {
                        RatingId = rating.RatingId,
                        ShopId = rating.ShopId,
                        CustomerName = customer?.FullName ?? "Anonymous",
                        CustomerAvatar = customer?.ImageUrl,
                        StarRating = rating.StarRating,
                        Comment = rating.Comment,
                        ImageUrls = images.Select(img => img.ImageUrl).ToList(),
                        CreatedDate = rating.CreatedDate
                    });
                }

                return new ApiResponseDto<List<RatingResponseDto>>
                {
                    Success = true,
                    Message = "Ratings retrieved successfully",
                    Data = ratingDtos
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<List<RatingResponseDto>>
                {
                    Success = false,
                    Message = $"Failed to retrieve ratings: {ex.Message}",
                    MessageId = "RATINGS_FETCH_FAILED"
                };
            }
        }

        /// <summary>
        /// GET /api/v1/Ratings/summary/{shopId}
        /// Get rating summary with average, breakdown, and recent ratings
        /// Called from: ShopDetailFragment.java
        /// </summary>
        public async Task<ApiResponseDto<ShopRatingSummaryDto>> GetRatingSummaryAsync(Guid shopId)
        {
            try
            {
                if (shopId == Guid.Empty)
                {
                    return new ApiResponseDto<ShopRatingSummaryDto>
                    {
                        Success = false,
                        Message = "Invalid shop ID",
                        MessageId = "INVALID_SHOP_ID"
                    };
                }

                var averageRating = await _unitOfWork.Rating.GetAverageRatingAsync(shopId);
                var ratingBreakdown = await _unitOfWork.Rating.GetRatingBreakdownAsync(shopId);
                var ratings = await _unitOfWork.Rating.GetRatingsByShopAsync(shopId);

                var recentRatings = ratings
                    .OrderByDescending(r => r.CreatedDate)
                    .Take(10)
                    .ToList();

                var recentRatingDtos = new List<RatingResponseDto>();
                foreach (var rating in recentRatings)
                {
                    var customer = await _unitOfWork.User.GetUserByIdAsync(rating.CustomerId);
                    var images = await _unitOfWork.RatingImage.GetListAsync(ri => ri.RatingId == rating.RatingId);

                    recentRatingDtos.Add(new RatingResponseDto
                    {
                        RatingId = rating.RatingId,
                        ShopId = rating.ShopId,
                        CustomerName = customer?.FullName ?? "Anonymous",
                        CustomerAvatar = customer?.ImageUrl,
                        StarRating = rating.StarRating,
                        Comment = rating.Comment,
                        ImageUrls = images.Select(img => img.ImageUrl).ToList(),
                        CreatedDate = rating.CreatedDate
                    });
                }

                var summary = new ShopRatingSummaryDto
                {
                    ShopId = shopId,
                    AverageRating = Math.Round(averageRating, 1),
                    TotalRatings = ratings.Count,
                    RatingBreakdown = ratingBreakdown,
                    RecentRatings = recentRatingDtos
                };

                return new ApiResponseDto<ShopRatingSummaryDto>
                {
                    Success = true,
                    Message = "Rating summary retrieved successfully",
                    Data = summary
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<ShopRatingSummaryDto>
                {
                    Success = false,
                    Message = $"Failed to retrieve rating summary: {ex.Message}",
                    MessageId = "RATING_SUMMARY_FETCH_FAILED"
                };
            }
        }

        /// <summary>
        /// PUT /api/v1/Ratings/{ratingId}
        /// Update rating by customer
        /// Called from: RatingDetailFragment.java
        /// </summary>
        public async Task<ApiResponseDto<string>> UpdateRatingAsync(
            string customerId, Guid ratingId, CreateRatingDto updateDto)
        {
            try
            {
                if (string.IsNullOrEmpty(customerId) || ratingId == Guid.Empty)
                {
                    return new ApiResponseDto<string>
                    {
                        Success = false,
                        Message = "Invalid customer ID or rating ID",
                        MessageId = "INVALID_REQUEST"
                    };
                }

                var rating = await _unitOfWork.Rating.GetAsync(r => r.RatingId == ratingId);
                if (rating == null)
                {
                    return new ApiResponseDto<string>
                    {
                        Success = false,
                        Message = "Rating not found",
                        MessageId = "RATING_NOT_FOUND"
                    };
                }

                // Verify customer owns this rating
                if (rating.CustomerId != customerId)
                {
                    return new ApiResponseDto<string>
                    {
                        Success = false,
                        Message = "You are not authorized to update this rating",
                        MessageId = "UNAUTHORIZED"
                    };
                }

                // Update rating
                rating.StarRating = updateDto.StarRating;
                rating.Comment = updateDto.Comment;
                rating.UpdatedDate = DateTime.UtcNow;

                _unitOfWork.Rating.Update(rating);
                await _unitOfWork.SaveAsync();

                return new ApiResponseDto<string>
                {
                    Success = true,
                    Message = "Rating updated successfully",
                    Data = "OK"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<string>
                {
                    Success = false,
                    Message = $"Failed to update rating: {ex.Message}",
                    MessageId = "RATING_UPDATE_FAILED"
                };
            }
        }

        /// <summary>
        /// DELETE /api/v1/Ratings/{ratingId}
        /// Delete rating by customer
        /// Called from: RatingDetailFragment.java
        /// </summary>
        public async Task<ApiResponseDto<string>> DeleteRatingAsync(string customerId, Guid ratingId)
        {
            try
            {
                if (string.IsNullOrEmpty(customerId) || ratingId == Guid.Empty)
                {
                    return new ApiResponseDto<string>
                    {
                        Success = false,
                        Message = "Invalid customer ID or rating ID",
                        MessageId = "INVALID_REQUEST"
                    };
                }

                var rating = await _unitOfWork.Rating.GetAsync(r => r.RatingId == ratingId);
                if (rating == null)
                {
                    return new ApiResponseDto<string>
                    {
                        Success = false,
                        Message = "Rating not found",
                        MessageId = "RATING_NOT_FOUND"
                    };
                }

                // Verify customer owns this rating
                if (rating.CustomerId != customerId)
                {
                    return new ApiResponseDto<string>
                    {
                        Success = false,
                        Message = "You are not authorized to delete this rating",
                        MessageId = "UNAUTHORIZED"
                    };
                }

                // Delete rating images
                var images = await _unitOfWork.RatingImage.GetListAsync(ri => ri.RatingId == ratingId);
                foreach (var image in images)
                {
                    _unitOfWork.RatingImage.Delete(image);
                }

                // Delete rating
                _unitOfWork.Rating.Delete(rating);
                await _unitOfWork.SaveAsync();

                return new ApiResponseDto<string>
                {
                    Success = true,
                    Message = "Rating deleted successfully",
                    Data = "OK"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<string>
                {
                    Success = false,
                    Message = $"Failed to delete rating: {ex.Message}",
                    MessageId = "RATING_DELETE_FAILED"
                };
            }
        }
    }
}
