using AutoMapper;
using FoodOrderSystem.Models.Domains;
using FoodOrderSystem.Models.DTOs.Authentication;
using FoodOrderSystem.Models.DTOs.Category;
using FoodOrderSystem.Models.DTOs.Profile;
using FoodOrderSystem.Models.DTOs.Shop;
using FoodOrderSystem.Utilities.Constants;

namespace FoodOrderSystem.Services.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // SignUpCustDto to User
            CreateMap<SignUpStudentDto, ApplicationUser>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.BirthDate))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => string.Empty))
                .ForMember(dest => dest.LockoutEnabled, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.EmailConfirmed, opt => opt.MapFrom(src => false));

            // ApplicationUser to GetUserResponseDto
            CreateMap<ApplicationUser, GetUserResponseDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.ImageUrl))
                .ForMember(dest => dest.RoleName, opt => opt.Ignore()) // Set in service
                .ForMember(dest => dest.WalletBalance, opt => opt.Ignore()) // Set in service
                .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.BirthDate.ToString("yyyy-MM-dd")));

            // RegisterShopOwnerDto to ApplicationUser
            CreateMap<RegisterShopOwnerDto, ApplicationUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Phone))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Avatar))
                .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.BirthDate ?? StaticOperationStatus.Timezone.Vietnam));

            // ApplicationUser to CustomerResponseDto
            CreateMap<ApplicationUser, CustomerResponseDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.ImageUrl))
                .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.BirthDate.ToString("yyyy-MM-dd")));

            // Category mappings
            CreateMap<Category, CategoryResponseDto>();
            CreateMap<CreateCategoryDto, Category>()
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => StaticOperationStatus.Timezone.Vietnam));

            // Shop mappings
            CreateMap<Shop, PopularShopResponseDto>()
                .ForMember(dest => dest.OpenHours, opt => opt.MapFrom(src => src.OpenHours.ToString(@"hh\:mm\:ss")))
                .ForMember(dest => dest.CloseHours, opt => opt.MapFrom(src => src.CloseHours.ToString(@"hh\:mm\:ss")));

            CreateMap<Shop, GetShopResponseDto>()
                .ForMember(dest => dest.OpenHours, opt => opt.MapFrom(src => src.OpenHours.ToString(@"hh\:mm\:ss")))
                .ForMember(dest => dest.CloseHours, opt => opt.MapFrom(src => src.CloseHours.ToString(@"hh\:mm\:ss")));

            CreateMap<MenuItem, MenuItemResponseDto>();
        }
    }
}
