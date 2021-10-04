using System;
using DominandoEntityFramework.Data;
using Microsoft.EntityFrameworkCore;

namespace DominandoEntityFramework.FuncoesDidaticas
{
    public static class DataAnnotations
    {
        public static void Atributos()
        {
            using (var db = new ApplicationContext())
            {
                var script = db.Database.GenerateCreateScript();

                //db.Database.EnsureDeleted();
                //db.Database.EnsureCreated();

                Console.WriteLine(script);

                db.Atributos.Add(new Domain.Atributo
                {
                    Descricao = "Exemplo",
                    Observacao = "Observação"
                });

                db.SaveChanges();

                /*
                db.RelatorioFinanceiro.Add(new Domain.RelatorioFinanceiro
                {
                    Descricao = "descricao",
                    Data = DateTime.Now
                });*/

                var dados = db.RelatorioFinanceiro.AsNoTracking();
                foreach (var item in dados)
                {
                    Console.WriteLine($"Descrição: { item.Descricao }");
                }
            }
        }
    }
}