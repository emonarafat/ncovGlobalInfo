namespace ncov.api.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using NCovid.Service;
    using NCovid.Service.Services;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="CoronaController" />.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CoronaController : ControllerBase
    {
        /// <summary>
        /// Defines the _coronaVirusService.
        /// </summary>
        private readonly ICoronaVirusService _coronaVirusService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CoronaController"/> class.
        /// </summary>
        /// <param name="coronaVirusService">The coronaVirusService<see cref="ICoronaVirusService"/>.</param>
        public CoronaController(ICoronaVirusService coronaVirusService)
        {
            _coronaVirusService = coronaVirusService;
        }

        /// <summary>
        /// The All.
        /// </summary>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpGet("all")]
        public async Task<IActionResult> All()
        {
            return Ok(await _coronaVirusService.GetAllData());
        }
        //countries
        [HttpGet("countries")]
        public async Task<List<CountryResult>> Countries()
        {
            return await _coronaVirusService.GetCountriesData();
        }
        /// <summary>
        /// The Get.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }
    }
}
