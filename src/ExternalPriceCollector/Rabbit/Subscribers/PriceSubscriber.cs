using System;
using System.Threading.Tasks;
using ExternalPriceCollector.Configuration;
using ExternalPriceCollector.Rabbit.Models;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;
using Microsoft.Extensions.Logging;
using Swisschain.LykkeLog.Adapter;

namespace ExternalPriceCollector.Rabbit.Subscribers
{
    public class PriceSubscriber : Autofac.IStartable, IDisposable
    {
        private readonly ILoggerFactory _loggerFactory;

        private AppConfig _config;

        private RabbitMqSubscriber<Quote> _subscriber;

        public PriceSubscriber(ILoggerFactory loggerFactory, AppConfig config)
        {
            _loggerFactory = loggerFactory;
            _config = config;
        }

        public void Start()
        {
            var settings = new RabbitMqSubscriptionSettings
            {
                ConnectionString = _config.QuoteSubscriber.ConnectionString,
                ExchangeName = _config.QuoteSubscriber.Exchange,
                QueueName = $"{_config.QuoteSubscriber.Exchange}.{_config.QuoteSubscriber.QuerySuffix}",
                DeadLetterExchangeName = null,
                IsDurable = false
            };

            _subscriber = new RabbitMqSubscriber<Quote>(LegacyLykkeLogFactoryToConsole.Instance, settings,
                    new ResilientErrorHandlingStrategy(LegacyLykkeLogFactoryToConsole.Instance, settings,
                    TimeSpan.FromSeconds(10)))
                .SetMessageDeserializer(new JsonMessageDeserializer<Quote>())
                .SetMessageReadStrategy(new MessageReadQueueStrategy())
                .Subscribe(ProcessMessageAsync)
                .CreateDefaultBinding()
                .Start();
        }

        public void Dispose()
        {
            _subscriber?.Stop();

            _subscriber?.Dispose();
        }

        private Task ProcessMessageAsync(Quote message)
        {
            return Task.CompletedTask;
        }
    }
    
}
