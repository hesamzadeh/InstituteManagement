using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;

public class ApiAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly IJSRuntime _js;

    public ApiAuthenticationStateProvider(IHttpClientFactory clientFactory, IJSRuntime js)
    {
        _clientFactory = clientFactory;
        _js = js;
    }

    // Ask API whoami (via JS fetch so cookies go with browser fetch)
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            // call browser fetch via JS helper to include cookies
            var res = await _js.InvokeAsync<JsonElement>("appAuth.whoami", "/api/auth/whoami");
            if (res.ValueKind == JsonValueKind.Object)
            {
                var isAuthenticated = res.GetProperty("IsAuthenticated").GetBoolean();
                if (isAuthenticated)
                {
                    var username = res.GetProperty("Username").GetString() ?? "";
                    var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, username) }, "apiauth");
                    return new AuthenticationState(new ClaimsPrincipal(identity));
                }
            }
        }
        catch
        {
            // ignore - return anonymous
        }

        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
    }

    public void NotifyUserAuthentication() =>
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

    public void NotifyUserLogout() =>
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()))));
}
