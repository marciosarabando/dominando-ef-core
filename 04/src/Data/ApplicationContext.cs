using System;
using Microsoft.EntityFrameworkCore;
using src.Domain;

namespace src.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Pessoa> Pessoas { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            const string strConnection = "server=localhost,1433; database=DEV.IO.ENTITY-3; User ID=SA;Password=Sara@1980docker; pooling=true;";
            //const string strConnectionPG = "Host=localhost; Database=DEVIO04;Username=postgres;Password=123";
            //const string strConnectionSqlite = "Data Source=Dev.IO.db";


            optionsBuilder
                .UseSqlServer(strConnection)
                //.UseNpgsql(strConnectionPG)
                //.UseSqlite(strConnectionSqlite)
                //.UseInMemoryDatabase(databaseName: "Teste-DevIO")
                //.UseCosmos("http://localhost:8081", "key", "dev-io04")
                .LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information)
                .EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Pessoa>(conf =>
            {
                conf.HasKey(p => p.Id);
                conf.Property(p => p.Nome).HasMaxLength(60).IsUnicode(false);
            });
        }
    }
}