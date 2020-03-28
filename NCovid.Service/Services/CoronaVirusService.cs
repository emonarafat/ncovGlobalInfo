namespace NCovid.Service.Services
{
    using AngleSharp;
    using AngleSharp.Dom;
    using DataContext;
    using Hubs;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;


    /// <summary>
    /// Defines the <see cref="CoronaVirusService" />.
    /// </summary>
    public class CoronaVirusService : ICoronaVirusService
    {
        private readonly CoronaDbContext _dbContext;
        /// <summary>
        /// Defines the CountriesTodayTbodyTr.
        /// </summary>
        private const string CountriesTodayTbodyTr = "table#main_table_countries_today tbody tr";

        /// <summary>
        /// Defines the BaseUrl..
        /// </summary>
        private const string BaseUrl = "https://www.worldometers.info/coronavirus/";

        /// <summary>
        /// Defines the MainCounterSelector..
        /// </summary>
        private const string MainCounterSelector = ".maincounter-number";

        /// <summary>
        /// Defines the _hubContext..
        /// </summary>
        private readonly IHubContext<CoronaHub> _hubContext;

        /// <summary>
        /// Defines the _mapper..
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="CoronaVirusService"/> class.
        /// </summary>
        /// <param name="hubContext">The hubContext<see cref="IHubContext{CoronaHub}" />.</param>
        /// <param name="mapper">The mapper<see cref="IMapper" />.</param>
        public CoronaVirusService(IHubContext<CoronaHub> hubContext, IMapper mapper, CoronaDbContext dbContext)
        {
            _hubContext = hubContext;
            _mapper = mapper;
            _dbContext = dbContext;
            SaveInfo();
        }

        /// <summary>
        /// Get Global Info Summery.
        /// </summary>
        /// <returns>Gl;global Info Summery <see cref="Task{AllResults}" />.</returns>
        public async Task<AllResults> GetAllData()
        {

            var summery= await _dbContext.All.FirstOrDefaultAsync();
            return summery != null ? _mapper.Map<AllResults>(summery) : await ParseGlobalInfoSummery();
        }

        /// <summary>
        /// Get Countries Data.
        /// </summary>
        /// <returns>List of Countries <see cref="List{CountryResult}" />.</returns>
        public async Task<List<CountryResult>> GetCountriesData()
        {
          
            var data = (await _dbContext.Countries.OrderBy(o => o.Cases)
                .ToListAsync()).Select(c => _mapper.Map<CountryResult>(c)).ToList();
            return data.Any() ? data :await ParseCountriesData();
        }

        /// <summary>
        /// Get Countries Data by Searched Criteria.
        /// </summary>
        /// <param name="search">The search<see cref="string" />.</param>
        /// <returns>List of Countries <see cref="List{CountryResult}" />.</returns>
        public async Task<List<CountryResult>> GetCountriesData(string search)
        {

            var data = (await _dbContext.Countries.Where(c => c.Country.Contains(search)).OrderBy(o => o.Cases)
                .ToListAsync()).Select(c => _mapper.Map<CountryResult>(c)).ToList();
            return data.Any() ? data : (await ParseCountriesData()).Where(c => c.Country.Contains(search)).OrderBy(o => o.Cases).ToList();
        }

        /// <summary>
        /// Save Info to Database to avoid Calling worldometers and get banned.
        /// </summary>
        public async void SaveInfo()
        {
            await RecurringTask(async () => { await FetchSaveInformationToDatabase(); }, 120, CancellationToken.None);
        }

        /// <summary>
        /// The CountryResults.
        /// </summary>
        /// <param name="document">The document<see cref="IParentNode"/>.</param>
        /// <returns>The <see cref="List{CountryResult}"/>.</returns>
        private static List<CountryResult> CountryResults(IParentNode document)
        {
            var result = new List<CountryResult>();
            var countriesTableCells = document.QuerySelectorAll(CountriesTodayTbodyTr);
            //var totalColumns = 10;
            const int countryColIndex = 0;
            const int casesColIndex = 1;
            const int todayCasesColIndex = 2;
            const int deathsColIndex = 3;
            const int todayDeathsColIndex = 4;
            const int curedColIndex = 5;
            const int activeColIndex = 6;
            const int criticalColIndex = 7;
            const int casesPerOneMillionColIndex = 8;
            const int deathsPerOneMillionColIndex = 9;
            const int firstCaseColIndex = 10;
            result.AddRange(countriesTableCells.Select(row => (row, cells: row.QuerySelectorAll("td")))
                .Select(t => new {t, country = GetCountry(countryColIndex, t.cells)})
                .Where(t => t.country != "Total:")
                .Select(t => new CountryResult
                {
                    Country = t.country,
                    Cases = t.t.cells[casesColIndex].TextContent.ToInt(),
                    Active = t.t.cells[activeColIndex].TextContent.ToInt(),
                    CasesPerOneMillion = t.t.cells[casesPerOneMillionColIndex].TextContent.ToDecimal(),
                    Critical = t.t.cells[criticalColIndex].TextContent.ToInt(),
                    Deaths = t.t.cells[deathsColIndex].TextContent.ToInt(),
                    DeathsPerOneMillion = t.t.cells[deathsPerOneMillionColIndex].TextContent.ToDecimal(),
                    Recovered = t.t.cells[curedColIndex].TextContent.ToInt(),
                    TodayCases = t.t.cells[todayCasesColIndex].TextContent.ToInt(),
                    TodayDeaths = t.t.cells[todayDeathsColIndex].TextContent.ToInt(),
                    FirstCase = t.t.cells[firstCaseColIndex]?.TextContent?.Trim()
                }));

            return result;
        }

        /// <summary>
        /// Get All Info.
        /// </summary>
        /// <returns>(Summery,Countries).</returns>
        private static async ValueTask<(AllResults All, List<CountryResult> Countries)> GetAllInfo()
        {
            using var document = await ReadDocument();
            return (All: ParseGlobalInfoSummery(document), Countries: ParseCountriesData(document));
        }

        /// <summary>
        /// Parse Country from a link or span.
        /// </summary>
        /// <param name="countryColIndex">The countryColIndex<see cref="int" />.</param>
        /// <param name="cells">The cells<see cref="IHtmlCollection{IElement}" />.</param>
        /// <returns>Country Name <see cref="string" />.</returns>
        private static string GetCountry(int countryColIndex, IHtmlCollection<IElement> cells)
        {
            var country = cells[countryColIndex].TextContent;
            return country.IsSet() ? country : cells[countryColIndex].QuerySelector("a")?.TextContent.Trim() ?? "";
        }

        /// <summary>
        /// The GlobalInfoSummery.
        /// </summary>
        /// <param name="document">The document<see cref="IParentNode"/>.</param>
        /// <returns>The <see cref="AllResults"/>.</returns>
        private static AllResults GlobalInfoSummery(IParentNode document)
        {
            var selectorAll = document.QuerySelectorAll(MainCounterSelector);
            var result = new AllResults();
            for (var i = 0; i < selectorAll.Length; i++)
            {
                int.TryParse((selectorAll[i].QuerySelector("span").TextContent?.Trim() ?? "0").Replace(",", ""),
                    out var count);
                switch (i)
                {
                    case 0:
                        result.Cases = count;
                        break;
                    case 1:
                        result.Deaths = count;
                        break;
                    default:
                        result.Recovered = count;
                        break;
                }
            }


            return result;
        }

        /// <summary>
        /// The ParseCountriesData.
        /// </summary>
        /// <returns>The <see cref="List{CountryResult}" />.</returns>
        private static async Task<List<CountryResult>> ParseCountriesData()
        {
            var result = new List<CountryResult>();
            var document = await ReadDocument();
            var countriesTableCells = document.QuerySelectorAll("table#main_table_countries_today tbody tr");
            //var totalColumns = 10;
            const int countryColIndex = 0;
            const int casesColIndex = 1;
            const int todayCasesColIndex = 2;
            const int deathsColIndex = 3;
            const int todayDeathsColIndex = 4;
            const int curedColIndex = 5;
            const int activeColIndex = 6;
            const int criticalColIndex = 7;
            const int casesPerOneMillionColIndex = 8;
            const int deathsPerOneMillionColIndex = 9;
            const int firstCaseColIndex = 10;
            result.AddRange(countriesTableCells.Select(row => (row, cells: row.QuerySelectorAll("td")))
                .Select(t => new {t, country = GetCountry(countryColIndex, t.cells)})
                .Where(t => t.country != "Total:")
                .Select(t => new CountryResult
                {
                    Country = t.country,
                    Cases = t.t.cells[casesColIndex].TextContent.ToInt(),
                    Active = t.t.cells[activeColIndex].TextContent.ToInt(),
                    CasesPerOneMillion = t.t.cells[casesPerOneMillionColIndex].TextContent.ToDecimal(),
                    Critical = t.t.cells[criticalColIndex].TextContent.ToInt(),
                    Deaths = t.t.cells[deathsColIndex].TextContent.ToInt(),
                    DeathsPerOneMillion = t.t.cells[deathsPerOneMillionColIndex].TextContent.ToDecimal(),
                    Recovered = t.t.cells[curedColIndex].TextContent.ToInt(),
                    TodayCases = t.t.cells[todayCasesColIndex].TextContent.ToInt(),
                    TodayDeaths = t.t.cells[todayDeathsColIndex].TextContent.ToInt(),
                    FirstCase = t.t.cells[firstCaseColIndex]?.TextContent?.Trim()
                }));

            return result;
        }

        /// <summary>
        ///  Parse Countries Data.
        /// </summary>
        /// <param name="document">The document<see cref="IParentNode"/>.</param>
        /// <returns>The <see cref="List{CountryResult}" />.</returns>
        private static List<CountryResult> ParseCountriesData(IParentNode document) => CountryResults(document);

        /// <summary>
        /// Save Global Info Summery to Database.
        /// </summary>
        /// <returns>Summery Result <see cref="Task{AllResults}" />.</returns>
        private static async Task<AllResults> ParseGlobalInfoSummery()
        {
            using var document =
                await BrowsingContext.New(Configuration.Default.WithDefaultLoader()).OpenAsync(BaseUrl);
            return GlobalInfoSummery(document);
        }

        /// <summary>
        /// Save Global Info Summery to Database.
        /// </summary>
        /// <param name="document">The document<see cref="IParentNode"/>.</param>
        /// <returns>The <see cref="Task{AllResults}" />.</returns>
        private static AllResults ParseGlobalInfoSummery(IParentNode document) => GlobalInfoSummery(document);

        /// <summary>
        ///  Read Document.
        /// </summary>
        /// <returns>The <see cref="Task{IDocument}"/>.</returns>
        private static async Task<IDocument> ReadDocument() => await BrowsingContext.New(Configuration.Default.WithDefaultLoader()).OpenAsync(BaseUrl);

        /// <summary>
        /// Recurring Task after Some Seconds  .
        /// </summary>
        /// <param name="action">The action<see cref="Action" />.</param>
        /// <param name="seconds">The seconds<see cref="int" />.</param>
        /// <param name="token">The token<see cref="CancellationToken" />.</param>
        /// <returns>Task <see cref="Task" />.</returns>
        private static Task RecurringTask(Action action, int seconds, CancellationToken token) =>
            action == null
                ? Task.CompletedTask
                : Task.Run(async () =>
                {
                    while (!token.IsCancellationRequested)
                    {
                        action();

                        await Task.Delay(TimeSpan.FromSeconds(seconds), token);
                    }
                }, token);

        /// <summary>
        /// The FetchSaveInformationToDatabase.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task FetchSaveInformationToDatabase()
        {

            _dbContext.Database.ExecuteSqlRaw("DELETE FROM Country");
            _dbContext.Database.ExecuteSqlRaw("DELETE FROM GlobalInfo");

            var (all, countries) = await GetAllInfo();
            await _dbContext.All.AddAsync(_mapper.Map<GlobalInfo>(all));

            await _dbContext.Countries.AddRangeAsync(countries.Select(c => _mapper.Map<Countries>(c)));
            await _dbContext.SaveChangesAsync();
            await SendToSignalRClient();
        }

        private async Task SendToSignalRClient()
        {
            // Send information to SignalR Hub Connections
            await _hubContext.Clients.All.SendAsync("getAll", await GetAllData()).ConfigureAwait(false);
            await _hubContext.Clients.All.SendAsync("getCountries", await GetCountriesData()).ConfigureAwait(false);
        }
    }
}