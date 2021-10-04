using System;
using System.Linq;
using DominandoEntityFramework.Data;
using DominandoEntityFramework.Domain;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace DominandoEntityFramework.FuncoesDidaticas
{
    public static class FuncoesDidaticas
    {
        public static void FiltroGlobal()
        {
            using var db = new ApplicationContext();
            Setup(db);

            var departamentos = db.Departamentos.Where(p => p.Id > 0).ToList();

            foreach (var departamento in departamentos)
            {
                Console.WriteLine($"Descrição: {departamento.Descricao} \t Excluído: {departamento.Excluido}");
            }
        }

        public static void IgnoreFiltroGlobal()
        {
            using var db = new ApplicationContext();
            Setup(db);

            var departamentos = db.Departamentos.IgnoreQueryFilters().Where(p => p.Id > 0).ToList();

            foreach (var departamento in departamentos)
            {
                Console.WriteLine($"Descrição: {departamento.Descricao} \t Excluído: {departamento.Excluido}");
            }
        }

        public static void ConsultaProjetada()
        {
            using var db = new ApplicationContext();
            Setup(db);

            var departamentos = db.Departamentos
            .Where(p => p.Id > 0)
            .Select(p => new { p.Descricao, Funcionarios = p.Funcionarios.Select(f => f.Nome) })
            .ToList();

            foreach (var departamento in departamentos)
            {
                Console.WriteLine($"Descrição: {departamento.Descricao}");

                foreach (var funcionario in departamento.Funcionarios)
                {
                    Console.WriteLine($"Nome: {funcionario}");
                }
            }
        }

        public static void ConsultaParametrizada()
        {
            using var db = new ApplicationContext();
            Setup(db);

            var id = 0;
            var departamentos = db.Departamentos
            .FromSqlRaw("SELECT * FROM Departamentos WHERE Id>{0}", id)
            .Where(p => p.Excluido != true)
            .ToList();

            foreach (var departamento in departamentos)
            {
                Console.WriteLine($"Descrição: {departamento.Descricao}");
            }
        }

        public static void ConsultaInterpolada()
        {
            using var db = new ApplicationContext();
            Setup(db);

            var id = 1;
            var departamentos = db.Departamentos
            .FromSqlInterpolated($"SELECT * FROM Departamentos WHERE Id>{id}")
            .Where(p => p.Excluido != true)
            .ToList();

            foreach (var departamento in departamentos)
            {
                Console.WriteLine($"Descrição: {departamento.Descricao}");
            }
        }

        public static void ConsultaCOMTAG()
        {
            using var db = new ApplicationContext();
            Setup(db);


            var departamentos = db.Departamentos
            .TagWith(@"ESTOU ENVIANDO UM COMENTARIO PARA O SERVIDRO
            SEGUNDO COMENTARIO,
            TERCEIRO COMENTARIO
            ")
            .ToList();

            foreach (var departamento in departamentos)
            {
                Console.WriteLine($"Descrição: {departamento.Descricao}");
            }
        }

        public static void EntendendoConsulta1NN1()
        {
            using var db = new ApplicationContext();
            Setup(db);


            // var departamentos = db.Departamentos
            // .Include(p => p.Funcionarios)
            // .ToList();

            // foreach (var departamento in departamentos)
            // {
            //     Console.WriteLine($"Descrição: {departamento.Descricao}");
            //     foreach (var funcionario in departamento.Funcionarios)
            //     {
            //         Console.WriteLine($"Nome: {funcionario.Nome}");
            //     }
            // }
            var funcionarios = db.Funcionarios
            .Include(p => p.Departamento)
            .ToList();

            foreach (var funcionario in funcionarios)
            {
                Console.WriteLine($"Nome: {funcionario.Nome} / Descrição Dep: {funcionario.Departamento.Descricao}");
            }
        }


        public static void DivisaoDeConsultas()
        {
            using var db = new ApplicationContext();
            Setup(db);

            var departamentos = db.Departamentos
            .Include(p => p.Funcionarios)
            .Where(p => p.Id < 3)
            //.AsSplitQuery()
            .AsSingleQuery()
            .ToList();

            foreach (var departamento in departamentos)
            {
                Console.WriteLine($"Descrição: {departamento.Descricao}");

                foreach (var funcionario in departamento.Funcionarios)
                {
                    Console.WriteLine($"Nome: {funcionario.Nome}");
                }
            }
        }

        public static void Setup(ApplicationContext db)
        {
            if (db.Database.EnsureCreated())
            {
                db.Departamentos.AddRange(new Departamento
                {
                    Descricao = "Departamento01",
                    Funcionarios = new System.Collections.Generic.List<Funcionario>{
                        new Funcionario{
                            Nome = "Marcio",
                            Cpf = "285.951.348-55",
                            Rg = "326147329"
                        }
                    },
                    Excluido = true
                },
                new Departamento
                {
                    Descricao = "Departamento02",
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

        public static void CriarStoredProcedure()
        {
            var criarDepartamento = @"
                CREATE OR ALTER PROCEDURE CriarDepartamento
                    @Descricao VARCHAR(50),
                    @Ativo bit
                AS
                BEGIN
                    INSERT INTO
                        Departamentos(Descricao, Ativo, Excluido)
                    VALUES (@Descricao, @Ativo, 0)
                END
            ";

            using var db = new ApplicationContext();

            db.Database.ExecuteSqlRaw(criarDepartamento);

        }

        public static void InserirDadosViaStoredProcedure()
        {
            using var db = new ApplicationContext();

            db.Database.ExecuteSqlRaw("execute CriarDepartamento @p0, @p1", "Departamento via Proc", true);
        }

        public static void CriarStoredProcedureConsulta()
        {
            var criarDepartamento = @"
                CREATE OR ALTER PROCEDURE GetDepartamentos
                    @Descricao VARCHAR(50)
                AS
                BEGIN
                    SELECT * FROM departamentos WHERE descricao Like @Descricao + '%'
                END
            ";

            using var db = new ApplicationContext();

            db.Database.ExecuteSqlRaw(criarDepartamento);

        }

        public static void ExecutaProcedureConsulta()
        {
            using var db = new ApplicationContext();

            var dep = new SqlParameter("@dep", "Departamento");

            var departamentos = db.Departamentos.FromSqlRaw("EXECUTE GetDepartamentos @dep", dep)
            .ToList();

            foreach (var item in departamentos)
            {
                Console.WriteLine(item.Descricao);
            }
        }
    }
}