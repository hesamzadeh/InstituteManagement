using AutoMapper;
using InstituteManagement.API.Services;
using InstituteManagement.Application.Common;
using InstituteManagement.Core.Entities;
using InstituteManagement.Infrastructure;
using InstituteManagement.Shared;
using InstituteManagement.Shared.DTOs.Signup;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace InstituteManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SignupController : ControllerBase
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

        [HttpPost]
        public async Task<IActionResult> Signup(SignupDto dto)
        {
            if (dto.RecaptchaToken == "dev-mode")
            {
                // Accept it without verification
            }
            else
            {
                if (!await _captchaValidator.IsCaptchaValid(dto.RecaptchaToken, "submit"))
                    return BadRequest(new { Message = _localizer["CaptchaFailed"] });
            }

            if (dto.NationalityCode == NationalityCode.IR &&
                !NationalIdValidator.IsValidIranianCodeMeli(dto.NationalId))
            {
                return BadRequest(new { Message = _localizer["Signup.InvalidNationalId"] });
            }

            // Check for duplicate NationalId
            try
            {
                var existingPerson = await _appDbContext.People
               .AsNoTracking()
               .FirstOrDefaultAsync(p => p.NationalityCode == dto.NationalityCode && p.NationalId == dto.NationalId);

                if (existingPerson != null)
                {
                    return Conflict(new
                    {
                        Field = nameof(dto.NationalId),
                        Message = _localizer["Signup.NationalIdAlreadyExists"],
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = _localizer["UnexpectedError"],
                    Error = ex.Message
                });
            }



            // Check for duplicate UserName
            var existingUser = await _appDbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserName == dto.UserName);

            if (existingUser != null)
            {
                return Conflict(new
                {
                    Field = nameof(dto.UserName),
                    Message = _localizer["Signup.UserNameAlreadyExists"]
                });
            }

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
                return BadRequest(new
                {
                    Message = _localizer["Signup.PersonSaveError"],
                    Error = dbEx.InnerException?.Message ?? dbEx.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = _localizer["UnexpectedError"],
                    Error = ex.Message
                });
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
                return BadRequest(new
                {
                    Message = _localizer["Signup.UserCreationFailed"],
                    Errors = createResult.Errors
                });
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
