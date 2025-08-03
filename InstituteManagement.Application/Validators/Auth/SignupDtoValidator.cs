using InstituteManagement.Shared.DTOs.Signup;
using FluentValidation;

namespace InstituteManagement.Application.Validators.Auth
{
    public class SignupDtoValidator : AbstractValidator<SignupDto>
    {
        public SignupDtoValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(50).WithMessage("First name must be at most 50 characters.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(50).WithMessage("Last name must be at most 50 characters.");

            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("User name is required.")
                .MaximumLength(100).WithMessage("User name must be at most 100 characters.");

            RuleFor(x => x.NationalityCode)
                .IsInEnum().WithMessage("Nationality code is required.");

            RuleFor(x => x.NationalId)
                .NotEmpty().WithMessage("National ID is required.")
                .Matches(@"^\d{10}$").WithMessage("National ID must be exactly 10 digits.");

            RuleFor(x => x.Birthday)
                .LessThan(DateOnly.FromDateTime(DateTime.UtcNow)).WithMessage("Birthday must be in the past.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email format is invalid.");

            RuleFor(x => x.RecaptchaToken)
                .NotEmpty().WithMessage("reCAPTCHA token is required.");
        }
    }
}
