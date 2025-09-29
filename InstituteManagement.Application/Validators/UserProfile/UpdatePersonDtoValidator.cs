using FluentValidation;
using InstituteManagement.Shared.DTOs.UserProfile;

namespace InstituteManagement.Application.Validators.UserProfile
{
    public class UpdatePersonDtoValidator : AbstractValidator<UpdatePersonDto>
    {
        public UpdatePersonDtoValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Birthday).LessThan(DateOnly.FromDateTime(DateTime.Today));
            RuleFor(x => x.PostalCode).MaximumLength(20);
            RuleFor(x => x.FullAddress).MaximumLength(500);
        }
    }
}
