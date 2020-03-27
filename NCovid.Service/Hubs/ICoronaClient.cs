namespace NCovid.Service.Hubs
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ICoronaClient
    {
        Task GetCountries(List<CountryResult> countryResults);
        Task GetAll(AllResults all);
    }
}