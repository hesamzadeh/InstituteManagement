
namespace InstituteManagement.Application.Common.Interfaces
{
    public interface ICurrentUserService
    {
        string UserId { get; }
        void SetUser(string userId);
    }
}
