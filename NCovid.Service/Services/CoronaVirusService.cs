using IMapper = AutoMapper.IMapper;

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



    /// <summary>
    /// Defines the <see cref="CoronaVirusService" />.
    /// </summary>
    public class CoronaVirusService : ICoronaVirusService
    {
        /// <summary>
        /// Defines the BaseUrl.
        /// </summary>
        private const string BaseUrl = "https://www.worldometers.info/coronavirus/";

        /// <summary>
        /// Defines the MainCounterSelector.
        /// </summary>
        private const string MainCounterSelector = ".maincounter-number";

        // private readonly CoronaDbContext _dbContext;
        /// <summary>
        /// Defines the _hubContext.
        /// </summary>
        private readonly IHubContext<CoronaHub> _hubContext;

        /// <summary>
        /// Defines the _mapper.
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="CoronaVirusService"/> class.
        /// </summary>
        /// <param name="hubContext">The hubContext<see cref="IHubContext{CoronaHub}"/>.</param>
        /// <param name="mapper">The mapper<see cref="IMapper"/>.</param>
        public CoronaVirusService(IHubContext<CoronaHub> hubContext, IMapper mapper)
        {
            _hubContext = hubContext;
            _mapper = mapper;
            SaveInfo();
        }


        /// <summary>
        /// Get Global Info Summery
        /// </summary>
        /// <returns>Gl;global Info Summery <see cref="Task{AllResults}"/>.</returns>
        public async Task<AllResults> GetAllData()
        {
            await using CoronaDbContext dbContext = new CoronaDbContext();
            //return await dbContext.All.Select(a => new AllResults
            //{
            //    Cases = a.Cases,
            //    Deaths = a.Deaths,
            //    Recovered = a.Recovered
            //}).FirstOrDefaultAsync();
            return _mapper.Map<AllResults>(await dbContext.All.FirstOrDefaultAsync());
        }

        /// <summary>
        ///  Get Countries Data.
        /// </summary>
        /// <returns>List of Countries <see cref="List{CountryResult}"/>.</returns>
        public async Task<List<CountryResult>> GetCountriesData()
        {
            await using CoronaDbContext dbContext = new CoronaDbContext();
            //return await dbContext.Countries.Select(c => new CountryResult
            //{
            //    Active = c.Active,
            //    Cases = c.Cases,
            //    Deaths = c.Deaths,
            //    Recovered = c.Recovered,
            //    CasesPerOneMillion = c.CasesPerOneMillion,
            //    Country = c.Country,
            //    Critical = c.Critical,
            //    DeathsPerOneMillion = c.DeathsPerOneMillion,
            //    TodayCases = c.TodayCases,
            //    TodayDeaths = c.TodayDeaths
            //}).ToListAsync();
            return (await dbContext.Countries.AsNoTracking().ToListAsync()).Select(c => _mapper.Map<CountryResult>(c)).ToList();
        }

        /// <summary>
        ///  Get Countries Data by Searched Criteria
        /// </summary>
        /// <param name="search">The search<see cref="string"/>.</param>
        /// <returns>List of Countries <see cref="List{CountryResult}"/>.</returns>
        public async Task<List<CountryResult>> GetCountriesData(string search)
        {
            await using CoronaDbContext dbContext = new CoronaDbContext();
            //return await dbContext.Countries.Where(c=>c.Country.Contains(search)).Select(c => new CountryResult
            //{
            //    Active = c.Active,
            //    Cases = c.Cases,
            //    Deaths = c.Deaths,
            //    Recovered = c.Recovered,
            //    CasesPerOneMillion = c.CasesPerOneMillion,
            //    Country = c.Country,
            //    Critical = c.Critical,
            //    DeathsPerOneMillion = c.DeathsPerOneMillion,
            //    TodayCases = c.TodayCases,
            //    TodayDeaths = c.TodayDeaths
            //}).OrderBy(o=>o.Cases).ToListAsync();
            return (await dbContext.Countries.Where(c => c.Country.Contains(search)).OrderBy(o => o.Cases).ToListAsync()).Select(c => _mapper.Map<CountryResult>(c)).ToList(); 
        }

        /// <summary>
        /// Save Info to Database to avoid Calling worldometers and get banned
        /// </summary>
        public async void SaveInfo()
        {

            await RecurringTask(async () =>
            {
                await using var dbContext = new CoronaDbContext();
                dbContext.Database.ExecuteSqlRaw("DELETE FROM Country");
                dbContext.Database.ExecuteSqlRaw("DELETE FROM GlobalInfo");
                var allData = await SaveGlobalInfoSummery();

                //await dbContext.All.AddAsync(new GlobalInfo
                //{
                //    Cases = allData.Cases,
                //    Deaths = allData.Deaths,
                //    Recovered = allData.Recovered
                //});
                await dbContext.All.AddAsync(_mapper.Map<GlobalInfo>(allData));
                //await dbContext.Countries.AddRangeAsync((await SaveCountriesData()).Select(c => new Countries
                //{
                //    Active = c.Active,
                //    Cases = c.Cases,
                //    Deaths = c.Deaths,
                //    Recovered = c.Recovered,
                //    CasesPerOneMillion = c.CasesPerOneMillion,
                //    Country = c.Country,
                //    Critical = c.Critical,
                //    DeathsPerOneMillion = c.DeathsPerOneMillion,
                //    TodayCases = c.TodayCases,
                //    TodayDeaths = c.TodayDeaths,
                //    FirstCase = c.FirstCase
                //}));
                await dbContext.Countries.AddRangeAsync((await SaveCountriesData()).Select(c => _mapper.Map<Countries>(c)));
                await dbContext.SaveChangesAsync();
                await _hubContext.Clients.All.SendAsync("getAll", await GetAllData()).ConfigureAwait(false);
                await _hubContext.Clients.All.SendAsync("getCountries", await GetCountriesData()).ConfigureAwait(false);
            }, 120, CancellationToken.None);
        }

        /// <summary>
        ///Parse Country from a link or span
        /// </summary>
        /// <param name="countryColIndex">The countryColIndex<see cref="int"/>.</param>
        /// <param name="cells">The cells<see cref="IHtmlCollection{IElement}"/>.</param>
        /// <returns>Country Name <see cref="string"/>.</returns>
        private static string GetCountry(int countryColIndex, IHtmlCollection<IElement> cells)
        {
            var country = cells[countryColIndex].TextContent;
            return country.IsSet() ? country : cells[countryColIndex].QuerySelector("a")?.TextContent.Trim() ?? "";
        }

        /// <summary>
        /// Recurring Task
        /// </summary>
        /// <param name="action">The action<see cref="Action"/>.</param>
        /// <param name="seconds">The seconds<see cref="int"/>.</param>
        /// <param name="token">The token<see cref="CancellationToken"/>.</param>
        /// <returns>Task <see cref="Task"/>.</returns>
        private static Task RecurringTask(Action action, int seconds, CancellationToken token)
        {
            if (action == null)
                return Task.CompletedTask;
            return Task.Run(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    action();

                    await Task.Delay(TimeSpan.FromSeconds(seconds), token);

                }
            }, token);
        }

        /// <summary>
        /// Save Global Info Summery to Database
        /// </summary>
        /// <returns>The <see cref="Task{AllResults}"/>.</returns>
        private static async Task<AllResults> SaveGlobalInfoSummery()
        {
            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(BaseUrl);
            var selectorAll = document.QuerySelectorAll(MainCounterSelector);
            var result = new AllResults();
            for (var i = 0; i < selectorAll.Length; i++)
            {
                int.TryParse((selectorAll[i].QuerySelector("span").TextContent?.Trim() ?? "0").Replace(",", ""), out var count);
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
        /// The SaveCountriesData.
        /// </summary>
        /// <returns>The <see cref="List{CountryResult}"/>.</returns>
        private static async Task<List<CountryResult>> SaveCountriesData()
        {
            var result = new List<CountryResult>();
            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(BaseUrl);
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
                .Select(t => new { t, country = GetCountry(countryColIndex, t.cells) })
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
    }
}
