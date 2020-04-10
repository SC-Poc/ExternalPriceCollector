namespace ExternalPriceCollector.Configuration
{
    public class AppConfig
    {
        public Main Main { get; set; }

        public RabbitMqSubscriber QuoteSubscriber { get; set; }

        public DbSettings DbSettings { get; set; }

        public AzureStorage AzureStorage { get; set; }
    }

    public class Main
    {
        public int QuoteBatchSize { get; set; }

        public bool WriteToDB { get; set; }

        public bool WriteToBlob { get; set; }
    }

    public class RabbitMqSubscriber
    {
        public string ConnectionString { get; set; }

        public string Exchange { get; set; }

        public string ExchangeTrade { get; set; }

        public string QueueSuffix { get; set; }
    }

    public class DbSettings
    {
        public string DataConnectionString { get; set; }
    }

    public class AzureStorage
    {
        public string ConnectionString { get; set; }
    }
}
