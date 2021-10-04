using System;
using System.Linq;
using DominandoEntityFramework.Data;
using Microsoft.EntityFrameworkCore;

namespace DominandoEntityFramework.FuncoesDidaticas
{
    public static class Performance
    {
        public static void Setup()
        {
            using var db = new ApplicationContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            db.Departamentos.Add(new Domain.Departamento
            {
                Descricao = "Departamento Teste",
                Ativo = true,
                Funcionarios = Enumerable.Range(1, 100).Select(p => new Domain.Funcionario
                {
                    Cpf = "28595134855",
                    Nome = $"Funcionario {p}",
                    Rg = p.ToString()
                }).ToList()
            });

            db.SaveChanges();
        }

        public static void ConsultaRastreada()
        {
            using var db = new ApplicationContext();

            var funcionarios = db.Funcionarios.Include(p => p.Departamento).ToList();
        }

        public static void ConsultaNaoRastreada()
        {
            using var db = new ApplicationContext();

            var funcionarios = db.Funcionarios.AsNoTracking().Include(p => p.Departamento).ToList();
        }

        public static void ConsultaComResolucaoDeIdentidade()
        {
            using var db = new ApplicationContext();

            var funcionarios = db.Funcionarios
                .AsNoTrackingWithIdentityResolution()
                .Include(p => p.Departamento)
                .ToList();
        }

        public static void ConsultaCustomizada()
        {
            using var db = new ApplicationContext();

            db.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;

            var funcionarios = db.Funcionarios
                //.AsTracking()
                .Include(p => p.Departamento)
                .ToList();
        }


        public static void ConsultaProjetadaERastreada()
        {
            using var db = new ApplicationContext();

            db.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;

            var departamentos = db.Departamentos
                .Include(p => p.Funcionarios)
                .Select(p => new
                {
                    Departamento = p,
                    TotalFuncionarios = p.Funcionarios.Count()
                })
                .ToList();

            departamentos[0].Departamento.Descricao = "Departamento anonimo atualizado";

            db.SaveChanges();
        }

        public static void Inserir200DepartamentosCom1Mb()
        {
            using var db = new ApplicationContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            var random = new Random();

            db.Departamentos.AddRange(Enumerable.Range(1, 200).Select(p => new Domain.Departamento
            {
                Descricao = "Departamento Teste",
                Imagem = getBytes()
            }));

            db.SaveChanges();

            byte[] getBytes()
            {
                var buffer = new byte[1024 * 1024];
                random.NextBytes(buffer);

                return buffer;
            }
        }

        public static void ConsultaProjetada()
        {
            using var db = new ApplicationContext();

            //Consulta Não Projetada
            //var departamentos = db.Departamentos.ToArray();

            //Consulta Projetada
            var departamentos = db.Departamentos
                .Select(p => new
                {
                    descricao = p.Descricao,
                    nome = p.Funcionarios.Select(y => y.Nome).FirstOrDefault() ?? "Teste"
                })
                .ToArray();

            var memoria = (System.Diagnostics.Process.GetCurrentProcess().WorkingSet64 / 1024 / 1024) + "MB";

            foreach (var item in departamentos)
            {
                Console.WriteLine(item);
            }

            Console.WriteLine(memoria);
        }

    }
}