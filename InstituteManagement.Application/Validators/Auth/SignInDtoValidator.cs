using FluentValidation;
using InstituteManagement.Shared.DTOs.Auth;
namespace InstituteManagement.Application.Validators.Auth
{
    public class SignInDtoValidator : AbstractValidator<SignInDto>
    {
        public SignInDtoValidator()
        {
            RuleFor(x => x.UsernameOrEmail).NotEmpty();
            RuleFor(x => x.Password).NotEmpty();
        }
    }
}
