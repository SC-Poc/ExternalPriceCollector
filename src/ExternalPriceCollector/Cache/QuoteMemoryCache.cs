using System.Collections.Generic;
using ExternalPriceCollector.Models;

namespace ExternalPriceCollector.Cache
{
    public class QuoteMemoryCache
    {
        private object _sync = new object();
        private List<Quote> cache = new List<Quote>();

        public void Add(Quote quote)
        {
            lock (_sync)
            {
                cache.Add(quote);
            }
        }

        public IList<Quote> GetAllAndClear()
        {
            lock(_sync)
            {
                var all = cache;

                cache = new List<Quote>();

                return all;
            }
        }
    }
}
