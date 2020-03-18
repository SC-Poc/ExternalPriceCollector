using ExternalPriceCollector.Protos;

namespace ExternalPriceCollector.Client
{
    public interface IExternalPriceCollectorClient
    {
        Monitoring.MonitoringClient Monitoring { get; }
    }
}
