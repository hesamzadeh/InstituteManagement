namespace InstituteManagement.Shared.DTOs.Auth
{
    public class SignInDto
    {
        public string UsernameOrEmail { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool RememberMe { get; set; } = false;
        public string RecaptchaToken { get; set; } = "";
        /// <summary>
        /// UI language code used to pick localized messages (e.g. "en-US", "fa-IR")
        /// </summary>
        public string Language { get; set; } = "en-US";
    }
}
