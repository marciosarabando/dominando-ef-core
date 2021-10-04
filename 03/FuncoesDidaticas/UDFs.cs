using System;
using System.Linq;
using DominandoEntityFramework.Data;
using DominandoEntityFramework.Funcoes;
using Microsoft.EntityFrameworkCore;

namespace DominandoEntityFramework.FuncoesDidaticas
{
    public class UDFs
    {
        public static void CadastrarLivro()
        {
            using (var db = new ApplicationContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                db.Livros.Add(new Domain.Livro
                {
                    Titulo = "Introdução o Entity Framework Core",
                    Autor = "Rafael Almeida",
                    CadastradoEm = DateTime.Now.AddDays(-1)
                });

                db.SaveChanges();
            }
        }

        public static void FuncaoLEFT()
        {
            CadastrarLivro();

            using var db = new ApplicationContext();

            var resultado = db.Livros.Select(p => MinhasFuncoes.Left(p.Titulo, 10));

            foreach (var parteTitulo in resultado)
            {
                Console.WriteLine(parteTitulo);
            }
        }

        public static void FuncaoDefinidaPeloUsuario()
        {
            CadastrarLivro();

            using var db = new ApplicationContext();

            db.Database.ExecuteSqlRaw(@"
                CREATE FUNCTION ConverterParaLetrasMaiusculas(@dados VARCHAR(100))
                RETURNS VARCHAR(100)
                BEGIN
                    RETURN UPPER(@dados)
                END
            ");

            var resultado = db.Livros.Select(p => MinhasFuncoes.LetrasMaiusculas(p.Titulo));

            foreach (var resultadoMaiusculo in resultado)
            {
                Console.WriteLine(resultadoMaiusculo);
            }

        }

        public static void DateDiff()
        {
            CadastrarLivro();

            using var db = new ApplicationContext();

            //var resultado = db.Livros.Select(p => EF.Functions.DateDiffDay(p.CadastradoEm, DateTime.Now));
            var resultado = db.Livros.Select(p => MinhasFuncoes.DateDiff("DAY", p.CadastradoEm, DateTime.Now));

            foreach (var diff in resultado)
            {
                Console.WriteLine(diff);
            }
        }

    }
}