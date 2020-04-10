using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace ExternalPriceCollector.Models
{
    [Table("Trades")]
    public class Trade
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonIgnore]
        public long Id { get; set; }

        [Column("exchange")]
        public string Exchange { get; set; }

        [Column("quote")]
        public string Quote { get; set; }

        [Column("base")]
        public string Base { get; set; }

        [Column("tradeId")]
        public long TradeId { get; set; }

        [Column("unix")]
        public long Unix { get; set; }

        [Column("side")]
        public string Side { get; set; }

        [Column("price")]
        public decimal Price { get; set; }

        [Column("amount")]
        public decimal Amount { get; set; }

        [Column("ts")]
        [JsonIgnore]
        public DateTime Ts { get; set; } = DateTime.UtcNow;
    }
}
