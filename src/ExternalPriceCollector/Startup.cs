using Autofac;
using ExternalPriceCollector.Configuration;
using ExternalPriceCollector.GrpcServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Swisschain.Sdk.Server.Common;

namespace ExternalPriceCollector
{
    public sealed class Startup : SwisschainStartup<AppConfig>
    {
        public Startup(IConfiguration configuration) : base(configuration)
        {
        }

        protected override void RegisterEndpoints(IEndpointRouteBuilder endpoints)
        {
            base.RegisterEndpoints(endpoints);

            endpoints.MapGrpcService<MonitoringService>();
        }

        protected override void ConfigureContainerExt(ContainerBuilder builder)
        {
            
        }
    }
}
