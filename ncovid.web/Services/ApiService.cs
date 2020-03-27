namespace ncovid.web.Services
{
    using NCovid.Service;

    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;

    /// <summary>
    /// Defines the <see cref="ApiService" />.
    /// </summary>
    public class ApiService
    {
        /// <summary>
        /// Defines the _httpClient.
        /// </summary>
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiService"/> class.
        /// </summary>
        /// <param name="client">The client<see cref="HttpClient"/>.</param>
        public ApiService(HttpClient client)
        {
            _httpClient = client;
        }

        /// <summary>
        /// Get Summery Info For Corona Infections.
        /// </summary>
        /// <returns>The <see cref="AllResults"/>.</returns>
        public async Task<AllResults> GetAll()
        {
            var option = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            var response = await _httpClient.GetAsync("api/corona/all");
            response.EnsureSuccessStatusCode();

            await using var responseContent = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<AllResults>(responseContent, option);
        }

        /// <summary>
        /// Get All Data For Countries
        /// </summary>
        /// <returns>The <see cref="List{CountryResult}"/>.</returns>
        public async Task<List<CountryResult>> GetCountries()
        {
            //var response = await _httpClient.GetJsonAsync<List<CountryResult>>("api/corona/countries");
            //return response;
            var option = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            var response = await _httpClient.GetAsync("api/corona/countries");
            response.EnsureSuccessStatusCode();

            await using var responseContent = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<List<CountryResult>>(responseContent, option);
        }
    }

   
}
