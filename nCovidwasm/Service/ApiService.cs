using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ncovid.Service
{
    using System.Net.Http;
    using System.Text.Json;
    using Microsoft.AspNetCore.Components;
    using NCovid.Service;

    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(HttpClient client)
        {
            _httpClient = client;
        }

        public async Task<List<CountryResult>> GetCountries()
        {
            var response = await _httpClient.GetJsonAsync<List<CountryResult>>("api/corona/countries");
            return response;
        }
    }
}
