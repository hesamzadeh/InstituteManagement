using InstituteManagement.Application.Common.Interfaces;
using System.Security.Claims;

namespace InstituteManagement.API.Services
{
    namespace InstituteManagement.API.Services
    {
        public class CurrentUserService : ICurrentUserService
        {
            private string? _userId;

            public string UserId => _userId ?? "system";

            public void SetUser(string userId) => _userId = userId;
        }
    }

}
