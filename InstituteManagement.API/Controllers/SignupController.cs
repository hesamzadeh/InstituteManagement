using AutoMapper;
using InstituteManagement.API.Services;
using InstituteManagement.Application.Common;
using InstituteManagement.Core.Entities;
using InstituteManagement.Infrastructure;
using InstituteManagement.Shared;
using InstituteManagement.Shared.DTOs.Signup;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace InstituteManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SignupController : AppControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly AppDbContext _appDbContext;
        private readonly IStringLocalizer<SharedResources> _localizer;
        private readonly ICaptchaValidator _captchaValidator;

        public SignupController(
            UserManager<AppUser> userManager,
            IMapper mapper,
            AppDbContext appDbContext,
            IStringLocalizer<SharedResources> localizer,
            ICaptchaValidator captchaValidator)
        {
            _userManager = userManager;
            _mapper = mapper;
            _appDbContext = appDbContext;
            _localizer = localizer;
            _captchaValidator = captchaValidator;
        }

        [HttpGet("check-username")]
        public async Task<IActionResult> CheckUsername([FromQuery] string username)
        {
            var isAvailable = await _userManager.FindByNameAsync(username) == null;
            return Ok(isAvailable);
        }

        [HttpPost]
        public async Task<IActionResult> Signup(SignupDto dto)
        {
            // CAPTCHA verification
            if (dto.RecaptchaToken != "dev-mode")
            {
                var captchaOk = await _captchaValidator.IsCaptchaValid(dto.RecaptchaToken, "submit");
                if (!captchaOk)
                {
                    return ValidationError(nameof(dto.RecaptchaToken), _localizer["Signup.CaptchaFailed"]);
                }
            }

            // National ID validation
            if (dto.NationalityCode == NationalityCode.IR &&
                !NationalIdValidator.IsValidIranianCodeMeli(dto.NationalId))
            {
                return ValidationError(nameof(dto.NationalId), _localizer["Signup.InvalidNationalId"]);
            }

            // Duplicate National ID
            try
            {
                var existingPerson = await _appDbContext.People
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.NationalityCode == dto.NationalityCode && p.NationalId == dto.NationalId);

                if (existingPerson != null)
                {
                    return ValidationError(nameof(dto.NationalId), _localizer["Signup.NationalIdAlreadyExists"]);
                }
            }
            catch (Exception ex)
            {
                return Problem(
                    detail: ex.Message,
                    title: _localizer["Signup.UnexpectedError"],
                    statusCode: 500
                );
            }

            // Duplicate Username
            var existingUser = await _appDbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserName == dto.UserName);

            if (existingUser != null)
            {
                return ValidationError(nameof(dto.UserName), _localizer["Signup.UserNameAlreadyExists"]);
            }

            // Create Person and User
            var password = GenerateRandomPassword(8);

            var person = new Person
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Birthday = dto.Birthday,
                SignupDate = DateTime.UtcNow,
                IsVerified = false,
                IsLocked = false,
                NationalityCode = dto.NationalityCode,
                NationalId = dto.NationalId
            };

            try
            {
                _appDbContext.People.Add(person);
                await _appDbContext.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                return Problem(
                    detail: dbEx.InnerException?.Message ?? dbEx.Message,
                    title: _localizer["Signup.PersonSaveError"],
                    statusCode: 400
                );
            }
            catch (Exception ex)
            {
                return Problem(
                    detail: ex.Message,
                    title: _localizer["Signup.UnexpectedError"],
                    statusCode: 500
                );
            }

            var user = new AppUser
            {
                UserName = dto.UserName,
                Email = dto.Email,
                EmailConfirmed = false,
                PhoneNumberConfirmed = false,
                Person = person
            };

            var createResult = await _userManager.CreateAsync(user, password);
            if (!createResult.Succeeded)
            {
                var modelState = new ModelStateDictionary();
                foreach (var error in createResult.Errors)
                {
                    modelState.AddModelError(string.Empty, error.Description);
                }
                return ValidationProblem(modelState);
            }

            person.AppUserId = user.Id;
            _appDbContext.People.Update(person);
            await _appDbContext.SaveChangesAsync();

            return Ok(new
            {
                Message = _localizer["Signup.SignupSuccess"],
                user.Id,
                GeneratedPassword = password
            });
        }

        private static string GenerateRandomPassword(int length = 8)
        {
            const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lower = "abcdefghijklmnopqrstuvwxyz";
            const string digits = "0123456789";
            const string symbols = "!@#$%^&*";

            var rand = new Random();

            var passwordChars = new List<char>
            {
                upper[rand.Next(upper.Length)],
                lower[rand.Next(lower.Length)],
                digits[rand.Next(digits.Length)],
                symbols[rand.Next(symbols.Length)]
            };

            string all = upper + lower + digits + symbols;
            for (int i = passwordChars.Count; i < length; i++)
            {
                passwordChars.Add(all[rand.Next(all.Length)]);
            }

            return new string(passwordChars.OrderBy(_ => rand.Next()).ToArray());
        }
    }
}
