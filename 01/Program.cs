using System;
using System.Linq;
using DominandoEntityFramework.Data;
using DominandoEntityFramework.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace DominandoEFCore
{
    class Program
    {
        static void Main(string[] args)
        {
            //EnsureCreatedAndDelete();
            //GapEnsuredCreated();
            //HealthCheckBancoDeDados();

            /*
            new DominandoEntityFramework.Data.ApplicationContext().Departamentos.AsNoTracking().Any();
            _count = 0;
            GerenciarEstadoDaConexao(false);
            _count = 0;
            GerenciarEstadoDaConexao(true);
            */

            //ExecuteSQL();

            //SqlInjection();

            //MigracoesPendentes();

            //AplicarMigracaoEmTempoExecucao();

            //TodasMigracoes();

            //TodasMigracoesAplicadas();

            //ScriptGeralBancoDados();

            //CarregamentoAdiantado();

            //CarregamentoExplicito();

            CarregamentoLazyLoad();
        }

        static void EnsureCreatedAndDelete()
        {
            using var db = new DominandoEntityFramework.Data.ApplicationContext();

            //db.Database.EnsureCreated();

            db.Database.EnsureDeleted();
        }

        static void GapEnsuredCreated()
        {
            using var db1 = new DominandoEntityFramework.Data.ApplicationContext();
            using var db2 = new DominandoEntityFramework.Data.ApplicationContextCidade();


            db1.Database.EnsureCreated();
            db2.Database.EnsureCreated();

            var databaseCreator = db2.GetService<IRelationalDatabaseCreator>();
            databaseCreator.CreateTables();
        }

        static void HealthCheckBancoDeDados()
        {
            using var db = new DominandoEntityFramework.Data.ApplicationContext();

            var canConnect = db.Database.CanConnect();

            if (canConnect)
            {
                Console.WriteLine("Posso me conectar");
            }
            else
            {
                Console.WriteLine("Não posso me conectar");
            }
        }

        static int _count;
        static void GerenciarEstadoDaConexao(bool gerenciarEstadoConexao)
        {
            using var db = new DominandoEntityFramework.Data.ApplicationContext();

            var time = System.Diagnostics.Stopwatch.StartNew();

            var conexao = db.Database.GetDbConnection();

            conexao.StateChange += (_, __) => ++_count;

            if (gerenciarEstadoConexao)
                conexao.Open();

            for (int i = 0; i < 200; i++)
            {
                db.Departamentos.AsNoTracking().Any();
            }

            time.Stop();

            var mensagem = $"TEMPO: {time.Elapsed.ToString()}, {gerenciarEstadoConexao}, Contador: {_count}";

            Console.WriteLine(mensagem);
        }

        static void ExecuteSQL()
        {
            using var db = new DominandoEntityFramework.Data.ApplicationContext();

            //Primeira opção
            using (var cmd = db.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = "SELECT 1";
                cmd.ExecuteNonQuery();
            }

            //Segunda
            var descricao = "TESTE";
            db.Database.ExecuteSqlRaw("update departamentos set descricao={0} where id = 1", descricao);

            //Terceira Opção
            db.Database.ExecuteSqlInterpolated($"update departamentos set descricao={descricao} where id = 1");
        }

        static void SqlInjection()
        {
            using var db = new DominandoEntityFramework.Data.ApplicationContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            db.Departamentos.AddRange(
                new Departamento
                {
                    Descricao = "SERVICO MILITAR"
                },
                new Departamento
                {
                    Descricao = "SFPC"
                }
            );
            db.SaveChanges();

            var descricao = "SERVICO MILITAR";
            db.Database.ExecuteSqlRaw("update departamentos set descricao='SVMIL' where descricao={0}", descricao);

            foreach (var departamento in db.Departamentos.AsNoTracking())
            {
                Console.WriteLine($"Id: {departamento.Id}, Descrição: {departamento.Descricao}");
            }
        }

        static void MigracoesPendentes()
        {
            using var db = new DominandoEntityFramework.Data.ApplicationContext();

            var migracoesPendentes = db.Database.GetPendingMigrations();

            Console.WriteLine($"Total: {migracoesPendentes.Count()}");

            foreach (var migracao in migracoesPendentes)
            {
                Console.WriteLine($"Migracão: {migracao}");
            }
        }

        static void AplicarMigracaoEmTempoExecucao()
        {
            using var db = new DominandoEntityFramework.Data.ApplicationContext();

            db.Database.Migrate();
        }

        static void TodasMigracoes()
        {
            using var db = new DominandoEntityFramework.Data.ApplicationContext();

            var migracoes = db.Database.GetMigrations();

            Console.WriteLine($"Total: {migracoes.Count()}");

            foreach (var item in migracoes)
            {
                Console.WriteLine($"Migração: {item}");
            }
        }

        static void TodasMigracoesAplicadas()
        {
            using var db = new DominandoEntityFramework.Data.ApplicationContext();

            var migracoes = db.Database.GetAppliedMigrations();

            Console.WriteLine($"Total: {migracoes.Count()}");

            foreach (var item in migracoes)
            {
                Console.WriteLine($"Migração: {item}");
            }
        }

        static void ScriptGeralBancoDados()
        {
            using var db = new DominandoEntityFramework.Data.ApplicationContext();
            var script = db.Database.GenerateCreateScript();

            Console.WriteLine(script);
        }

        static void SetupTiposCarregamento(ApplicationContext db)
        {
            if (!db.Departamentos.Any())
            {
                db.Departamentos.AddRange(new Departamento
                {
                    Descricao = "SFPC",
                    Funcionarios = new System.Collections.Generic.List<Funcionario>{
                        new Funcionario{
                            Nome = "Marcio",
                            Cpf = "285.951.348-55",
                            Rg = "326147329"
                        }
                    }
                },
                new Departamento
                {
                    Descricao = "SVMIL",
                    Funcionarios = new System.Collections.Generic.List<Funcionario>{
                        new Funcionario{
                            Nome = "Cassia",
                            Cpf = "987987987987",
                            Rg = "4564654654"
                        },
                        new Funcionario{
                            Nome = "Nogueira",
                            Cpf = "4567894544",
                            Rg = "22222555"
                        }
                    }
                });

                db.SaveChanges();
                db.ChangeTracker.Clear();
            }
        }

        static void CarregamentoAdiantado()
        {
            using var db = new DominandoEntityFramework.Data.ApplicationContext();

            SetupTiposCarregamento(db);

            var departamentos = db.Departamentos
            .Include(p => p.Funcionarios);

            foreach (var departamento in departamentos)
            {
                Console.WriteLine("-----------------------------------");
                Console.WriteLine($"Departamento: {departamento.Descricao}");

                if (departamento.Funcionarios?.Any() ?? false)
                {
                    foreach (var funcionario in departamento.Funcionarios)
                    {
                        Console.WriteLine($"Funcionario: {funcionario.Nome}");
                    }
                }
                else
                {
                    Console.WriteLine($"Nenhum funncionario encontrado");
                }

            }
        }

        static void CarregamentoExplicito()
        {
            using var db = new DominandoEntityFramework.Data.ApplicationContext();

            SetupTiposCarregamento(db);

            var departamentos = db.Departamentos.ToList();


            foreach (var departamento in departamentos)
            {
                if (departamento.Id == 2)
                {
                    //db.Entry(departamento).Collection(x => x.Funcionarios).Load();
                    db.Entry(departamento).Collection(x => x.Funcionarios).Query().Where(f => f.Id > 2).ToList();
                }

                Console.WriteLine("-----------------------------------");
                Console.WriteLine($"Departamento: {departamento.Descricao}");

                if (departamento.Funcionarios?.Any() ?? false)
                {
                    foreach (var funcionario in departamento.Funcionarios)
                    {
                        Console.WriteLine($"Funcionario: {funcionario.Nome}");
                    }
                }
                else
                {
                    Console.WriteLine($"Nenhum funncionario encontrado");
                }

            }
        }

        static void CarregamentoLazyLoadWithProxies()
        {
            using var db = new DominandoEntityFramework.Data.ApplicationContext();

            SetupTiposCarregamento(db);

            //db.ChangeTracker.LazyLoadingEnabled = false;

            var departamentos = db.Departamentos.ToList();


            foreach (var departamento in departamentos)
            {
                Console.WriteLine("-----------------------------------");
                Console.WriteLine($"Departamento: {departamento.Descricao}");

                if (departamento.Funcionarios?.Any() ?? false)
                {
                    foreach (var funcionario in departamento.Funcionarios)
                    {
                        Console.WriteLine($"Funcionario: {funcionario.Nome}");
                    }
                }
                else
                {
                    Console.WriteLine($"Nenhum funncionario encontrado");
                }

            }
        }

        static void CarregamentoLazyLoad()
        {
            using var db = new DominandoEntityFramework.Data.ApplicationContext();

            SetupTiposCarregamento(db);

            //db.ChangeTracker.LazyLoadingEnabled = false;

            var departamentos = db.Departamentos.ToList();


            foreach (var departamento in departamentos)
            {
                Console.WriteLine("-----------------------------------");
                Console.WriteLine($"Departamento: {departamento.Descricao}");

                if (departamento.Funcionarios?.Any() ?? false)
                {
                    foreach (var funcionario in departamento.Funcionarios)
                    {
                        Console.WriteLine($"Funcionario: {funcionario.Nome}");
                    }
                }
                else
                {
                    Console.WriteLine($"Nenhum funncionario encontrado");
                }

            }
        }

    }
}
