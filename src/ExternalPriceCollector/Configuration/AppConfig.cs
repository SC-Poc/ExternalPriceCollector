namespace ExternalPriceCollector.Configuration
{
    public class AppConfig
    {
        public ApiService ApiService { get; set; }
        public string Name { get; set; }
    }

    public class ApiService
    {
        public string GasAmountReservePercentage { get; set; }
    }
}
