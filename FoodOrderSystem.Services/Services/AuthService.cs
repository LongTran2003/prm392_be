using AutoMapper;
using FoodOrderSystem.DataAccess.IRepositories;
using FoodOrderSystem.Models.Domains;
using FoodOrderSystem.Services.IServices;
using Microsoft.AspNetCore.Identity;

namespace FoodOrderSystem.Services.Services
{
    public class AuthService : IAuthService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;

        public AuthService
        (
            RoleManager<IdentityRole> roleManager,
            IUnitOfWork unitOfWork,
            UserManager<ApplicationUser> userManager,
            IMapper mapper,
            ITokenService tokenService
        )
        {
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
            _tokenService = tokenService;
        }
    }
}
