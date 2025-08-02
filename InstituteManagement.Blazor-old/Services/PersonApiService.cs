using InstituteManagement.Shared.Common;
using InstituteManagement.Shared.DTOs.Persons;

namespace InstituteManagement.Blazor.Services
{
    public class PersonApiService
    {
        private readonly HttpClient _http;

        public PersonApiService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<PersonDto>> GetPeopleAsync()
        {
            return await _http.GetFromJsonAsync<List<PersonDto>>("api/people");
        }

        public async Task<PersonDto?> CreatePersonAsync(CreatePersonDto dto)
        {
            var response = await _http.PostAsJsonAsync("api/people", dto);
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<PersonDto>>();
                return apiResponse?.Data;
            }

            return null;
        }
    }
}
