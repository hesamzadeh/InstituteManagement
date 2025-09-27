using InstituteManagement.Shared.Enums;
using System.Globalization;

namespace InstituteManagement.Shared.DTOs.Signup
{
    public class SignupDto
    {
        public string? FirstName { get; set; } 
        public string? LastName { get; set; } 
        public string? UserName { get; set; }
        public string? FullName { get; set; }
        public NationalityCode NationalityCode { get; set; }
        public string? NationalId { get; set; } 
        public DateOnly Birthday { get; set; } 
        public string? Email { get; set; }
        public string? RecaptchaToken { get; set; }
        public string? Language { get; set; }
        public ProfileType InitialRole { get; set; }
        public string? DisplayName { get; set; }
    }
}