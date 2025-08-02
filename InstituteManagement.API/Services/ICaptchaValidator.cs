namespace InstituteManagement.API.Services
{
    public interface ICaptchaValidator
    {
        Task<bool> IsCaptchaValid(string token, string action, double threshold = 0.5);
    }

}
