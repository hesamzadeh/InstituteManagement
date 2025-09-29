using FluentValidation;
using InstituteManagement.Shared.DTOs.UserProfile;

namespace InstituteManagement.Application.Validators.UserProfile
{
    using FluentValidation;
    using global::InstituteManagement.Shared;
    using static global::InstituteManagement.Shared.MessageKeys;

    namespace InstituteManagement.Application.Validators.UserProfile
    {
        public class UpdateAccountDtoValidator : AbstractValidator<UpdateAccountDto>
        {
            public UpdateAccountDtoValidator()
            {
                RuleFor(x => x.RecaptchaToken)
                    .NotEmpty().WithMessage(x => UpdateAccount.Keys.RecaptchaTokenRequired.Get(x.Language));

                RuleFor(x => x.Email)
                    .EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email))
                    .WithMessage(x => UpdateAccount.Keys.EmailInvalid.Get(x.Language));

                RuleFor(x => x.PhoneNumber)
                    .Matches(@"^\+?[1-9]\d{7,14}$").When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber))
                    .WithMessage(x => UpdateAccount.Keys.PhoneInvalid.Get(x.Language));

                RuleFor(x => x.NewPassword)
                    .MinimumLength(8).When(x => !string.IsNullOrWhiteSpace(x.NewPassword))
                    .WithMessage(x => UpdateAccount.Keys.PasswordMinLength.Get(x.Language))
                    .Matches("[A-Z]").WithMessage(x => UpdateAccount.Keys.PasswordUppercaseRequired.Get(x.Language))
                    .Matches("[a-z]").WithMessage(x => UpdateAccount.Keys.PasswordLowercaseRequired.Get(x.Language))
                    .Matches(@"\d").WithMessage(x => UpdateAccount.Keys.PasswordDigitRequired.Get(x.Language))
                    .Matches(@"[\W_]").WithMessage(x => UpdateAccount.Keys.PasswordSpecialRequired.Get(x.Language));
            }
        }
    }

}
