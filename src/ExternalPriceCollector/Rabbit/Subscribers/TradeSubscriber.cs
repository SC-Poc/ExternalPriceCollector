using System;
using System.Diagnostics;
using System.Threading.Tasks;
using ExternalPriceCollector.Cache;
using ExternalPriceCollector.Configuration;
using ExternalPriceCollector.EntityFramework.Context;
using ExternalPriceCollector.Models;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;
using Microsoft.Extensions.Logging;
using Swisschain.LykkeLog.Adapter;

namespace ExternalPriceCollector.Rabbit.Subscribers
{
    public class TradeSubscriber : Autofac.IStartable, IDisposable
    {
        private RabbitMqSubscriber<Trade> _subscriber;
        private AppConfig _config;
        private readonly ConnectionFactory _connectionFactory;
        private readonly ILogger<TradeSubscriber> _logger;
        private QuoteMemoryCache<Trade> _tradeMemoryCache;


        public TradeSubscriber(AppConfig config, EntityFramework.Context.ConnectionFactory connectionFactory,
            ILogger<TradeSubscriber> logger)
        {
            _config = config;
            _connectionFactory = connectionFactory;
            _logger = logger;

            _tradeMemoryCache = new QuoteMemoryCache<Trade>();
        }

        public void Start()
        {
            var settings = new RabbitMqSubscriptionSettings
            {
                ConnectionString = _config.QuoteSubscriber.ConnectionString,
                ExchangeName = _config.QuoteSubscriber.ExchangeTrade,
                QueueName = $"{_config.QuoteSubscriber.ExchangeTrade}.{_config.QuoteSubscriber.QueueSuffix}",
                DeadLetterExchangeName = null,
                IsDurable = false
            };

            _subscriber = new RabbitMqSubscriber<Trade>(LegacyLykkeLogFactoryToConsole.Instance, settings,
                    new ResilientErrorHandlingStrategy(LegacyLykkeLogFactoryToConsole.Instance, settings,
                        TimeSpan.FromSeconds(10)))
                .SetMessageDeserializer(new JsonMessageDeserializer<Trade>())
                .SetMessageReadStrategy(new MessageReadQueueStrategy())
                .Subscribe(ProcessMessageAsync)
                .CreateDefaultBinding()
                .Start();
        }

        private async Task ProcessMessageAsync(Trade arg)
        {
            _tradeMemoryCache.Add(arg);

            if (_tradeMemoryCache.GetCount() >= 500)
            {
                var sw = new Stopwatch();
                sw.Start();
                var list = _tradeMemoryCache.GetAllAndClear();
                using (var context = _connectionFactory.CreateDataContext())
                {
                    context.Trades.AddRange(list);
                    await context.SaveChangesAsync();
                }
                sw.Stop();
                _logger.LogInformation("Added to DB.Trades {tradeCount} trades, took {seconds} seconds.", list.Count, sw.Elapsed.TotalSeconds);
            }
        }

        public void Dispose()
        {
            _subscriber?.Stop();
            _subscriber?.Dispose();
        }
    }
}
