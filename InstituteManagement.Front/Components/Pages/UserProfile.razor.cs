using InstituteManagement.Shared;
using InstituteManagement.Shared.DTOs.UserProfile;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Text.Json;
using System.Globalization;

namespace InstituteManagement.Front.Components.Pages;

public partial class UserProfile
{
    [Inject] private IJSRuntime JS { get; set; } = default!;
    [Inject] private NavigationManager Nav { get; set; } = default!;
    [Inject] private UiLocalizer L { get; set; } = default!;

    private PersonDto? person;
    private UpdateAccountDto account = new();
    private string? confirmPassword; // client-side only
    private bool loading = true;
    private bool isAuthenticated = false;
    private string? loadError;
    private bool editMode = false;
    private bool accountEditMode = false;

    private string? confirmPasswordError;      // error message if passwords don't match

    protected override async Task OnInitializedAsync()
    {
        await LoadProfileAsync();
        await LoadAccountAsync(); // ensure email & phone load
    }

    private async Task LoadProfileAsync()
    {
        loading = true;
        loadError = null;
        person = null;
        isAuthenticated = false;

        try
        {
            var result = await JS.InvokeAsync<JsonElement>("appAuth.get", "/api/UserProfile/me");
            if (!result.TryGetProperty("ok", out var okEl) || !okEl.GetBoolean())
            {
                var status = result.TryGetProperty("status", out var stEl) ? stEl.GetInt32() : 0;
                if (status == 401 || status == 403)
                    isAuthenticated = false;
                else
                    loadError = result.TryGetProperty("body", out var body) ? body.ToString() : L["LoadFailed"];
                return;
            }

            if (result.TryGetProperty("body", out var bodyEl) && bodyEl.ValueKind == JsonValueKind.Object)
            {
                person = JsonSerializer.Deserialize<PersonDto>(
                    bodyEl.GetRawText(),
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
                isAuthenticated = true;
            }
            else
            {
                loadError = L["InvalidResponse"];
            }
        }
        catch (JSException jsEx)
        {
            loadError = L["ClientError"];
        }
        catch (Exception ex)
        {
            loadError = L["UnexpectedError"];
        }
        finally
        {
            loading = false;
        }
    }

    private async Task LoadAccountAsync()
    {
        try
        {
            var result = await JS.InvokeAsync<JsonElement>("appAuth.get", "/api/UserProfile/account");

            if (result.TryGetProperty("ok", out var okEl) && okEl.GetBoolean()
                && result.TryGetProperty("body", out var bodyEl) && bodyEl.ValueKind == JsonValueKind.Object)
            {
                account = JsonSerializer.Deserialize<UpdateAccountDto>(
                    bodyEl.GetRawText(),
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                ) ?? new UpdateAccountDto();
            }
        }
        catch (Exception ex)
        {
            loadError = L["FailedToLoadAccount"];
        }
    }

    private async Task SaveAccountAsync()
    {
        // Client-side confirm password validation
        if (!string.IsNullOrWhiteSpace(account.NewPassword) &&
            account.NewPassword != confirmPassword)
        {
            confirmPasswordError = L["PasswordsDoNotMatch"];
            return;
        }
        else
        {
            confirmPasswordError = null; // clear previous error
        }

        // Require current password
        if (string.IsNullOrWhiteSpace(account.CurrentPassword))
        {
            loadError = L["CurrentPasswordRequired"];
            return;
        }

        try
        {
            // request reCAPTCHA token
            string token;
            try
            {
                token = await JS.InvokeAsync<string>("getRecaptchaToken");
            }
            catch
            {
                loadError = L["RecaptchaUnavailable"];
                return;
            }

            // include recaptcha + culture info
            account.RecaptchaToken = token;
            account.Language = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

            var result = await JS.InvokeAsync<JsonElement>("appAuth.put", "/api/UserProfile/account", account);
            bool ok = result.TryGetProperty("ok", out var okEl) && okEl.GetBoolean();
            if (!ok)
            {
                loadError = result.TryGetProperty("body", out var body) ? body.ToString() : L["SaveFailed"];
                return;
            }

            accountEditMode = false;
            confirmPassword = null;
            await LoadAccountAsync();
        }
        catch (Exception ex)
        {
            loadError = L["SaveError"];
        }
    }

    private async Task SaveProfileAsync()
    {
        try
        {
            var result = await JS.InvokeAsync<JsonElement>("appAuth.put", "/api/UserProfile/me", person);
            bool ok = result.TryGetProperty("ok", out var okEl) && okEl.GetBoolean();
            if (!ok)
            {
                loadError = result.TryGetProperty("body", out var body) ? body.ToString() : L["SaveFailed"];
                return;
            }

            editMode = false;
            await LoadProfileAsync();
        }
        catch (Exception ex)
        {
            loadError = L["SaveError"];
        }
    }

    private async Task RequestValidationAsync()
    {
        try
        {
            var result = await JS.InvokeAsync<JsonElement>("appAuth.post", "/api/UserProfile/request-validation", null);
            bool ok = result.TryGetProperty("ok", out var okEl) && okEl.GetBoolean();
            if (!ok)
            {
                loadError = result.TryGetProperty("body", out var body) ? body.ToString() : L["ValidationRequestFailed"];
                return;
            }

            await LoadProfileAsync();
        }
        catch (Exception ex)
        {
            loadError = L["ValidationRequestError"];
        }
    }
}
