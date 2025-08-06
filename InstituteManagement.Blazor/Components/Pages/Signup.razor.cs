using InstituteManagement.Shared.DTOs.Signup;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Globalization;
using System.Net.Http.Json;
using System.Timers;
using Timer = System.Timers.Timer;

namespace InstituteManagement.Blazor.Components.Pages
{
    public class SignupBase : ComponentBase, IDisposable
    {
        [Inject] protected HttpClient Http { get; set; } = default!;
        [Inject] protected IJSRuntime JS { get; set; } = default!;
        [Inject] protected IStringLocalizer <Pages.Signup> L { get; set; } = default!;

        protected SignupDto signup = new() { NationalityCode = NationalityCode.IR };
        protected List<NationalityCode> NationalityOptions =
            Enum.GetValues<NationalityCode>().ToList();

        protected EditContext editContext;
        protected ValidationMessageStore messageStore;
        protected bool isSubmitting;
        protected bool SignupSuccess;
        protected string responseMessage = "";
        protected string usernameStatus = "";

        private Timer usernameDebounce;
        private bool _recaptchaLoaded;

        // culture
        protected bool isFa => CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "fa";

        protected override void OnInitialized()
        {
            editContext = new EditContext(signup);
            messageStore = new ValidationMessageStore(editContext);
            editContext.OnFieldChanged += (_, args) =>
            {
                messageStore.Clear(args.FieldIdentifier);
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
                await JS.InvokeVoidAsync("getRecaptchaToken");

                var locale = isFa ? "fa" : "default";
                await JS.InvokeVoidAsync("initDatePicker", "#birthdate", locale);
                _recaptchaLoaded = true;
            }
        }

        protected async Task HandleSignup()
        {
            if (isSubmitting || !_recaptchaLoaded) return;
            isSubmitting = true;
            messageStore.Clear();

            signup.RecaptchaToken = await JS.InvokeAsync<string>("getRecaptchaToken");
            signup.Language = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

            var resp = await Http.PostAsJsonAsync("api/signup", signup);
            if (resp.IsSuccessStatusCode)
            {
                SignupSuccess = true;
                responseMessage = L["Signup.SuccessMessage"];
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
            }
            else
            {
                responseMessage = L["Signup.ServerError"];
            }

            isSubmitting = false;
        }

        protected void CheckUsername(FocusEventArgs _) => usernameDebounce.Start();

        private async Task ValidateUsername()
        {
            if (string.IsNullOrWhiteSpace(signup.UserName))
            {
                usernameStatus = "";
            }
            else
            {

                var ok = await Http.GetFromJsonAsync<bool>($"api/signup/check-username?username={signup.UserName}");
                usernameStatus = ok ? L["Signup.UsernameAvailable"] : L["Signup.UsernameTaken"];

            }
            StateHasChanged();
        }

        public void Dispose()
        {
            usernameDebounce?.Dispose();

        }
    }
}
