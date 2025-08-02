using System.Net.Http.Json;
using InstituteManagement.Shared.Common;
using InstituteManagement.Shared.DTOs.Persons;

namespace InstituteManagement.Blazor.Services
{
    public class PersonService(HttpClient http)
    {
        private readonly HttpClient _http = http;

        public async Task<List<PersonDto>> GetAllAsync()
        {
            var response = await _http.GetFromJsonAsync<ApiResponse<List<PersonDto>>>("api/person");
            return response?.Data ?? [];
        }

        public async Task<bool> CreateAsync(CreatePersonDto dto)
        {
            var response = await _http.PostAsJsonAsync("api/person", dto);
            return response.IsSuccessStatusCode;
        }
    }

}
