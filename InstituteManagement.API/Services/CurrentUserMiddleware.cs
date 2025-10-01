using InstituteManagement.Application.Common.Interfaces;
using System.Security.Claims;

namespace InstituteManagement.API.Services
{
    public class CurrentUserMiddleware
    {
        private readonly RequestDelegate _next;

        public CurrentUserMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context, ICurrentUserService currentUserService)
        {
            if (context.User.Identity?.IsAuthenticated ?? false)
            {
                var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
                currentUserService.SetUser(userId!);
            }

            await _next(context);
        }
    }

}
