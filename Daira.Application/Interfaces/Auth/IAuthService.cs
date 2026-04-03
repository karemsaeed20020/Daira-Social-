using Daira.Application.DTOs.AuthDto;
using Daira.Application.Response.Auth;

namespace Daira.Application.Interfaces.Auth
{
    public interface IAuthService
    {
        Task<RegisterResponse> RegisterAsync(RegisterDto registerDto);
        Task<LoginResponse> LoginAsync(LoginDto loginDto);
        Task<ConfirmEmailResponse> ConfirmEmailAsync(ConfirmEmailDto confirmEmailDto);
    }
}
