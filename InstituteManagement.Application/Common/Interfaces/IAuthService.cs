using InstituteManagement.Shared.DTOs.Auth;

namespace InstituteManagement.Application.Common.Interfaces
{
    public interface IAuthService
    {
        Task<bool> SignInAsync(SignInDto model);
        Task SignOutAsync();
        Task<UserDto?> GetCurrentUserAsync();
    }
}
