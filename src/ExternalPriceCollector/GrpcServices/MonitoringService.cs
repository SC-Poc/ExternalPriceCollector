using System.Threading.Tasks;
using ExternalPriceCollector.Configuration;
using ExternalPriceCollector.Protos;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Swisschain.Sdk.Server.Common;

namespace ExternalPriceCollector.GrpcServices
{
    public class MonitoringService : Monitoring.MonitoringBase
    {
        private readonly ILogger<MonitoringService> _logger;
        private readonly AppConfig _config;

        public MonitoringService(ILogger<MonitoringService> logger, AppConfig config)
        {
            _logger = logger;
            _config = config;
        }

        public override Task<IsAliveResponce> IsAlive(IsAliveRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Hello {who}! How are {name}?", "world", "alex");
            _logger.LogInformation("config: {conf}", _config.ApiService.GasAmountReservePercentage);

            var result = new IsAliveResponce
            {
                Name = ApplicationInformation.AppName,
                Version = ApplicationInformation.AppVersion,
                StartedAt = ApplicationInformation.StartedAt.ToString("yyyy-MM-dd HH:mm:ss")
            };

            return Task.FromResult(result);
        }
    }
}
