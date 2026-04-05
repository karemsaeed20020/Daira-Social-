using Daira.Application.DTOs.AuthDto;
using Daira.Application.Response.Auth;

namespace Daira.Application.Interfaces.Auth
{
    public interface IAuthService
    {
        Task<RegisterResponse> RegisterAsync(RegisterDto registerDto);
        Task<LoginResponse> LoginAsync(LoginDto loginDto);
        Task<ConfirmEmailResponse> ConfirmEmailAsync(ConfirmEmailDto confirmEmailDto);
        Task<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenDto refreshTokenDto);
        Task<LogoutResponse> LogoutAsync(string userId);
        Task<ForgetPasswordResponse> ForgetPasswordAsync(ForgotPasswordDto forgotPasswordDto);
        Task<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
        Task<ResendConfirmationResponse> ResendEmailAsync(ResendEmailConfirmationDto resendEmailConfirmationDto);
        Task<ChangePasswordResponse> ChangePasswordAsync(string userId, ChangePasswordDto changePasswordDto);
        Task<UserProfileResponse> GetUserProfileAsync(string userId);
        Task<UpdateProfileResponse> UpdateProfileAsync(string userId, UpdateProfileDto updateProfileDto);
    }
}
