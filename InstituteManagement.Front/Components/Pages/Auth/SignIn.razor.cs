using InstituteManagement.Shared;
using InstituteManagement.Shared.DTOs.Auth;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System.Globalization;
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
        /// <summary>
        /// localized key to be rendered (use L[responseMessageKey] in markup). Prefer this for localizable messages.
        /// </summary>
        protected string? responseMessageKey;
        /// <summary>
        /// raw message returned by server or JS exception text (already localized by server if needed)
        /// </summary>
        protected string? rawResponseMessage;
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

                // Also clear any global messages (keys or raw) so user sees fresh state
                responseMessageKey = null;
                rawResponseMessage = null;
                recaptchaFailed = false;

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
            responseMessageKey = null;
            rawResponseMessage = null;
            messageStore?.Clear();
            editContext?.NotifyValidationStateChanged();

            if (isSubmitting) return;

            if (!_jsReady)
            {
                // show localized message in markup: set key, do not call L[...] here
                responseMessageKey = "WaitForPageToLoad";
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
                catch (Exception)
                {
                    // set a key — actual localization happens in Razor markup
                    responseMessageKey = "RecaptchaUnavailable";
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
                    // non-localized raw message (JS exceptions are not localized on client)
                    rawResponseMessage = $"{L["ClientSignInError"]}: {jsex.Message}";
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
                                    // mark recaptcha failure and let markup show localized text
                                    recaptchaFailed = true;
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
                                // server may already localized this message; show raw text
                                rawResponseMessage = extractedMessage;
                                return;
                            }

                            // nothing found — show the entire object as fallback (safe)
                            rawResponseMessage = body.ToString();
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
                                            rawResponseMessage = m3.GetString();
                                            return;
                                        }
                                    }
                                }
                                catch
                                {
                                    // not JSON — fall through to display raw string
                                }

                                rawResponseMessage = txt;
                                return;
                            }
                        }
                    }
                    catch
                    {
                        // swallow parse errors and fall through to generic message
                    }

                    // final fallback: set a localized key for render-time translation
                    responseMessageKey = "SignInFailed";
                    return;
                }

                // success
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
