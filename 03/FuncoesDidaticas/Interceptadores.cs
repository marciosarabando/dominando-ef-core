using System;
using System.Linq;
using System.Threading.Tasks;
using DominandoEntityFramework.Data;
using Microsoft.EntityFrameworkCore;

namespace DominandoEntityFramework.FuncoesDidaticas
{
    public static class Interceptadores
    {
        public static void TesteInterceptacao()
        {
            using (var db = new ApplicationContext())
            {
                var consulta = db
                    .Funcoes
                    .TagWith("Use NOLOCK")
                    .AsNoTracking()
                    .FirstOrDefault();

                Console.WriteLine($"Consulta: {consulta?.Descricao1}");
            }
        }
        public static void TesteInterceptacaoSavingChanges()
        {
            using (var db = new ApplicationContext())
            {
                db.Funcoes.Add(new Domain.Funcao
                {
                    Descricao1 = "Teste123"
                });

                db.SaveChanges();
            }
        }
    }
}