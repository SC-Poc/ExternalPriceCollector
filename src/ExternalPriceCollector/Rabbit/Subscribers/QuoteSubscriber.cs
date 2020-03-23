using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Common;
using ExternalPriceCollector.Azure;
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

        private IBlobStorage _blobStorage;

        private QuoteMemoryCache<Quote> _quoteMemoryCache;

        private object _sync = new object();

        private readonly ILogger<QuoteSubscriber> _logger;

        private readonly string _container;

        public QuoteSubscriber(QuoteRepository quoteRepository, IBlobStorage blobStorage, QuoteMemoryCache<Quote> quoteMemoryCache, AppConfig config, ILogger<QuoteSubscriber> logger)
        {
            _quoteRepository = quoteRepository;
            _blobStorage = blobStorage;
            _quoteMemoryCache = quoteMemoryCache;
            _config = config;
            _logger = logger;
            _container = DateTime.UtcNow.ToIsoDateTime().Replace(":", "-").Replace(" ", "t");
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

            _blobStorage.CreateContainerIfNotExistsAsync(_container).GetAwaiter().GetResult();
        }

        public void Dispose()
        {
            _subscriber?.Stop();

            _subscriber?.Dispose();
        }

        private async Task ProcessMessageAsync(Quote quote)
        {
            if (quote.Ask == 0 && quote.Bid == 0)
                return;

            quote.SourceId = quote.Source.GetHashCode();
            quote.AssetPairId = quote.Asset.GetHashCode();

            lock (_sync)
            {
                _quoteMemoryCache.Add(quote);

                var count = _quoteMemoryCache.GetCount();

                if (count >= _config.Main.QuoteBatchSize)
                {
                    var quotes = _quoteMemoryCache.GetAllAndClear();

                    if (_config.Main.WriteToDB)
                        WriteToDB(quotes);

                    if (_config.Main.WriteToBlob)
                        WriteToBlob(quotes);
                }
            }
        }

        private void WriteToDB(IEnumerable<Quote> quotes)
        {
            try
            {
                var stopwatch = new Stopwatch();

                stopwatch.Start();

                _quoteRepository.InsertRangeAsync(quotes).GetAwaiter().GetResult();

                stopwatch.Stop();

                _logger.LogInformation("Added to DB {quotesCount} quotes, took {seconds} seconds.", quotes.Count(), stopwatch.Elapsed.TotalSeconds);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong while saving to Postgres.");
            }
        }

        private void WriteToBlob(IEnumerable<Quote> quotes)
        {
            try
            {
                var data = string.Join(Environment.NewLine,
                    quotes.Select(x => $"{x.Source};{x.Asset};{x.Bid};{x.Ask};{x.Timestamp.ToIsoDateTime()}"));

                var stopwatch = new Stopwatch();

                stopwatch.Start();

                _blobStorage.SaveBlobAsync(_container, _container, data.ToStream()).GetAwaiter().GetResult();

                stopwatch.Stop();

                _logger.LogInformation("Added to Blob {quotesCount} quotes, took {seconds} seconds.", quotes.Count(), stopwatch.Elapsed.TotalSeconds);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong while saving to Azure Blob.");
            }
        }
    }
    
}
