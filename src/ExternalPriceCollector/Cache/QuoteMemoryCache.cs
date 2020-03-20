using System.Collections.Generic;

namespace ExternalPriceCollector.Cache
{
    public class QuoteMemoryCache<T>
    {
        private object _sync = new object();
        private List<T> cache = new List<T>();

        public void Add(T obj)
        {
            lock (_sync)
            {
                cache.Add(obj);
            }
        }

        public IList<T> GetAllAndClear()
        {
            lock(_sync)
            {
                var all = cache;

                cache = new List<T>();

                return all;
            }
        }

        public int GetCount()
        {
            lock (_sync)
            {
                return cache.Count;
            }
        }
    }
}
