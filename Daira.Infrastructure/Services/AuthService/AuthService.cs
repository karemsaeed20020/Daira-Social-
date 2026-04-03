using AutoMapper;
using Daira.Application.DTOs.AuthDto;
using Daira.Application.Interfaces;
using Daira.Application.Interfaces.Auth;
using Daira.Application.Response.Auth;
using Daira.Infrastructure.Settings;
using Daira.Infrastructure.Specefication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Encodings.Web;

namespace Daira.Infrastructure.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AuthService> _logger;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly EmailSettings _emailSettings;
        public AuthService(UserManager<AppUser> userManager, IUnitOfWork unitOfWork, SignInManager<AppUser> signInManager, ITokenService tokenService, ILogger<AuthService> logger, IMapper mapper, IEmailService emailService, IOptions<EmailSettings> emailSettings)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _logger = logger;
            _mapper = mapper;
            _emailService = emailService;
            _emailSettings = emailSettings.Value;
        }
        public async Task<LoginResponse> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user is null)
            {
                return LoginResponse.Failure("Invalid email or password");
            }
            if (!user.IsActive)
            {
                return LoginResponse.Failure("User account is deactivated.");
            }
            if (!user.EmailConfirmed)
            {
                return LoginResponse.Failure("Email is not confirmed. Please check your email to confirm your account.");
            }
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, lockoutOnFailure: true);
            if (result.IsLockedOut)
            {
                return LoginResponse.LockedOut();
            }
            if (result.RequiresTwoFactor)
            {
                return LoginResponse.TwoFactorRequired(user.Email!);
            }
            if (!result.Succeeded)
            {
                return LoginResponse.Failure("Invalid email or password.");
            }
            return await GenerateAuthTokensAsync(user);

        }
        // Register User
        public async Task<RegisterResponse> RegisterAsync(RegisterDto registerDto)
        {
            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser is not null)
            {
                return RegisterResponse.Failure(new[] { "Email is already Exists" });
            }
            var user = _mapper.Map<AppUser>(registerDto);
            var createUser = await _userManager.CreateAsync(user, registerDto.Password);
            if (!createUser.Succeeded)
            {
                var errors = createUser.Errors.Select(e => e.Description);
                return RegisterResponse.Failure(errors);
            }
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var confirmationLink = $"{_emailSettings.BaseUrl}/api/auth/confirm-email?email={UrlEncoder.Default.Encode(user.Email!)}&token={encodedToken}";
            try
            {
                await _emailService.SendEmailConfirmationAsync(user.Email!, confirmationLink);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send confirmation email to {Email}", user.Email);
            }
            _logger.LogInformation("User {Email} registered successfully", user.Email);
            return RegisterResponse.Success(user.Id, user.Email!, requiresConfirmation: true);
        }

        public async Task<ConfirmEmailResponse> ConfirmEmailAsync(ConfirmEmailDto confirmEmailDto)
        {
            var user = await _userManager.FindByEmailAsync(confirmEmailDto.Email);
            if (user is null)
            {
                return ConfirmEmailResponse.Failed("User Not Found");
            }
            if (user.EmailConfirmed)
            {
                return ConfirmEmailResponse.AlreadyConfirmed();
            }
            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(confirmEmailDto.Token));
            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return ConfirmEmailResponse.Failed(string.Join(", ", errors));
            }
            _logger.LogInformation("User {Email} confirmed their email successfully", user.Email);
            return ConfirmEmailResponse.Success();
        }

        //Private Methods
        private async Task<LoginResponse> GenerateAuthTokensAsync(AppUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var accessToken = await _tokenService.GenerateAccessTokenAsync(user, roles);
            var refreshToken = _tokenService.GenerateRefreshToken();


            var refreshTokenrow = new RefreshToken
            {
                Token = refreshToken,
                ExpiresAt = _tokenService.GetRefreshTokenExpiration(),
                UserId = user.Id,
            };
            await _unitOfWork.Repository<RefreshToken>().AddAsync(refreshTokenrow);
            await _unitOfWork.CommitAsync();

            return LoginResponse.Success(
                user.Id,
                user.Email!,
                user.FullName,
                accessToken,
                _tokenService.GetAccessTokenExpiration(),
                _tokenService.GetRefreshTokenExpiration(),
                 refreshToken,
                roles);
        }

        public Task<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenDto refreshTokenDto)
        {
            throw new NotImplementedException();
        }
        // Logout User
        public async Task<LogoutResponse> LogoutAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                return LogoutResponse.Failed("User Not Found");
            }
            var spec = new RefreshTokenSpecification(rt => rt.UserId == user.Id && rt.IsRevoked == false && rt.ExpiresAt >= DateTime.UtcNow);
            var refreshTokens = await _unitOfWork.Repository<RefreshToken>().GetAllWithSpec(spec);
            foreach (var token in refreshTokens)
            {
                token.IsRevoked = true;
                token.RevokedAt = DateTime.UtcNow;
            }
            await _unitOfWork.CommitAsync();
            return LogoutResponse.Succedd();
        }
        // Forgot Password
        public async Task<ForgetPasswordResponse> ForgetPasswordAsync(ForgotPasswordDto forgotPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
            if (user is null || !user.EmailConfirmed)
            {

                return ForgetPasswordResponse.Success();
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var resetLink = $"{_emailSettings.BaseUrl}/api/auth/reset-password?email={UrlEncoder.Default.Encode(user.Email!)}&token={encodedToken}";
            try
            {
                await _emailService.SendPasswordResetAsync(user.Email!, resetLink);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send password reset email to {Email}", user.Email);
            }
            return ForgetPasswordResponse.Success();
        }

        public async Task<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user is null)
            {
                return ResetPasswordResponse.Failure("User Not Found.");
            }
            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(resetPasswordDto.Token));
            var result = await _userManager.ResetPasswordAsync(user, decodedToken, resetPasswordDto.NewPassword);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return ResetPasswordResponse.Failure(errors);
            }
            await LogoutAsync(user.Id);
            return ResetPasswordResponse.Success();
        }

        //Resend Email Confirmation
        public async Task<ResendConfirmationResponse> ResendEmailAsync(ResendEmailConfirmationDto resendEmailConfirmationDto)
        {
            var user = await _userManager.FindByEmailAsync(resendEmailConfirmationDto.Email);
            if (user is null) return ResendConfirmationResponse.Failure("User not Found");
            if (!user.EmailConfirmed) return ResendConfirmationResponse.AlreadyConfirmed();
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var confirmationLink = $"{_emailSettings.BaseUrl}/api/auth/confirm-email?email={UrlEncoder.Default.Encode(user.Email!)}&token={encodedToken}";

            try
            {
                await _emailService.SendEmailConfirmationAsync(user.Email!, confirmationLink);
            }
            catch
            {
                _logger.LogError("Failed to send confirmation email to {Email}", user.Email);
            }
            return ResendConfirmationResponse.Success(user.Email!);
        }

    }
}
