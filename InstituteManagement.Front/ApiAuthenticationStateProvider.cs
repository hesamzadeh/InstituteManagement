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
            var res = await _js.InvokeAsync<JsonElement>("appAuth.whoami", "/api/auth/whoami");
            if (res.ValueKind == JsonValueKind.Object && res.GetProperty("IsAuthenticated").GetBoolean())
            {
                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, res.GetProperty("FullName").GetString() ?? ""),
                new Claim(ClaimTypes.Upn, res.GetProperty("Username").GetString() ?? "")
            };

                // optional additional claims
                if (res.TryGetProperty("profilePictureUrl", out var pic))
                    claims.Add(new Claim("ProfilePictureUrl", pic.GetString() ?? ""));

                if (res.TryGetProperty("firstName", out var fn))
                    claims.Add(new Claim("FirstName", fn.GetString() ?? ""));

                if (res.TryGetProperty("lastName", out var ln))
                    claims.Add(new Claim("LastName", ln.GetString() ?? ""));

                if (res.TryGetProperty("lastUsedProfileId", out var lpid))
                    claims.Add(new Claim("LastUsedProfileId", lpid.GetString() ?? ""));

                var identity = new ClaimsIdentity(claims, "apiauth");
                return new AuthenticationState(new ClaimsPrincipal(identity));
            }
        }
        catch
        {
            // ignore
        }

        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
    }


    public void NotifyUserAuthentication() =>
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

    public void NotifyUserLogout() =>
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()))));
}
