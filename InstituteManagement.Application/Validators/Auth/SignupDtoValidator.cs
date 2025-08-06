using FluentValidation;
using InstituteManagement.Application.Common.Interfaces;
using InstituteManagement.Shared;
using InstituteManagement.Shared.DTOs.Signup;
using System.Globalization;
using static InstituteManagement.Shared.MessageKeys.Signup.Keys;

namespace InstituteManagement.Application.Validators.Auth
{
    public class SignupDtoValidator : AbstractValidator<SignupDto>
    {
        public SignupDtoValidator()
        {

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage(x => FirstNameRequired.Get(x.Language))
                .MaximumLength(50).WithMessage(x => FirstNameMaxLength.Get(x.Language));

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage(x => LastNameRequired.Get(x.Language))
                .MaximumLength(50).WithMessage(x => LastNameMaxLength.Get(x.Language));

            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage(x => UsernameRequired.Get(x.Language))
                .MinimumLength(5).WithMessage(x => UsernameMinLength.Get(x.Language))
                .MaximumLength(20).WithMessage(x => UsernameMaxLength.Get(x.Language));

            RuleFor(x => x.NationalityCode)
                .IsInEnum().WithMessage(x => NationalityCodeRequired.Get(x.Language));

            RuleFor(x => x.NationalId)
                .NotEmpty().WithMessage(x => NationalIdRequired.Get(x.Language))
                .Matches(@"^\d{10}$").WithMessage(x => NationalIdMustBe10Digits.Get(x.Language));

            RuleFor(x => x.Birthday)
                .LessThan(DateOnly.FromDateTime(DateTime.UtcNow)).WithMessage(x => BirthdayMustBeInPast.Get(x.Language));

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(x => EmailRequired.Get(x.Language))
                .EmailAddress().WithMessage(x => EmailFormatInvalid.Get(x.Language));

            RuleFor(x => x.RecaptchaToken)
                .NotEmpty().WithMessage(x => RecaptchaTokenRequired.Get(x.Language));
        }

    }
}
