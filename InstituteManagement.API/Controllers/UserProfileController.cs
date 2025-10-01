using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using InstituteManagement.API.Services;
using InstituteManagement.Infrastructure.Persistence;
using InstituteManagement.Shared.DTOs.UserProfile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.Security.Claims;

namespace InstituteManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserProfileController : AppControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly AppDbContext _appDbContext;
        private readonly ICaptchaValidator _captchaValidator;
        private readonly IValidator<UpdateAccountDto> _validator;

        public UserProfileController(
            UserManager<AppUser> userManager,
            IMapper mapper,
            AppDbContext appDbContext,
            ICaptchaValidator captchaValidator,
            IValidator<UpdateAccountDto> validator,
            IWebHostEnvironment env
            )
        {
            _userManager = userManager;
            _mapper = mapper;
            _appDbContext = appDbContext;
            _captchaValidator = captchaValidator;
            _validator = validator;
            _env = env;
        }
       
        [HttpGet("me")]
        public async Task<IActionResult> GetProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var person = await _appDbContext.People
                .FirstOrDefaultAsync(p => p.AppUserId == user.Id);
            if (person == null) return NotFound();

            var dto = _mapper.Map<PersonDto>(person);
            return Ok(dto);
        }

        [HttpPut("me")]
        public async Task<ActionResult> UpdateMe([FromBody] UpdatePersonDto dto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized(); ;

            var person = await _appDbContext.People.FirstOrDefaultAsync(p => p.AppUserId == user.Id);
            if (person == null) return NotFound();

            _mapper.Map(dto, person); // update fields

            _appDbContext.People.Update(person);
            await _appDbContext.SaveChangesAsync();

            return Ok(_mapper.Map<PersonDto>(person));
        }


        [HttpPut("account")]
        public async Task<ActionResult> UpdateAccount([FromBody] UpdateAccountDto dto)
        {
            // 1. Validate DTO
            ValidationResult validationResult = await _validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            // 2. CAPTCHA verification
            if (dto.RecaptchaToken != "dev-mode")
            {
                var captchaOk = await _captchaValidator.IsCaptchaValid(dto.RecaptchaToken, "submit");
                if (!captchaOk)
                {
                    return BadRequest(new { Error = "CaptchaFailed", Message = "CAPTCHA validation failed." });
                }
            }

            // 3. Get current user
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();


            if (string.IsNullOrWhiteSpace(dto.CurrentPassword))
                return BadRequest("Current password is required for any change.");

            // 4. Verify current password first
            if (!await _userManager.CheckPasswordAsync(user, dto.CurrentPassword))
                return BadRequest("Current password is incorrect.");

            // 5. Update Email
            if (!string.IsNullOrWhiteSpace(dto.Email) && dto.Email != user.Email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, dto.Email);
                if (!setEmailResult.Succeeded) return BadRequest(setEmailResult.Errors);

                // TODO: send email verification code
            }

            // 6. Update Phone
            if (!string.IsNullOrWhiteSpace(dto.PhoneNumber) && dto.PhoneNumber != user.PhoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, dto.PhoneNumber);
                if (!setPhoneResult.Succeeded) return BadRequest(setPhoneResult.Errors);

                // TODO: send SMS verification code
            }

            // 7. Change password
            if (!string.IsNullOrWhiteSpace(dto.NewPassword))
            {
                if (string.IsNullOrWhiteSpace(dto.CurrentPassword))
                    return BadRequest(new { Error = "CurrentPasswordRequired", Message = "Current password is required." });

                var changePassResult = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
                if (!changePassResult.Succeeded) return BadRequest(changePassResult.Errors);
            }

            return Ok("Account updated. Verification required for new email/phone.");
        }

        [HttpGet("account")]
        public async Task<ActionResult<UpdateAccountDto>> GetAccount()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var dto = new UpdateAccountDto
            {
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                UserName = user.UserName
            };

            return Ok(dto);
        }

        [HttpGet("profile-pic/{personId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProfilePic(Guid personId)
        {
            var person = await _appDbContext.People.FindAsync(personId);
            if (person == null)
                return NotFound();

            var fileName = $"{person.AppUserId}.jpg";
            var profilePath = Path.Combine(
                _env.WebRootPath, "images", "profiles", "profile-pics", fileName
            );

            if (!System.IO.File.Exists(profilePath))
            {
                var defaultPath = Path.Combine(
                    _env.WebRootPath, "images", "profiles", "profile-pics", "default-icon.jpg"
                );
                return PhysicalFile(defaultPath, "image/jpeg");
            }

            return PhysicalFile(profilePath, "image/jpeg");
        }



        [HttpPost("upload-profile-pic")]
        public async Task<IActionResult> UploadProfilePic(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            // Validate size (max 2MB)
            if (file.Length > 2 * 1024 * 1024)
                return BadRequest("File too large. Max 2MB allowed.");

            //var userId = User.FindFirst("sub")?.Value ?? User.Identity?.Name;
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var uploadsPath = Path.Combine(_env.WebRootPath, "images", "profiles", "profile-pics");
            if (!Directory.Exists(uploadsPath))
                Directory.CreateDirectory(uploadsPath);

            var fileName = $"{userId}.jpg";
            var filePath = Path.Combine(uploadsPath, fileName);

            // Process image with ImageSharp
            using var image = await Image.LoadAsync(file.OpenReadStream());
            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Crop,
                Size = new Size(512, 512)
            }));

            await image.SaveAsJpegAsync(filePath);

            // Build relative URL (for <img src>)
            var relativeUrl = $"/images/profiles/profile-pics/{fileName}";

            // Update DB
            var person = await _appDbContext.People.FirstOrDefaultAsync(p => p.AppUserId == Guid.Parse(userId));
            if (person == null) return NotFound();

            person.ProfilePictureUrl = relativeUrl;
            await _appDbContext.SaveChangesAsync();

            // Return URL to frontend
            return Ok(relativeUrl);
        }
    }
}