using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DominandoEntityFramework.Configurations;
using DominandoEntityFramework.Domain;
using DominandoEntityFramework.Funcoes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.Extensions.Logging;

namespace DominandoEntityFramework.Data
{
    public class ApplicationContext : DbContext
    {
        private readonly StreamWriter _writer = new StreamWriter("meu_log_ef_core.txt", append: true);
        public DbSet<Departamento> Departamentos { get; set; }
        public DbSet<Funcionario> Funcionarios { get; set; }
        public DbSet<Estado> Estados { get; set; }
        public DbSet<Conversor> Conversores { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Ator> Atores { get; set; }
        public DbSet<Filme> Filmes { get; set; }
        public DbSet<Documento> Documentos { get; set; }
        public DbSet<Pessoa> Pessoas { get; set; }
        public DbSet<Instrutor> Instrutores { get; set; }
        public DbSet<Aluno> Alunos { get; set; }
        public DbSet<Dictionary<string, object>> Configuracoes => Set<Dictionary<string, object>>("Configuracoes");
        public DbSet<Atributo> Atributos { get; set; }
        public DbSet<Aeroporto> Aeroportos { get; set; }
        public DbSet<Voo> Voos { get; set; }
        public DbSet<RelatorioFinanceiro> RelatorioFinanceiro { get; set; }
        public DbSet<Funcao> Funcoes { get; set; }
        public DbSet<Livro> Livros { get; set; }

        /*[DbFunction(name: "LEFT", IsBuiltIn = true)]
        public static string Left(string dados, int quantidade) => throw new NotImplementedException();*/


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            const string strConnection = "server=localhost,1433; database=DEV.IO.ENTITY-2; User ID=SA;Password=Sara@1980docker; pooling=true;";

            optionsBuilder
            .UseSqlServer(strConnection)
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTrackingWithIdentityResolution)
            .LogTo(Console.WriteLine, LogLevel.Information)
            .EnableSensitiveDataLogging()
            .AddInterceptors(new Interceptadores.InterceptadorDeComandos())
            .AddInterceptors(new Interceptadores.InterceptadorDeConexao())
            .AddInterceptors(new Interceptadores.InterceptadorPersistencia());
        }

        public override void Dispose()
        {
            base.Dispose();
            _writer.Dispose();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Departamento>().HasQueryFilter(p => !p.Excluido);
            /*modelBuilder.UseCollation("SQL_Latin1_General_CP1_CI_AI");

            modelBuilder.Entity<Departamento>().Property(p => p.Descricao).UseCollation("SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.HasSequence<int>("MinhaSequencia", "sequencias")
                .StartsAt(1)
                .IncrementsBy(2)
                .HasMin(1)
                .HasMax(10)
                .IsCyclic();

            modelBuilder.Entity<Departamento>().Property(p => p.Id).HasDefaultValueSql("NEXT VALUE FOR sequencias.MinhaSequencia");*/
            /*
            modelBuilder
                .Entity<Departamento>()
                .HasIndex(p => new { p.Descricao, p.Ativo })
                .HasDatabaseName("idx_meu_indice_composto")
                .HasFilter("Descricao IS NOT NULL")
                .HasFillFactor(80)
                .IsUnique();
                */

            /*
            modelBuilder.Entity<Estado>().HasData(new[]{
                new Estado {Id = 1, Nome = "São Paulo"},
                new Estado {Id = 2, Nome = "Paraná"}
            });*/

            modelBuilder.HasDefaultSchema("Cadastros");

            modelBuilder.Entity<Estado>().ToTable("Estados", "SegundoEsquema");

            //var conversao = new ValueConverter<Versao, string>(p => p.ToString(), p => (Versao)Enum.Parse(typeof(Versao), p));

            //var conversao1 = new EnumToStringConverter<Versao>();

            /*
            modelBuilder.Entity<Conversor>()
                .Property(p => p.Versao)
                //.HasConversion<string>();
                //.HasConversion(p => p.ToString(), p => (Versao)Enum.Parse(typeof(Versao), p));
                //.HasConversion(conversao);
                //.HasConversion(conversao1);
                .HasConversion(new EnumToStringConverter<Versao>());
            */

            /*
            modelBuilder.Entity<Conversor>()
                .Property(p => p.Status)
                .HasConversion(new ConversorCustomizado());

            modelBuilder.Entity<Departamento>().Property<DateTime>("UltimaAtualizacao");
            */

            //Dessa forma informa uma a uma das configurações das entidadades
            //modelBuilder.ApplyConfiguration(new ClienteConfigurations());

            //Dessa forma o entity aplica todas as configurações que herdam de IEntityTypeConfiguration
            //modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            //ou
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationContext).Assembly);

            modelBuilder.SharedTypeEntity<Dictionary<string, object>>("Configuracoes", b =>
            {
                b.Property<int>("Id");

                b.Property<string>("Chave")
                    .HasColumnType("VARCHAR(40)")
                    .IsRequired();

                b.Property<string>("Valor")
                    .HasColumnType("VARCHAR(255)")
                    .IsRequired();
            });

            modelBuilder.Entity<Funcao>(conf =>
            {
                conf.Property<string>("PropriedadeDeSombra")
                .HasColumnType("VARCHAR(100)")
                .HasDefaultValueSql("'Teste'");
            });

            //MinhasFuncoes.RegistrarFuncoes(modelBuilder);

            modelBuilder.HasDbFunction(_minhaFuncao)
                .HasName("LEFT")
                .IsBuiltIn();

            modelBuilder.HasDbFunction(_letraMaiusculas)
                .HasName("ConverterParaLetrasMaiusculas")
                .HasSchema("dbo");

            modelBuilder.HasDbFunction(_dateDiff)
                .HasName("DATEDIFF")
                .HasTranslation(p =>
                {
                    var argumentos = p.ToList();

                    var constante = (SqlConstantExpression)argumentos[0];
                    argumentos[0] = new SqlFragmentExpression(constante.Value.ToString());

                    return new SqlFunctionExpression("DATEDIFF", argumentos, false, new[] { false, false, false }, typeof(int), null);
                })
                .IsBuiltIn();
        }

        private static MethodInfo _minhaFuncao = typeof(MinhasFuncoes).GetRuntimeMethod("Left", new[] { typeof(string), typeof(int) });
        private static MethodInfo _letraMaiusculas = typeof(MinhasFuncoes).GetRuntimeMethod(nameof(MinhasFuncoes.LetrasMaiusculas), new[] { typeof(string) });
        private static MethodInfo _dateDiff = typeof(MinhasFuncoes).GetRuntimeMethod(nameof(MinhasFuncoes.DateDiff), new[] { typeof(string), typeof(DateTime), typeof(DateTime) });
    }
}