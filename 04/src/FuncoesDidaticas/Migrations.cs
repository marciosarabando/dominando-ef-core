using System;
using Microsoft.EntityFrameworkCore;
using src.Data;

namespace src.FuncoesDidaticas
{
    public static class ClassMigrations
    {
        public static void GetMigrations()
        {
            using var db = new ApplicationContext();

            //db.Database.Migrate();

            var migracoes = db.Database.GetPendingMigrations();

            foreach (var migracao in migracoes)
            {
                Console.WriteLine(migracao);
            }
        }
    }
}