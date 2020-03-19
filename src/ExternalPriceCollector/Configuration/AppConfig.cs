namespace ExternalPriceCollector.Configuration
{
    public class AppConfig
    {
        public ApiService ApiService { get; set; }

        public RabbitMqSubscriber QuoteSubscriber { get; set; }
    }

    public class ApiService
    {
        public string GasAmountReservePercentage { get; set; }
    }

    public class RabbitMqSubscriber
    {
        public string ConnectionString { get; set; }

        public string Exchange { get; set; }

        public string QuerySuffix { get; set; }
    }
}
