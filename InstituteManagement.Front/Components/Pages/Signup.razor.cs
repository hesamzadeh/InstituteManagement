using InstituteManagement.Shared;
using InstituteManagement.Shared.DTOs.Signup;
using InstituteManagement.Shared.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.JSInterop;
using System.Globalization;
using System.Text.Json;
using Timer = System.Timers.Timer;

namespace InstituteManagement.Front.Components.Pages
{
    public class SignupBase : ComponentBase, IDisposable
    {
        [Inject] protected HttpClient Http { get; set; } = default!;
        [Inject] protected IJSRuntime JS { get; set; } = default!;
        [Inject] protected UiLocalizer L { get; set; } = default!;

        protected SignupDto signup = new() { NationalityCode = NationalityCode.IR };
        protected List<NationalityCode> NationalityOptions =
            Enum.GetValues<NationalityCode>().ToList();

        [SupplyParameterFromQuery(Name = "role")]
        public string? RoleQuery { get; set; }

        protected EditContext editContext;
        protected ValidationMessageStore messageStore;
        protected bool isSubmitting;
        protected bool SignupSuccess;
        /// <summary>
        /// key to use with UiLocalizer at render time (e.g. "Signup.SuccessMessage" or "Signup.ServerError")
        /// </summary>
        protected string? responseMessageKey;
        /// <summary>
        /// raw server-provided message text (already localized by server possibly)
        /// </summary>
        protected string responseMessage = "";
        protected string usernameStatus = "";
        protected bool isUsernameTaken = false;

        private Timer usernameDebounce;
        private bool _recaptchaLoaded;

        // culture
        protected bool isFa => CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "fa";

        protected override void OnInitialized()
        {
            editContext = new EditContext(signup);
            messageStore = new ValidationMessageStore(editContext);


            if (!string.IsNullOrEmpty(RoleQuery) &&
                Enum.TryParse<ProfileType>(RoleQuery, ignoreCase: true, out var parsed))
            {
                signup.InitialRole = parsed;
            }

            editContext.OnFieldChanged += (_, args) =>
                {
                    messageStore.Clear(args.FieldIdentifier);

                    // clear global messages when user edits
                    responseMessage = "";
                    responseMessageKey = null;

                    StateHasChanged();
                };

            usernameDebounce = new Timer(600) { AutoReset = false };
            usernameDebounce.Elapsed += async (_, _) => await InvokeAsync(ValidateUsername);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                // preload reCAPTCHA and datepicker
                try
                {
                    // this will reject if grecaptcha not loaded (we enforce it)
                    await JS.InvokeAsync<string>("getRecaptchaToken");
                    _recaptchaLoaded = true;
                }
                catch
                {
                    _recaptchaLoaded = false;
                    // Do not call L[...] here — set key for markup to translate
                    responseMessageKey = "RecaptchaUnavailable";
                    StateHasChanged();
                    return;
                }

                var locale = isFa ? "fa" : "default";
            }
        }

        protected async Task HandleSignup()
        {
            if (isSubmitting || !_recaptchaLoaded) return;
            isSubmitting = true;
            messageStore.Clear();

            responseMessage = "";
            responseMessageKey = null;

            try
            {
                signup.RecaptchaToken = await JS.InvokeAsync<string>("getRecaptchaToken");
            }
            catch
            {
                responseMessageKey = "RecaptchaUnavailable";
                isSubmitting = false;
                return;
            }

            signup.Language = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

            var resp = await Http.PostAsJsonAsync("api/signup", signup);
            if (resp.IsSuccessStatusCode)
            {
                SignupSuccess = true;
                responseMessageKey = "SuccessMessage";
            }
            else if (resp.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var problem = await resp.Content.ReadFromJsonAsync<ValidationProblemDetails>();
                if (problem?.Errors != null)
                {
                    foreach (var kv in problem.Errors)
                        messageStore.Add(editContext.Field(kv.Key), kv.Value);
                    editContext.NotifyValidationStateChanged();
                }
                else
                {
                    // fallback – try to read textual message
                    try
                    {
                        var text = await resp.Content.ReadAsStringAsync();
                        if (!string.IsNullOrWhiteSpace(text))
                        {
                            // try parse JSON with "message" property
                            try
                            {
                                using var doc = JsonDocument.Parse(text);
                                if (doc.RootElement.TryGetProperty("message", out var m) && m.ValueKind == JsonValueKind.String)
                                    responseMessage = m.GetString() ?? "";
                                else
                                    responseMessage = text;
                            }
                            catch
                            {
                                responseMessage = text;
                            }
                        }
                        else
                        {
                            responseMessageKey = "ServerError";
                        }
                    }
                    catch
                    {
                        responseMessageKey = "ServerError";
                    }
                }
            }
            else
            {
                responseMessageKey = "ServerError";
            }

            isSubmitting = false;
            await InvokeAsync(StateHasChanged);
        }

        protected void CheckUsername(FocusEventArgs _) => usernameDebounce.Start();

        private async Task ValidateUsername()
        {
            isUsernameTaken = false;
            usernameStatus = "";

            if (string.IsNullOrWhiteSpace(signup.UserName))
            {
                StateHasChanged();
                return;
            }

            var isAvailable = await Http.GetFromJsonAsync<bool>($"api/signup/check-username?username={Uri.EscapeDataString(signup.UserName)}");

            isUsernameTaken = !isAvailable;

            StateHasChanged();
        }

        public void Dispose()
        {
            usernameDebounce?.Dispose();
        }
    }
}
