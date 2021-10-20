using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using src.Domain;
using src.Extensions;

namespace src.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Colaborador> Colaboradores { get; set; }
        public DbSet<Departamento> Departamentos { get; set; }
        public DbSet<UsuarioFuncao> UsuarioFuncoes { get; set; }
        public DbSet<DepartarmentoRelatorio> DepartarmentoRelatorios { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("server=localhost,1433; database=DEV.IO.DICAS; User ID=SA;Password=Sara@1980docker; pooling=true;")
            .LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information)
            .EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<UsuarioFuncao>().HasNoKey();

            modelBuilder.Entity<DepartarmentoRelatorio>(e =>
            {
                e.HasNoKey();

                e.ToView("vw_departamento_relatorio");

                e.Property(p => p.Departamento).HasColumnName("Descricao");
            });

            //Garante que as colunas string das tabelas sejam criadas como varchar e não como nvarchar
            var properties = modelBuilder.Model.GetEntityTypes()
                .SelectMany(p => p.GetProperties())
                .Where(p => p.ClrType == typeof(string)
                    && p.GetColumnType() == null);

            foreach (var prop in properties)
            {
                prop.SetIsUnicode(false);
            }

            modelBuilder.ToSnakeCaseNames();
        }
    }
}