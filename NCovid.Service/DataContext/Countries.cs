namespace NCovid.Service.DataContext
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Country",Schema = "Config")]
    public class Countries
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        /// <summary>
        ///     Gets or sets the Active.
        /// </summary>

        public int Active { get; set; }

        /// <summary>
        ///     Gets or sets the Cases.
        /// </summary>

        public int Cases { get; set; }

        /// <summary>
        ///     Gets or sets the CasesPerOneMillion.
        /// </summary>
 
        public decimal CasesPerOneMillion { get; set; }

        /// <summary>
        ///     Gets or sets the Country.
        /// </summary>

        public string Country { get; set; }

        /// <summary>
        ///     Gets or sets the Critical.
        /// </summary>

        public int Critical { get; set; }

        /// <summary>
        ///     Gets or sets the Deaths.
        /// </summary>
        public int Deaths { get; set; }

        /// <summary>
        ///     Gets or sets the DeathsPerOneMillion.
        /// </summary>
        public decimal DeathsPerOneMillion { get; set; }

        /// <summary>
        ///     Gets or sets the Recovered.
        /// </summary>

        public int Recovered { get; set; }

        /// <summary>
        ///     Gets or sets the TodayCases.
        /// </summary>
        public int TodayCases { get; set; }

        /// <summary>
        ///     Gets or sets the TodayDeaths.
        /// </summary>
        public int TodayDeaths { get; set; }

        public string FirstCase { get; set; }
    }
}