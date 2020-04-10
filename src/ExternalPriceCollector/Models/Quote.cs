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
        [Column("Id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonIgnore]
        public long Id { get; set; }

        [NotMapped]
        public string Source { get; set; }

        [Column("SourceId")]
        [JsonIgnore]
        public int SourceId { get ; set; }

        [NotMapped]
        public string Asset { get; set; }

        [Column("AssetPairId")]
        [JsonIgnore]
        public int AssetPairId { get; set; }

        [Column("Timestamp")]
        public DateTime Timestamp { get; set; }
        
        [Column("Bid")]
        public decimal Bid { get; set; }
        
        [Column("Ask")]
        public decimal Ask { get; set; }
    }



    // {"exchange":"HitBTC","quote":"BTC","base":"ZRX","tradeId":819426505,"unix":1586503025922,"side":"buy","price":"0.000024665","amount":"584.9"}
}
