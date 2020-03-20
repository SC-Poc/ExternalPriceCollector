using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ExternalPriceCollector.EntityFramework.Context
{
    public class ConnectionFactory
    {
        private readonly string _connectionString;

        public ConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void EnsureMigration()
        {
            using (var context = CreateDataContext())
            {
                context.Database.Migrate();
            }
        }

        internal DataContext CreateDataContext()
        {
            return new DataContext(_connectionString);
        }
    }
}
