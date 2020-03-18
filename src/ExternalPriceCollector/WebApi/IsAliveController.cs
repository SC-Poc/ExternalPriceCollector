using System.Collections.Generic;
using ExternalPriceCollector.Configuration;
using ExternalPriceCollector.WebApi.Models.IsAlive;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swisschain.Sdk.Server.Common;

namespace ExternalPriceCollector.WebApi
{
    [ApiController]
    [Route("api/[controller]")]
    public class IsAliveController : ControllerBase
    {
        private readonly ILogger<IsAliveController> _logger;
        private readonly AppConfig _config;

        public IsAliveController(ILogger<IsAliveController> logger, AppConfig config)
        {
            _logger = logger;
            _config = config;
        }

        [HttpGet]
        public IsAliveResponse Get()
        {
            _logger.LogInformation("Hello {who}! How are {name}?", "world", "alex");
            _logger.LogInformation("config: {conf}", _config.ApiService.GasAmountReservePercentage);
            
            _logger.LogInformation("data: {$data}", new {Name="alex", Number=54});
            _logger.LogInformation("data: {@data}", new { Name = "alex", Number = 54 });

            var response = new IsAliveResponse
            {
                Name = ApplicationInformation.AppName,
                Version = ApplicationInformation.AppVersion,
                StartedAt = ApplicationInformation.StartedAt,
                Env = ApplicationEnvironment.Environment,
                HostName = ApplicationEnvironment.HostName,
                StateIndicators = new List<IsAliveResponse.StateIndicator>()
            };

            return response;
        }
    }
}
