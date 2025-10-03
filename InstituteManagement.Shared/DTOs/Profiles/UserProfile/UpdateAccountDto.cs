namespace InstituteManagement.Shared.DTOs.Profiles.UserProfile
{
    public class UpdateAccountDto
    {
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? CurrentPassword { get; set; }
        public string? NewPassword { get; set; }
        public string RecaptchaToken { get; set; } = string.Empty;
        public string? Language { get; set; }
        public string? UserName { get; set; }
    }
}
