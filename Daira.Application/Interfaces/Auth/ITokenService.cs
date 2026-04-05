using Daira.Domain.Entities.AuthModel;
using System.Security.Claims;

namespace Daira.Application.Interfaces.Auth
{
    public interface ITokenService
    {
        Task<string> GenerateAccessTokenAsync(AppUser user, IList<string> roles);
        string GenerateRefreshToken();
        DateTime GetAccessTokenExpiration();
        DateTime GetRefreshTokenExpiration();
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }
}
