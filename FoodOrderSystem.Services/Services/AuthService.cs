using AutoMapper;
using FoodOrderSystem.DataAccess.IRepositories;
using FoodOrderSystem.Models.Domains;
using FoodOrderSystem.Models.DTOs;
using FoodOrderSystem.Models.DTOs.Authentication;
using FoodOrderSystem.Services.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FoodOrderSystem.Services.Services
{
    public class AuthService : IAuthService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;   

        public AuthService
        (
            RoleManager<IdentityRole> roleManager,
            IUnitOfWork unitOfWork,
            UserManager<ApplicationUser> userManager,
            IMapper mapper,
            ITokenService tokenService,
            IEmailService emailService
        )
        {
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
            _tokenService = tokenService;
            _emailService = emailService;
        }

        public Task<ResponseDto> SignIn(SignInDto signInDto)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDto> SignUpCust(SignUpStudentDto signUpStudentDto)
        {
            throw new NotImplementedException();
        }
    }
}
