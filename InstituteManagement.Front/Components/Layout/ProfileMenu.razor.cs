using InstituteManagement.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using System.Security.Claims;
using System.Text.Json;
namespace InstituteManagement.Front.Components.Layout
{
    public partial class ProfileMenu : ComponentBase
    {
        private bool showHover;
        private bool isSidebarOpen;
        private bool isDark;

        private ClaimsPrincipal? user;
        private bool IsAuthenticated => user?.Identity?.IsAuthenticated ?? false;

        private string UserName => GetClaim("preferred_username") ?? GetClaim(ClaimTypes.Upn) ?? "";
        private string FullName => GetClaim(ClaimTypes.Name) ?? GetClaim("FullName") ?? UserName;
        private string Email => GetClaim(ClaimTypes.Email) ?? "";
        private string ProfilePictureOrPlaceholder =>
            ResolveProfilePictureUrl(GetClaim("ProfilePictureUrl"));

        [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;
        [Inject] private NavigationManager Nav { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;
        [Inject] private ApiAuthenticationStateProvider ApiAuthStateProvider { get; set; } = default!;
        [Inject] private IOptions<ApiSettings> ApiOptions { get; set; } = default!;
        [Inject] private UiLocalizer L { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            AuthenticationStateProvider.AuthenticationStateChanged += OnAuthenticationStateChanged;
            await LoadUserAsync();

            try
            {
                isDark = await JS.InvokeAsync<bool>("profileMenu.getSavedTheme");
                await ApplyTheme(isDark);
            }
            catch { }
        }

        private async Task LoadUserAsync()
        {
            var auth = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            user = auth.User;
            StateHasChanged();
        }

        private void OnAuthenticationStateChanged(Task<AuthenticationState> task)
        {
            InvokeAsync(async () =>
            {
                user = (await task).User;
                StateHasChanged();
            });
        }

        private string? GetClaim(string type) => user?.FindFirst(type)?.Value;

        private string ResolveProfilePictureUrl(string? claimValue)
        {
            if (string.IsNullOrWhiteSpace(claimValue))
                return "/images/profiles/profile-pics/default-icon.jpg";

            // if already absolute (http/https), just return
            if (Uri.TryCreate(claimValue, UriKind.Absolute, out _))
                return claimValue;

            // otherwise prepend API base URL
            return $"{ApiOptions.Value.BaseUrl}{claimValue}";
        }

        private void ToggleSidebar() => isSidebarOpen = !isSidebarOpen;
        private void CloseSidebar() => isSidebarOpen = false;

        private async Task ToggleTheme(ChangeEventArgs e)
        {
            isDark = (bool)e.Value;
            await ApplyTheme(isDark);
        }

        private async Task ApplyTheme(bool dark)
        {
            try { await JS.InvokeVoidAsync("profileMenu.setTheme", dark ? "dark" : "light"); }
            catch { }
        }

        private async Task Logout()
        {
            try
            {
                var res = await JS.InvokeAsync<JsonElement>("appAuth.post", "/api/auth/logout");
                var ok = res.GetProperty("ok").GetBoolean();
                if (ok && ApiAuthStateProvider != null)
                    ApiAuthStateProvider.NotifyUserLogout();

                Nav.NavigateTo("/", forceLoad: true);
            }
            catch
            {
                Nav.NavigateTo("/logout", forceLoad: true);
            }
        }
    }

}
