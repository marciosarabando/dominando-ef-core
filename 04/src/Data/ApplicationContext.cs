using System;
using Microsoft.EntityFrameworkCore;
using src.Domain;

namespace src.Data
{
    public class ApplicationContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            const string strConnection = "server=localhost,1433; database=DEV.IO.ENTITY-3; User ID=SA;Password=Sara@1980docker; pooling=true;";

            optionsBuilder
                .UseSqlServer(strConnection)
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