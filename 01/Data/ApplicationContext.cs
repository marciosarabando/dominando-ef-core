using System;
using DominandoEntityFramework.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DominandoEntityFramework.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Departamento> Departamentos { get; set; }
        public DbSet<Funcionario> Funcionarios { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            const string strConnection = "server=localhost,1433; database=DEV.IO.ENTITY; User ID=SA;Password=Sara@1980docker; pooling=true;";

            optionsBuilder
            .UseSqlServer(strConnection)
            .EnableSensitiveDataLogging()
            .UseLazyLoadingProxies()
            .LogTo(Console.WriteLine, LogLevel.Information);
        }
    }
}