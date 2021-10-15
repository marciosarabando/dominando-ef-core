using System;
using System.Linq;
using src.Data;

namespace src.FuncoesDidaticas
{
    public static class OutrosBancoDados
    {
        public static void SetupPostgres()
        {
            using var db = new ApplicationContext();

            db.Database.EnsureCreated();

            var pessoas = db.Pessoas.ToList();
        }

        public static void AddPessoaSQLite()
        {
            using var db = new ApplicationContext();

            db.Database.EnsureCreated();

            db.Pessoas.Add(new Domain.Pessoa
            {
                Nome = "Marcio",
                Telefone = "996586669"
            });

            db.SaveChanges();

            var pessoas = db.Pessoas.ToList();

            foreach (var item in pessoas)
            {
                Console.WriteLine($"Nome: { item.Nome }");
            }
        }
    }
}