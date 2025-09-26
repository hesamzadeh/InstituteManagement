using FluentValidation;
using InstituteManagement.Shared;
using InstituteManagement.Shared.DTOs.Auth;
using static InstituteManagement.Shared.MessageKeys.Auth.Keys;

namespace InstituteManagement.Application.Validators.Auth
{
    public class SignInDtoValidator : AbstractValidator<SignInDto>
    {
        public SignInDtoValidator()
        {
            RuleFor(x => x.UsernameOrEmail)
                .NotEmpty().WithMessage(x => UsernameOrEmailRequired.Get(x.Language))
                .MaximumLength(100).WithMessage(x => UsernameOrEmailMaxLength.Get(x.Language));

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage(x => PasswordRequired.Get(x.Language))
                .MinimumLength(6).WithMessage(x => PasswordMinLength.Get(x.Language));

            RuleFor(x => x.RecaptchaToken)
                .NotEmpty().WithMessage(x => RecaptchaTokenRequired.Get(x.Language));
        }
    }
}
