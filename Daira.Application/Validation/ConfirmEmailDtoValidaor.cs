using Daira.Application.DTOs.AuthDto;
using FluentValidation;

namespace Daira.Application.Validation
{
    public class ConfirmEmailDtoValidaor : AbstractValidator<ConfirmEmailDto>
    {
        public ConfirmEmailDtoValidaor()
        {
            RuleFor(c => c.Email)
               .NotEmpty().WithMessage("Email is required.")
               .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(c => c.Token)
                .NotEmpty().WithMessage("Token is required.");
        }
    }
}
