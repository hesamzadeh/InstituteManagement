using InstituteManagement.Shared;
using InstituteManagement.Shared.DTOs.UserProfile;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Globalization;
using System.Text.Json;
using System.Net.Http;
using System.Net.Http.Headers;

namespace InstituteManagement.Front.Components.Pages;

public partial class UserProfile
{
    [Inject] private IJSRuntime JS { get; set; } = default!;
    [Inject] private NavigationManager Nav { get; set; } = default!;
    [Inject] private UiLocalizer L { get; set; } = default!;
    [Inject] private HttpClient Http { get; set; }

    private PersonDto? person;
    private UpdateAccountDto account = new();
    private string? confirmPassword;

    private bool loading = true;
    private bool isAuthenticated = false;
    private string? loadError;
    private bool editMode = false;
    private bool accountEditMode = false;

    private string? confirmPasswordError;

    // 🔹 Error handling like SignIn
    private string? responseMessageKey;
    private string? rawResponseMessage;
    private ValidationMessageStore? messageStore;

    // 🔹 EditContexts for forms
    private EditContext? accountEditContext;
    private EditContext? profileEditContext;
    private string? ProfilePicUrl;   
    protected override async Task OnInitializedAsync()
    {
        accountEditContext = new EditContext(account);
        profileEditContext = new EditContext(person ?? new PersonDto());
        messageStore = new ValidationMessageStore(accountEditContext);

        accountEditContext.OnFieldChanged += (_, args) =>
        {
            messageStore?.Clear(args.FieldIdentifier);
            responseMessageKey = null;
            rawResponseMessage = null;
            confirmPasswordError = null;
            InvokeAsync(StateHasChanged);
        };

        profileEditContext.OnFieldChanged += (_, args) =>
        {
            responseMessageKey = null;
            rawResponseMessage = null;
            InvokeAsync(StateHasChanged);
        };

        await LoadProfileAsync();
        await LoadAccountAsync();
    }
    private async Task UploadProfilePic(InputFileChangeEventArgs e)
    {
        var file = e.File;
        using var stream = file.OpenReadStream(5 * 1024 * 1024); // 5 MB limit
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);
        var bytes = ms.ToArray();

        // Build JS FormData with file blob
        var dotNetStream = new DotNetStreamReference(new MemoryStream(bytes));
        await JS.InvokeVoidAsync("eval", @"
        window.__buildProfileFormData = async (streamRef, fileName) => {
            const arrayBuffer = await streamRef.arrayBuffer();
            const blob = new Blob([arrayBuffer]);
            const formData = new FormData();
            formData.append('file', blob, fileName); // 👈 must match API parameter name
            return formData;
        };
    ");

        var formData = await JS.InvokeAsync<IJSObjectReference>(
            "__buildProfileFormData", dotNetStream, file.Name);

        var result = await JS.InvokeAsync<JsonElement>(
            "appAuth.postForm",
            "/api/UserProfile/upload-profile-pic",
            formData
        );

        if (result.TryGetProperty("ok", out var okProp) && okProp.GetBoolean())
        {
            var url = result.GetProperty("body").GetString();
            ProfilePicUrl = url; // update local property
            StateHasChanged();
        }
        else
        {
            // handle error (log, show toast, etc.)
        }
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
                person = JsonSerializer.Deserialize<PersonDto>(bodyEl.GetRawText(),
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                profileEditContext = new EditContext(person!);
                isAuthenticated = true;
            }
            else
            {
                loadError = L["InvalidResponse"];
            }
        }
        catch
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
                account = JsonSerializer.Deserialize<UpdateAccountDto>(bodyEl.GetRawText(),
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new UpdateAccountDto();
                accountEditContext = new EditContext(account);
                messageStore = new ValidationMessageStore(accountEditContext);
            }
        }
        catch
        {
            loadError = L["FailedToLoadAccount"];
        }
    }

    private async Task SaveAccountAsync()
    {
        responseMessageKey = null;
        rawResponseMessage = null;
        messageStore?.Clear();

        if (!string.IsNullOrWhiteSpace(account.NewPassword) &&
            account.NewPassword != confirmPassword)
        {
            confirmPasswordError = L["PasswordsDoNotMatch"];
            return;
        }
        else
        {
            confirmPasswordError = null;
        }

        if (string.IsNullOrWhiteSpace(account.CurrentPassword))
        {
            responseMessageKey = "CurrentPasswordRequired";
            return;
        }

        try
        {
            string token;
            try
            {
                token = await JS.InvokeAsync<string>("getRecaptchaToken");
            }
            catch
            {
                responseMessageKey = "RecaptchaUnavailable";
                return;
            }

            account.RecaptchaToken = token;
            account.Language = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

            var result = await JS.InvokeAsync<JsonElement>("appAuth.put", "/api/UserProfile/account", account);
            if (!result.TryGetProperty("ok", out var okEl) || !okEl.GetBoolean())
            {
                HandleServerErrors(result, accountEditContext);
                return;
            }

            accountEditMode = false;
            confirmPassword = null;
            await LoadAccountAsync();
        }
        catch
        {
            responseMessageKey = "SaveError";
        }
    }

    private async Task SaveProfileAsync()
    {
        responseMessageKey = null;
        rawResponseMessage = null;

        try
        {
            var result = await JS.InvokeAsync<JsonElement>("appAuth.put", "/api/UserProfile/me", person);
            if (!result.TryGetProperty("ok", out var okEl) || !okEl.GetBoolean())
            {
                HandleServerErrors(result, profileEditContext);
                return;
            }

            editMode = false;
            await LoadProfileAsync();
        }
        catch
        {
            responseMessageKey = "SaveError";
        }
    }

    private async Task RequestValidationAsync()
    {
        try
        {
            var result = await JS.InvokeAsync<JsonElement>("appAuth.post", "/api/UserProfile/request-validation", null);
            if (!result.TryGetProperty("ok", out var okEl) || !okEl.GetBoolean())
            {
                HandleServerErrors(result, profileEditContext);
                return;
            }

            await LoadProfileAsync();
        }
        catch
        {
            responseMessageKey = "ValidationRequestError";
        }
    }

    private void HandleServerErrors(JsonElement result, EditContext? ctx)
    {
        try
        {
            if (result.TryGetProperty("body", out var body))
            {
                if (body.ValueKind == JsonValueKind.Object && body.TryGetProperty("errors", out var errors))
                {
                    foreach (var prop in errors.EnumerateObject())
                    {
                        var fieldName = prop.Name;
                        var messages = prop.Value.EnumerateArray()
                            .Select(x => x.GetString())
                            .Where(s => s != null!)
                            .ToArray();

                        if (messages.Length > 0 && ctx != null)
                            messageStore?.Add(ctx.Field(fieldName), messages);
                    }
                    ctx?.NotifyValidationStateChanged();
                    return;
                }

                if (body.ValueKind == JsonValueKind.String)
                    rawResponseMessage = body.GetString();
                else
                    rawResponseMessage = body.ToString();
            }
            else
            {
                responseMessageKey = "SaveFailed";
            }
        }
        catch
        {
            responseMessageKey = "SaveFailed";
        }
    }
}
