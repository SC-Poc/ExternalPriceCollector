using ExternalPriceCollector.Client.Common;
using ExternalPriceCollector.Protos;

namespace ExternalPriceCollector.Client
{
    public class ExternalPriceCollectorClient : BaseGrpcClient, IExternalPriceCollectorClient
    {
        public ExternalPriceCollectorClient(string serverGrpcUrl) : base(serverGrpcUrl)
        {
            Monitoring = new Monitoring.MonitoringClient(Channel);
        }

        public Monitoring.MonitoringClient Monitoring { get; }
    }
}
