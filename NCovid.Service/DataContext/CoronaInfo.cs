namespace NCovid.Service.DataContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("CoronaInfo", Schema = "Config")]
    public class CoronaInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTimeOffset UpdateDate { get; set; }

        public List<Countries> Countries { get; set; }
    }
}