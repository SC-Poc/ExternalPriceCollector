namespace ExternalPriceCollector.Rabbit.Models
{
    public class Quote
    {
        public string source { get; set; }
        public string asset { get; set; }
        public string timestamp { get; set; }
        public decimal bid { get; set; }
        public decimal ask { get; set; }
    }
}
