using System.Linq;
using DominandoEntityFramework.Data;
using Microsoft.EntityFrameworkCore;

namespace DominandoEntityFramework.FuncoesDidaticas
{
    public static class InfraEstrutura
    {
        public static void ConsultarDepartamentos()
        {
            using var db = new ApplicationContext();

            var departamentos = db.Departamentos.Where(p => p.Id > 0).ToArray();
        }

        public static void DadosSensiveis()
        {
            using var db = new ApplicationContext();

            var descricao = "Departamento";

            var departamentos = db.Departamentos.Where(p => p.Descricao == descricao).ToArray();
        }

        public static void HabilitandoBachSize()
        {
            using var db = new ApplicationContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            for (int i = 0; i < 50; i++)
            {
                db.Departamentos.Add(new Domain.Departamento
                {
                    Descricao = "Departamento " + i
                });
            }

            db.SaveChanges();
        }

        public static void TempoComandoGeral()
        {
            using var db = new ApplicationContext();

            db.Database.SetCommandTimeout(10);

            db.Database.ExecuteSqlRaw("WAITFOR DELAY '00:00:07'; SELECT 1");
        }

        public static void ExecutaEstrategiaResiliencia()
        {
            using var db = new ApplicationContext();

            var strategy = db.Database.CreateExecutionStrategy();

            strategy.Execute(() =>
            {
                using var transaction = db.Database.BeginTransaction();

                db.Departamentos.Add(new Domain.Departamento { Descricao = "Departamento Transacao" });
                db.SaveChanges();

                transaction.Commit();
            });
        }
    }
}