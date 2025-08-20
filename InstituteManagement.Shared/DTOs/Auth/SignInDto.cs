
namespace InstituteManagement.Shared.DTOs.Auth
{
    public class SignInDto
    {
        public string UsernameOrEmail { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool RememberMe { get; set; }
    }
}
