using ExternalPriceCollector.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ExternalPriceCollector.EntityFramework.Context
{
    public class DataContext : DbContext
    {
        private const string Schema = "prices";

        private string _connectionString;

        public DataContext()
        {
        }

        public DataContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        internal DbSet<Quote> Quotes { get; set; }

        internal DbSet<Trade> Trades { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_connectionString == null)
            {
                System.Console.Write("Enter connection string: ");
                _connectionString = System.Console.ReadLine();
            }

            optionsBuilder.UseNpgsql(_connectionString,
                o => o.MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schema));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(Schema);
            modelBuilder.UseIdentityColumns();
        }
    }
}
