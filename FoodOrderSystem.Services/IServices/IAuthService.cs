using FoodOrderSystem.Models.DTOs;
using FoodOrderSystem.Models.DTOs.Authentication;

namespace FoodOrderSystem.Services.IServices
{
    public interface IAuthService
    {
        Task<ResponseDto> SignUpCust(SignUpStudentDto signUpStudentDto);
        Task<ResponseDto> SignIn(SignInDto signInDto);
        //Task<ResponseDto> SendVerifyEmail(EmailDto emailDto);
        //Task<ResponseDto> VerifyEmail(VerifyEmailDto verifyEmailDto);
        //Task<ResponseDto> VerifyOtp(VerifyOtpDto verifyOtpDto);
        //Task<ResponseDto> VerifyResetOtp(VerifyOtpDto verifyOtpDto);
        //Task<ResponseDto> ForgotPassword(EmailDto forgotPasswordDto);
        //Task<ResponseDto> ResetPassword(ResetPasswordDto resetPasswordDto);
        //Task<ResponseDto> ChangePassword(ChangePasswordDto changePasswordDto, ClaimsPrincipal User);
        //Task<ResponseDto> SendOTP(EmailDto sendOTPDto);
        //Task<ResponseDto> ResendOTP(string email);
    }
}
