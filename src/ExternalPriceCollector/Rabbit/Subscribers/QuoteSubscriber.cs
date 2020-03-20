using System;
using System.Diagnostics;
using System.Threading.Tasks;
using ExternalPriceCollector.Cache;
using ExternalPriceCollector.Configuration;
using ExternalPriceCollector.EntityFramework.Repositories;
using ExternalPriceCollector.Models;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;
using Microsoft.Extensions.Logging;
using Swisschain.LykkeLog.Adapter;

namespace ExternalPriceCollector.Rabbit.Subscribers
{
    public class QuoteSubscriber : Autofac.IStartable, IDisposable
    {
        private AppConfig _config;

        private RabbitMqSubscriber<Quote> _subscriber;

        private QuoteRepository _quoteRepository;

        private QuoteMemoryCache<Quote> _quoteMemoryCache;

        private object _sync = new object();

        private readonly ILogger<QuoteSubscriber> _logger;

        public QuoteSubscriber(QuoteRepository quoteRepository, QuoteMemoryCache<Quote> quoteMemoryCache, AppConfig config, ILogger<QuoteSubscriber> logger)
        {
            _quoteRepository = quoteRepository;
            _quoteMemoryCache = quoteMemoryCache;
            _config = config;
            _logger = logger;
        }

        public void Start()
        {
            var settings = new RabbitMqSubscriptionSettings
            {
                ConnectionString = _config.QuoteSubscriber.ConnectionString,
                ExchangeName = _config.QuoteSubscriber.Exchange,
                QueueName = $"{_config.QuoteSubscriber.Exchange}.{_config.QuoteSubscriber.QueueSuffix}",
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

        private async Task ProcessMessageAsync(Quote quote)
        {
            lock (_sync)
            {
                _quoteMemoryCache.Add(quote);

                var count = _quoteMemoryCache.GetCount();

                if (count >= _config.ApiService.QuoteBatchSize)
                {
                    var quotes = _quoteMemoryCache.GetAllAndClear();

                    var stopwatch = new Stopwatch();

                    stopwatch.Start();

                    _quoteRepository.InsertRangeAsync(quotes).GetAwaiter().GetResult();

                    stopwatch.Stop();

                    _logger.LogInformation("Added to DB {quotesCount} quotes, took {seconds} seconds.", quotes.Count, stopwatch.Elapsed.TotalSeconds);
                }
            }
        }
    }
    
}
