namespace Daira.Application.Response.Auth
{
    public class RefreshTokenResponse
    {
        public bool Succeeded { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? AccessTokenExpireation { get; set; }
        public DateTime? RefreshTokenExpiration { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; }

        public static RefreshTokenResponse Success(string accessToken, string refreshToken, DateTime accessTokenExpireation, DateTime refreshTokenExpiration)
        {
            return new RefreshTokenResponse
            {
                Succeeded = true,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpireation = accessTokenExpireation,
                RefreshTokenExpiration = refreshTokenExpiration,
                Message = "Tokens refreshed successfully",
            };
        }
        public static RefreshTokenResponse Failure(string message)
        {
            return new RefreshTokenResponse
            {
                Succeeded = false,
                Message = message,
                Errors = new List<string> { message }
            };
        }
    }
}
