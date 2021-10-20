using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using src.Data;
using src.Domain;

namespace DicasTruques
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");
            //ToQueryString();
            //DebugView();
            //Clear();
            //ConsultaFiltrada();
            //SingleOrDefaultVsFirstOrDefault();
            //SemChavePrimaria();
            //ToView();
            //NaoUniCode_SnakeCase();
            //OperadoresAgregacao();
            //OperadoresAgregacaoNoAgrupamento();
            ContadorDeEventos();
        }

        static void ToQueryString()
        {
            var db = new ApplicationContext();
            db.Database.EnsureCreated();

            var query = db.Departamentos.Where(x => x.Id > 2);

            var sql = query.ToQueryString();

            Console.WriteLine(sql);
        }

        static void DebugView()
        {
            var db = new ApplicationContext();

            db.Departamentos.Add(new src.Domain.Departamento
            {
                Descricao = "Teste Debug View"
            });

            var query = db.Departamentos.Where(x => x.Id > 2);
        }

        static void Clear()
        {
            var db = new ApplicationContext();

            db.Departamentos.Add(new src.Domain.Departamento
            {
                Descricao = "Teste Debug View"
            });

            db.ChangeTracker.Clear();
        }
        static void ConsultaFiltrada()
        {
            var db = new ApplicationContext();

            var sql = db.Departamentos.Include(p => p.Colaboradores.Where(c => c.Nome == "Teste"));

            Console.WriteLine(sql.ToQueryString());
        }

        static void SingleOrDefaultVsFirstOrDefault()
        {
            var db = new ApplicationContext();

            Console.WriteLine("SingleOrDefault");

            _ = db.Departamentos.SingleOrDefault(p => p.Id > 2);

            Console.WriteLine("FirstOrDefault");

            _ = db.Departamentos.FirstOrDefault(p => p.Id > 2);
        }

        static void SemChavePrimaria()
        {
            using var db = new ApplicationContext();

            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            var usuarioFuncoes = db.UsuarioFuncoes.Where(p => p.UsuarioId == Guid.NewGuid()).ToArray();
        }

        static void ToView()
        {
            using var db = new ApplicationContext();

            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            db.Database.ExecuteSqlRaw(@"
                CREATE VIEW vw_departamento_relatorio AS
                    SELECT 
                        d.Descricao, count(c.Id) as Colaboradores
                    FROM Departamentos d
                    LEFT JOIN Colaboradores c ON c.DepartamentoId=d.Id
                    GROUP BY d.Descricao
            ");

            var departamentos = Enumerable.Range(1, 10).Select(p => new Departamento
            {
                Descricao = $"Departamento {p}",
                Colaboradores = Enumerable.Range(1, p).Select(c => new Colaborador
                {
                    Nome = $"Colaborador {p}-{c}"
                }).ToList()
            });

            var departamento = new Departamento
            {
                Descricao = "Departamento sem colaborador"
            };

            db.Departamentos.Add(departamento);
            db.Departamentos.AddRange(departamentos);
            db.SaveChanges();

            var relatorio = db.DepartarmentoRelatorios
                .Where(p => p.Colaboradores < 20)
                .OrderBy(p => p.Departamento)
                .ToList();

            foreach (var dep in relatorio)
            {
                Console.WriteLine($"{dep.Departamento} [Colaboradores: {dep.Colaboradores}]");
            }
        }

        static void NaoUniCode_SnakeCase()
        {
            using var db = new ApplicationContext();

            var sql = db.Database.GenerateCreateScript();

            Console.WriteLine(sql);
        }

        static void OperadoresAgregacao()
        {
            using var db = new ApplicationContext();

            var sql = db
                .Departamentos
                .GroupBy(p => p.Descricao)
                .Select(p => new
                {
                    Descricao = p.Key,
                    Contador = p.Count(),
                    Media = p.Average(p => p.Id),
                    Maximo = p.Max(p => p.Id),
                    Soma = p.Sum(p => p.Id),
                }).ToQueryString();


            Console.WriteLine(sql);

        }

        static void OperadoresAgregacaoNoAgrupamento()
        {
            using var db = new ApplicationContext();

            var sql = db
                .Departamentos
                .GroupBy(p => p.Descricao)
                .Where(x => x.Count() > 1)
                .Select(p => new
                {
                    Descricao = p.Key,
                    Contador = p.Count()
                }).ToQueryString();


            Console.WriteLine(sql);
        }

        static void ContadorDeEventos()
        {
            using var db = new ApplicationContext();

            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            Console.WriteLine($" PID: {System.Diagnostics.Process.GetCurrentProcess().Id}");

            while (Console.ReadKey().Key != ConsoleKey.Escape)
            {
                var departamento = new Departamento
                {
                    Descricao = $"Departamento sem colaborador"
                };

                db.Departamentos.Add(departamento);
                db.SaveChanges();

                _ = db.Departamentos.Find(1);
                _ = db.Departamentos.AsNoTracking().FirstOrDefault();
            }
        }
    }
}
