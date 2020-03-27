using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ncovid.web.Pages
{
    using NCovid.Service;
    using Services;

    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ApiService _apiService;

        public IList<CountryResult> Countries { get; set; }
        public AllResults AllResults { get; set; }
        public IndexModel(ILogger<IndexModel> logger, ApiService apiService)
        {
            _logger = logger;
            _apiService = apiService;
        }

        public async Task OnGetAsync()
        {
            AllResults = await _apiService.GetAll();
            Countries = await _apiService.GetCountries();
        }
    }
}
