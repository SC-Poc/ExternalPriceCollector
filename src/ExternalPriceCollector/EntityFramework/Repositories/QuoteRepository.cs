using System.Collections.Generic;
using System.Threading.Tasks;
using ExternalPriceCollector.EntityFramework.Context;
using ExternalPriceCollector.Models;
using Microsoft.EntityFrameworkCore;

namespace ExternalPriceCollector.EntityFramework.Repositories
{
    public class QuoteRepository
    {
        private readonly ConnectionFactory _connectionFactory;

        public QuoteRepository(ConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IReadOnlyList<Quote>> GetAllAsync()
        {
            using (var context = _connectionFactory.CreateDataContext())
            {
                var entities = await context.Quotes
                    .ToListAsync();

                return entities;
            }
        }

        public async Task InsertAsync(Quote quote)
        {
            using (var context = _connectionFactory.CreateDataContext())
            {
                context.Quotes.Add(quote);

                await context.SaveChangesAsync();
            }
        }


        public async Task DeleteAsync(Quote quote)
        {
            using (var context = _connectionFactory.CreateDataContext())
            {
                context.Entry(quote).State = EntityState.Deleted;

                await context.SaveChangesAsync();
            }
        }
    }
}
