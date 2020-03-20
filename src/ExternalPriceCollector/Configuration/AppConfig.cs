namespace ExternalPriceCollector.Configuration
{
    public class AppConfig
    {
        public ApiService ApiService { get; set; }

        public RabbitMqSubscriber QuoteSubscriber { get; set; }

        public DbSettings DbSettings { get; set; }
    }

    public class ApiService
    {
        public int QuoteBatchSize { get; set; }
    }

    public class RabbitMqSubscriber
    {
        public string ConnectionString { get; set; }

        public string Exchange { get; set; }

        public string QueueSuffix { get; set; }
    }

    public class DbSettings
    {
        public string DataConnectionString { get; set; }
    }
}
