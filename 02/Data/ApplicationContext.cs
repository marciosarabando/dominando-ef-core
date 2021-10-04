using System;
using System.IO;
using DominandoEntityFramework.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace DominandoEntityFramework.Data
{
    public class ApplicationContext : DbContext
    {
        private readonly StreamWriter _writer = new StreamWriter("meu_log_ef_core.txt", append: true);
        public DbSet<Departamento> Departamentos { get; set; }
        public DbSet<Funcionario> Funcionarios { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            const string strConnection = "server=localhost,1433; database=DEV.IO.ENTITY-2; User ID=SA;Password=Sara@1980docker; pooling=true;";

            optionsBuilder
            //.UseSqlServer(strConnection, p => p.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
            .UseSqlServer(strConnection,
                o => o
                .MaxBatchSize(100)
                .CommandTimeout(5)
                .EnableRetryOnFailure(4, TimeSpan.FromSeconds(10), null)
            ).EnableSensitiveDataLogging()
            //* Log no console
            //.LogTo(Console.WriteLine, new[] { CoreEventId.ContextInitialized, RelationalEventId.CommandExecuted },
            //LogLevel.Information, DbContextLoggerOptions.LocalTime | DbContextLoggerOptions.SingleLine);
            //* Log em um arquivo
            //.LogTo(_writer.WriteLine, LogLevel.Information);
            .LogTo(Console.WriteLine, LogLevel.Information);
            //.EnableDetailedErrors()
            ;

        }

        public override void Dispose()
        {
            base.Dispose();
            _writer.Dispose();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Departamento>().HasQueryFilter(p => !p.Excluido);
        }
    }
}