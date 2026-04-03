namespace Daira.Application.Response.Auth
{
    public class ConfirmEmailResponse
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; }
        public string Email { get; set; }
        public List<string> Error { get; set; }

        public static ConfirmEmailResponse Success()
        {
            return new ConfirmEmailResponse
            {
                Succeeded = true,
                Message = "Email confirmed successfully, You can now login."
            };
        }

        public static ConfirmEmailResponse Success(string email)
        {
            return new ConfirmEmailResponse
            {
                Succeeded = true,
                Message = "Email confirmed successfully, You can now login.",
                Email = email
            };
        }

        public static ConfirmEmailResponse AlreadyConfirmed()
        {
            return new ConfirmEmailResponse
            {
                Succeeded = true,
                Message = "Email is already confirmed."
            };
        }
        public static ConfirmEmailResponse Failed(string message)
        {
            return new ConfirmEmailResponse
            {
                Succeeded = false,
                Message = message,
                Error = new List<string> { message }
            };
        }
    }
}