namespace NCovid.Service
{
    using System.Text.Json.Serialization;

    /// <summary>
    ///     Defines the <see cref="CountryResult" />.
    /// </summary>
    public class CountryResult
    {
        /// <summary>
        ///     Gets or sets the Active.
        /// </summary>
        [JsonPropertyName("active")]
        public int Active { get; set;}

        /// <summary>
        ///     Gets or sets the Cases.
        /// </summary>
        [JsonPropertyName("cases")]
        public int Cases { get; set; }

        /// <summary>
        ///     Gets or sets the CasesPerOneMillion.
        /// </summary>
        [JsonPropertyName("casesPerOneMillion")]
        public decimal CasesPerOneMillion { get; set;}

        /// <summary>
        ///     Gets or sets the Country.
        /// </summary>
        [JsonPropertyName("country")]
        public string Country { get; set;}

        /// <summary>
        ///     Gets or sets the Critical.
        /// </summary>
        [JsonPropertyName("critical")]
        public int Critical { get; set;}

        /// <summary>
        ///     Gets or sets the Deaths.
        /// </summary>
        [JsonPropertyName("deaths")]
        public int Deaths { get; set;}

        /// <summary>
        ///     Gets or sets the DeathsPerOneMillion.
        /// </summary>
        [JsonPropertyName("deathsPerOneMillion")]
        public decimal DeathsPerOneMillion { get; set;}

        /// <summary>
        ///     Gets or sets the Recovered.
        /// </summary>
        [JsonPropertyName("recovered")]
        public int Recovered { get; set;}

        /// <summary>
        ///     Gets or sets the TodayCases.
        /// </summary>
        [JsonPropertyName("todayCases")]
        public int TodayCases { get; set;}

        /// <summary>
        ///     Gets or sets the TodayDeaths.
        /// </summary>
        [JsonPropertyName("todayDeaths")]
        public int TodayDeaths { get; set;}
    }
}