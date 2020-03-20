using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExternalPriceCollector.Models
{
    [Table("Quotes")]
    public class Quote
    {
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
