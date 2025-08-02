using System.Net.Http;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace InstituteManagement.API.Services
{
    public class GoogleRecaptchaValidator : ICaptchaValidator
    {
        private readonly HttpClient _httpClient;
        private readonly string _secretKey;

        public GoogleRecaptchaValidator(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _secretKey = configuration["Captcha:SecretKey"]; // Add to appsettings.json
        }

        public async Task<bool> IsCaptchaValid(string token, string action, double threshold = 0.5)
        {
            var response = await _httpClient.PostAsync(
                $"https://www.google.com/recaptcha/api/siteverify?secret={_secretKey}&response={token}",
                null);

            if (!response.IsSuccessStatusCode)
                return false;

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<RecaptchaResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return data != null && data.Success && data.Action == action && data.Score >= threshold;
        }

        private class RecaptchaResponse
        {
            public bool Success { get; set; }
            public double Score { get; set; }
            public string Action { get; set; }
            public DateTime Challenge_ts { get; set; }
            public string Hostname { get; set; }
            public List<string> ErrorCodes { get; set; }
        }
    }
}
