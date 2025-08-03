using global::InstituteManagement.Shared.DTOs.Signup;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using Timer = System.Timers.Timer;

namespace InstituteManagement.Blazor.Client.Pages
{
    public class SignupBase : ComponentBase, IDisposable
    {
        [Inject] protected HttpClient Http { get; set; } = default!;
        [Inject] protected IJSRuntime JS { get; set; } = default!;

        protected bool isSubmitting = false;

        protected SignupDto signup = new()
        {
            NationalityCode = NationalityCode.IR
        };

        protected List<NationalityCode> NationalityOptions = Enum
            .GetValues(typeof(NationalityCode))
            .Cast<NationalityCode>()
            .ToList();

        protected EditContext editContext;
        protected ValidationMessageStore messageStore;
        protected string responseMessage;
        protected bool SignupSuccess;
        protected string usernameStatus;

        private Timer usernameDebounceTimer;
        private string? _recaptchaToken;
        private bool _recaptchaRequested;

        protected override void OnInitialized()
        {
            editContext = new EditContext(signup);
            messageStore = new ValidationMessageStore(editContext);

            // **NEW**: clear server errors for a field when the user edits it
            editContext.OnFieldChanged += HandleFieldChanged;

            usernameDebounceTimer = new Timer(600) { AutoReset = false };
            usernameDebounceTimer.Elapsed += async (_, _) => await InvokeAsync(ValidateUsernameAvailability);
        }

        private void HandleFieldChanged(object sender, FieldChangedEventArgs e)
        {
            // remove any server‐side messages for this field
            messageStore.Clear(e.FieldIdentifier);
            StateHasChanged();
        }



        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && !_recaptchaRequested)
            {
                _recaptchaRequested = true;
                try
                {
                    _recaptchaToken = await JS.InvokeAsync<string>("getRecaptchaToken");
                    signup.RecaptchaToken = _recaptchaToken;
                    await JS.InvokeVoidAsync("flatpickr", "#birthdate", new { dateFormat = "Y-m-d" });

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"reCAPTCHA error: {ex.Message}");
                }
            }
        }
        protected string formattedBirthDate
        {
            get => signup.Birthday.ToString("yyyy-MM-dd");
            set
            {
                if (DateTime.TryParse(value, out var date))
                    signup.Birthday = DateOnly.FromDateTime(date);
            }
        }

        protected async Task HandleSignup()
        {
            if (isSubmitting)
                return;

            isSubmitting = true;
            responseMessage = string.Empty;

            try
            {
                signup.RecaptchaToken = await JS.InvokeAsync<string>("getRecaptchaToken");

                messageStore.Clear();
                editContext.NotifyValidationStateChanged();

                var response = await Http.PostAsJsonAsync("api/signup", signup);

                if (response.IsSuccessStatusCode)
                {
                    responseMessage = "Signup successful!";
                    SignupSuccess = true;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    var errorData = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
                    if (errorData?.Errors != null)
                    {
                        foreach (var kv in errorData.Errors)
                            messageStore.Add(editContext.Field(kv.Key), kv.Value);

                        editContext.NotifyValidationStateChanged();
                    }
                    else
                    {
                        responseMessage = "Signup failed with unknown error.";
                    }
                }
                else
                {
                    responseMessage = "Signup failed due to server error.";
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                responseMessage = "An unexpected error occurred.";
            }

            isSubmitting = false;
        }


        protected void CheckUsername(ChangeEventArgs args)
        {
            usernameDebounceTimer.Stop();
            usernameDebounceTimer.Start();
        }

        protected async Task ValidateUsernameAvailability()
        {
            if (string.IsNullOrWhiteSpace(signup.UserName))
            {
                usernameStatus = string.Empty;
                return;
            }

            try
            {
                var result = await Http.GetAsync($"api/signup/check-username?username={Uri.EscapeDataString(signup.UserName)}");
                if (result.IsSuccessStatusCode)
                {
                    bool isAvailable = await result.Content.ReadFromJsonAsync<bool>();
                    usernameStatus = isAvailable ? "Username is available" : "Username is already taken";
                }
                else
                {
                    usernameStatus = "Could not verify username.";
                }
            }
            catch
            {
                usernameStatus = "Network error checking username.";
            }

            StateHasChanged();
        }

        public void Dispose()
        {
            usernameDebounceTimer?.Dispose();
            editContext.OnFieldChanged -= HandleFieldChanged;
        }
    }

}
