using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace ExternalPriceCollector.Models
{
    [Table("Quotes")]
    public class Quote
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonIgnore]
        public long Id { get; set; }

        [Column("Source", TypeName = "varchar(64)")]
        public string Source { get; set; }

        [Column("Asset", TypeName = "varchar(64)")]
        public string Asset { get; set; }
        
        [Column("Timestamp")]
        public DateTime Timestamp { get; set; }
        
        [Column("Bid")]
        public decimal Bid { get; set; }
        
        [Column("Ask")]
        public decimal Ask { get; set; }
    }
}
