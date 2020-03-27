namespace NCovid.Service.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="ICoronaVirusService" />.
    /// </summary>
    public interface ICoronaVirusService
    {
        /// <summary>
        /// The GetAllData.
        /// </summary>
        /// <returns>The <see cref="Task{AllResults}"/>.</returns>
        Task<AllResults> GetAllData();

        /// <summary>
        /// The GetCountriesData.
        /// </summary>
        /// <returns>The <see cref="Task{System.Collections.Generic.List{CountryResult}}"/>.</returns>
        Task<System.Collections.Generic.List<CountryResult>> GetCountriesData();
        Task<List<CountryResult>> GetCountriesData(string search);
        void SaveInfo();
    }
}
