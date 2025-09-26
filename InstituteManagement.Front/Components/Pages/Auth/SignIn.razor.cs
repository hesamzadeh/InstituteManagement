using InstituteManagement.Shared;
using InstituteManagement.Shared.DTOs.Auth;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;

namespace InstituteManagement.Front.Components.Pages.Auth
{
    public class SignInBase : ComponentBase, IDisposable
    {
        [Inject] protected HttpClient Http { get; set; } = default!;
        [Inject] protected IJSRuntime JS { get; set; } = default!;
        [Inject] protected NavigationManager Nav { get; set; } = default!;
        [Inject] protected ApiAuthenticationStateProvider AuthStateProvider { get; set; } = default!;
        [Inject] protected UiLocalizer L { get; set; } = default!;

        protected readonly SignInDto model = new();
        protected EditContext? editContext;
        protected ValidationMessageStore? messageStore;
        protected bool isSubmitting;
        protected string? message;
        private bool _jsReady;
        protected bool recaptchaFailed;

        protected override void OnInitialized()
        {
            editContext = new EditContext(model);
            messageStore = new ValidationMessageStore(editContext);
            editContext.OnFieldChanged += (_, args) =>
            {
                // clear specific field errors when user edits
                messageStore?.Clear(args.FieldIdentifier);
                InvokeAsync(StateHasChanged);
            };
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                // quick JS check — will throw during server prerender
                try
                {
                    await JS.InvokeVoidAsync("console.debug", "signin: js-runtime-ready");
                    _jsReady = true;

                    // optionally pre-fetch a recaptcha token to warm things up (harmless)
                    try
                    {
                        await JS.InvokeAsync<string>("getRecaptchaToken");
                    }
                    catch
                    {
                        // ignore if recaptcha helper not present
                    }

                    StateHasChanged();
                }
                catch
                {
                    _jsReady = false;
                }
            }
        }

        protected async Task HandleSignIn()
        {
            // reset
            message = null;
            messageStore?.Clear();
            editContext?.NotifyValidationStateChanged();

            if (isSubmitting) return;

            if (!_jsReady)
            {
                message = L["WaitForPageToLoad"];
                return;
            }

            isSubmitting = true;

            try
            {
                // Obtain reCAPTCHA token — must succeed. No dev-mode fallback.
                try
                {
                    model.RecaptchaToken = await JS.InvokeAsync<string>("getRecaptchaToken");
                }
                catch
                {
                    // localized key - add to resources:
                    // "RecaptchaUnavailable"
                    message = L["RecaptchaUnavailable"];
                    return;
                }

                model.Language = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

                JsonElement result;
                try
                {
                    result = await JS.InvokeAsync<JsonElement>("appAuth.signIn", "/api/auth/signin", model);
                }
                catch (JSException jsex)
                {
                    message = L["ClientSignInError"] + ": " + jsex.Message;
                    return;
                }

                bool ok = false;
                try { ok = result.GetProperty("ok").GetBoolean(); } catch { ok = false; }

                if (!ok)
                {
                    try
                    {
                        var body = result.GetProperty("body");

                        // If server returned validation errors: { errors: { field: [msgs] } }
                        if (body.ValueKind == JsonValueKind.Object && body.TryGetProperty("errors", out var errors))
                        {
                            foreach (var prop in errors.EnumerateObject())
                            {
                                var fieldName = prop.Name;
                                var messages = prop.Value.EnumerateArray()
                                                        .Select(x => x.GetString())
                                                        .Where(s => s != null!)
                                                        .ToArray();
                                if (messages.Length > 0)
                                    messageStore?.Add(editContext!.Field(fieldName), messages);
                                if (fieldName.Contains("Recaptcha", StringComparison.OrdinalIgnoreCase))
                                {
                                    recaptchaFailed = true; // just set flag
                                }
                            }

                            editContext?.NotifyValidationStateChanged();

                            return;
                        }

                        // If body is an object, try to find a textual message in common properties (case-insensitive)
                        if (body.ValueKind == JsonValueKind.Object)
                        {
                            string? extractedMessage = null;

                            foreach (var prop in body.EnumerateObject())
                            {
                                var name = prop.Name.ToLowerInvariant();
                                if (name == "message" || name == "error" || name == "detail" || name == "title" || name == "description")
                                {
                                    if (prop.Value.ValueKind == JsonValueKind.String)
                                        extractedMessage = prop.Value.GetString();
                                    else
                                        extractedMessage = prop.Value.ToString(); // fallback to JSON text
                                    break;
                                }
                            }

                            // also check nested "body" -> { message: ... }
                            if (extractedMessage == null && body.TryGetProperty("body", out var nested))
                            {
                                if (nested.ValueKind == JsonValueKind.String)
                                    extractedMessage = nested.GetString();
                                else if (nested.ValueKind == JsonValueKind.Object && nested.TryGetProperty("message", out var nm) && nm.ValueKind == JsonValueKind.String)
                                    extractedMessage = nm.GetString();
                            }

                            if (!string.IsNullOrEmpty(extractedMessage))
                            {
                                message = extractedMessage;
                                return;
                            }

                            // nothing found — show the entire object as fallback (safe)
                            message = body.ToString();
                            return;
                        }

                        // If body is a plain string, display it. If it contains JSON text, try to parse a "message" inside.
                        if (body.ValueKind == JsonValueKind.String)
                        {
                            var txt = body.GetString();
                            if (!string.IsNullOrWhiteSpace(txt))
                            {
                                // try to parse as JSON
                                try
                                {
                                    var doc = JsonDocument.Parse(txt);
                                    var root = doc.RootElement;
                                    if (root.ValueKind == JsonValueKind.Object)
                                    {
                                        if (root.TryGetProperty("message", out var m3) && m3.ValueKind == JsonValueKind.String)
                                        {
                                            message = m3.GetString();
                                            return;
                                        }
                                    }
                                }
                                catch
                                {
                                    // not JSON — fall through to display raw string
                                }

                                message = txt;
                                return;
                            }
                        }
                    }
                    catch
                    {
                        // swallow parse errors and fall through to generic message
                    }

                    // final fallback: localized generic message (ensure this key exists in your SignIn resources)
                    message = L["SignInFailed"];
                    return;
                }


                AuthStateProvider.NotifyUserAuthentication();
                Nav.NavigateTo("/", forceLoad: false);
            }
            finally
            {
                isSubmitting = false;
                await InvokeAsync(StateHasChanged);
            }
        }


        public void Dispose()
        {
            if (editContext != null)
                editContext.OnFieldChanged -= (_, _) => { };
        }
    }
}
