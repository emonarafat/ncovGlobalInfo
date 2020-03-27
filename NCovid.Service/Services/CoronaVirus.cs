namespace NCovid.Service.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using AngleSharp;
    using AngleSharp.Dom;
    using DataContext;
    using Hubs;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.EntityFrameworkCore;

    public class CoronaVirusService : ICoronaVirusService
    {
        private const string BaseUrl = "https://www.worldometers.info/coronavirus/";

        private const string MainCounterSelector = ".maincounter-number";
        // private readonly CoronaDbContext _dbContext;
        private readonly IHubContext<CoronaHub> _hubContext;

        public CoronaVirusService(IHubContext<CoronaHub> hubContext)
        {
            _hubContext = hubContext;
        }

        //public CoronaVirusService()
        //{
        //   // _dbContext = dbContext;
        //    //_hubContext = hubContext;
        //}

        public async Task<AllResults> GetAllData()
        {
            await using CoronaDbContext dbContext = new CoronaDbContext();
            return await dbContext.All.Select(a => new AllResults
            {
                Cases = a.Cases,
                Deaths = a.Deaths,
                Recovered = a.Recovered
            }).FirstOrDefaultAsync();
        }

        public async Task<List<CountryResult>> GetCountriesData()
        {
            await using CoronaDbContext dbContext = new CoronaDbContext();
            return await dbContext.Countries.Select(c => new CountryResult
            {
                Active = c.Active,
                Cases = c.Cases,
                Deaths = c.Deaths,
                Recovered = c.Recovered,
                CasesPerOneMillion = c.CasesPerOneMillion,
                Country = c.Country,
                Critical = c.Critical,
                DeathsPerOneMillion = c.DeathsPerOneMillion,
                TodayCases = c.TodayCases,
                TodayDeaths = c.TodayDeaths
            }).ToListAsync();
        }
        public async Task<List<CountryResult>> GetCountriesData(string search)
        {
            await using CoronaDbContext dbContext = new CoronaDbContext();
            return await dbContext.Countries.Where(c=>c.Country.Contains(search)).Select(c => new CountryResult
            {
                Active = c.Active,
                Cases = c.Cases,
                Deaths = c.Deaths,
                Recovered = c.Recovered,
                CasesPerOneMillion = c.CasesPerOneMillion,
                Country = c.Country,
                Critical = c.Critical,
                DeathsPerOneMillion = c.DeathsPerOneMillion,
                TodayCases = c.TodayCases,
                TodayDeaths = c.TodayDeaths
            }).OrderBy(o=>o.Cases).ToListAsync();
        }
        public async void SaveInfo()
        {

            await RecurringTask(async () =>
            {
                await using CoronaDbContext dbContext = new CoronaDbContext();
                dbContext.Database.ExecuteSqlRaw("DELETE FROM Country");
                dbContext.Database.ExecuteSqlRaw("DELETE FROM GlobalInfo");
                var allData = await SaveAll();

                await dbContext.All.AddAsync(new GlobalInfo
                {
                    Cases = allData.Cases,
                    Deaths = allData.Deaths,
                    Recovered = allData.Recovered
                });

                await dbContext.Countries.AddRangeAsync((await SaveCountriesData()).Select(c => new Countries
                {
                    Active = c.Active,
                    Cases = c.Cases,
                    Deaths = c.Deaths,
                    Recovered = c.Recovered,
                    CasesPerOneMillion = c.CasesPerOneMillion,
                    Country = c.Country,
                    Critical = c.Critical,
                    DeathsPerOneMillion = c.DeathsPerOneMillion,
                    TodayCases = c.TodayCases,
                    TodayDeaths = c.TodayDeaths
                }));
                await dbContext.SaveChangesAsync();
                var all = await GetAllData();
                await _hubContext.Clients.All.SendAsync("getAll", all).ConfigureAwait(false);
                var countries = await GetCountriesData();
                await _hubContext.Clients.All.SendAsync("getCountries", countries).ConfigureAwait(false);
            }, 120, CancellationToken.None);
        }

        private static async Task<AllResults> SaveAll()
        {
            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(BaseUrl);
            var all = document.QuerySelectorAll(MainCounterSelector);
            var result = new AllResults
            {
                Cases = 0,
                Deaths = 0,
                Recovered = 0
            };
            for (var i = 0; i < all.Length; i++)
            {
                var el = all[i];
                var data = el.QuerySelector("span").TextContent?.Trim() ?? "0";
                int.TryParse(data.Replace(",", ""), out var count);
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

        private static async Task<List<CountryResult>> SaveCountriesData()
        {
            var result = new List<CountryResult>();
            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(BaseUrl);
            var countriesTableCells = document.QuerySelectorAll("table#main_table_countries_today tbody tr");
            //var totalColumns = 10;
            var countryColIndex = 0;
            var casesColIndex = 1;
            var todayCasesColIndex = 2;
            var deathsColIndex = 3;
            var todayDeathsColIndex = 4;
            var curedColIndex = 5;
            var activeColIndex = 6;
            var criticalColIndex = 7;
            var casesPerOneMillionColIndex = 8;
            var deathsPerOneMillionColIndex = 9;
            result.AddRange(from row in countriesTableCells
                let cells = row.QuerySelectorAll("td")
                let country = GetCountry(countryColIndex, cells)
                where country != "Total:"
                select new CountryResult
                {
                    Country = country,
                    Cases = cells[casesColIndex].TextContent.ToInt(),
                    Active = cells[activeColIndex].TextContent.ToInt(),
                    CasesPerOneMillion = cells[casesPerOneMillionColIndex].TextContent.ToDecimal(),
                    Critical = cells[criticalColIndex].TextContent.ToInt(),
                    Deaths = cells[deathsColIndex].TextContent.ToInt(),
                    DeathsPerOneMillion = cells[deathsPerOneMillionColIndex].TextContent.ToDecimal(),
                    Recovered = cells[curedColIndex].TextContent.ToInt(),
                    TodayCases = cells[todayCasesColIndex].TextContent.ToInt(),
                    TodayDeaths = cells[todayDeathsColIndex].TextContent.ToInt()
                });

            return result;
        }

        private static string GetCountry(int countryColIndex, IHtmlCollection<IElement> cells)
        {
            var country = cells[countryColIndex].TextContent;
            return country.IsSet() ? country : cells[countryColIndex].QuerySelector("a")?.TextContent.Trim() ?? "";
        }

        private  Task RecurringTask(Action action, int seconds, CancellationToken token)
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
    }
}