using FluentValidation;
using FluentValidation.Results;
using InstituteManagement.API.Services;
using InstituteManagement.Infrastructure.Persistence;
using InstituteManagement.Shared;
using InstituteManagement.Shared.DTOs.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Security.Claims;

namespace InstituteManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : AppControllerBase
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly ICaptchaValidator _captchaValidator;
        private readonly IValidator<SignInDto> _validator;
        private readonly IUserClaimsPrincipalFactory<AppUser> _claimsFactory;

        public AuthController(
            SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager,
            ICaptchaValidator captchaValidator,
            IValidator<SignInDto> validator,
            IUserClaimsPrincipalFactory<AppUser> claimsFactory)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _captchaValidator = captchaValidator;
            _validator = validator;
            _claimsFactory = claimsFactory;
        }

        /// <summary>
        /// Sign-in endpoint - uses FluentValidation + reCAPTCHA verification.
        /// </summary>
        [HttpPost("signin")]
        public async Task<IActionResult> SignIn([FromBody] SignInDto model)
        {
            // 1 Run FluentValidation
            ValidationResult validationResult = await _validator.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                var modelState = new ModelStateDictionary();
                foreach (var error in validationResult.Errors)
                {
                    modelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return ValidationProblem(modelState);
            }

            // 2️ CAPTCHA verification (allow dev bypass)
            if (model.RecaptchaToken != "dev-mode")
            {
                var captchaOk = await _captchaValidator.IsCaptchaValid(model.RecaptchaToken, "submit");
                if (!captchaOk)
                    return ValidationError(nameof(model.RecaptchaToken),
                                           MessageKeys.Auth.Keys.RecaptchaTokenRequired.Get(model.Language));
            }

            // 3️ Locate user by username or email
            var user = await _userManager.FindByNameAsync(model.UsernameOrEmail)
                       ?? await _userManager.FindByEmailAsync(model.UsernameOrEmail);

            if (user == null)
                return Unauthorized(new { Message = MessageKeys.Auth.Keys.InvalidCredentials.Get(model.Language) });

            // 4️ Verify password
            var passwordOk = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!passwordOk)
                return Unauthorized(new { Message = MessageKeys.Auth.Keys.InvalidCredentials.Get(model.Language) });

            // 5️ Generate ClaimsPrincipal including custom claims
            var principal = await _claimsFactory.CreateAsync(user);

            // 6️ Sign in with the principal
            await _signInManager.SignInAsync(user, isPersistent: model.RememberMe);

            // 7️ Extract claims to send lightweight user info back to client
            var profilePictureUrl = principal.Claims.FirstOrDefault(c => c.Type == "ProfilePictureUrl")?.Value
                                    ?? "/images/profiles/profile-pics/default-icon.jpg";

            var firstName = principal.Claims.FirstOrDefault(c => c.Type == "FirstName")?.Value ?? "";
            var lastName = principal.Claims.FirstOrDefault(c => c.Type == "LastName")?.Value ?? "";
            var lastUsedProfileId = principal.Claims.FirstOrDefault(c => c.Type == "LastUsedProfileId")?.Value ?? "";

            var fullName =  firstName + " " + lastName
                           ?? principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            // 8️ Return response to Blazor
            return Ok(new
            {
                Message = MessageKeys.Auth.Keys.SignInSuccess.Get(model.Language),
                user.Id,
                Username = user.UserName,
                user.Email,
                FullName = fullName,
                ProfilePictureUrl = profilePictureUrl,
                FirstName = firstName,
                LastName = lastName,
                LastUsedProfileId = lastUsedProfileId
            });
        }


        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { ok = true });
        }

        // Called by Blazor to check current browser's auth cookie
        [HttpGet("whoami")]
        public IActionResult WhoAmI()
        {
            if (User?.Identity?.IsAuthenticated ?? false)
            {
                var claims = User.Claims.ToList();

                // Extract the claims we want

                string firstName = claims.FirstOrDefault(c => c.Type == "FirstName")?.Value ?? string.Empty;
                string lastName = claims.FirstOrDefault(c => c.Type == "LastName")?.Value ?? string.Empty;
                string profilePictureUrl = claims.FirstOrDefault(c => c.Type == "ProfilePictureUrl")?.Value
                                           ?? "/images/profiles/profile-pics/default-icon.jpg";
                string lastUsedProfileId = claims.FirstOrDefault(c => c.Type == "LastUsedProfileId")?.Value ?? string.Empty;
                var fullName = firstName + " " + lastName
                            ?? string.Empty;
                return Ok(new
                {
                    IsAuthenticated = true,
                    Username = User.Identity!.Name,
                    FullName = fullName,
                    FirstName = firstName,
                    LastName = lastName,
                    ProfilePictureUrl = profilePictureUrl,
                    LastUsedProfileId = lastUsedProfileId
                });
            }

            return Ok(new
            {
                IsAuthenticated = false,
                Username = string.Empty,
                FullName = string.Empty,
                FirstName = string.Empty,
                LastName = string.Empty,
                ProfilePictureUrl = "/images/profiles/profile-pics/default-icon.jpg",
                LastUsedProfileId = string.Empty
            });
        }

    }
}
