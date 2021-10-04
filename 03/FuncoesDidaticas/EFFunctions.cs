using System;
using System.Linq;
using DominandoEntityFramework.Data;
using Microsoft.EntityFrameworkCore;

namespace DominandoEntityFramework.FuncoesDidaticas
{
    public static class EFFunctions
    {
        public static void ApagarCriarBancoDados()
        {
            using var db = new ApplicationContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            db.Funcoes.AddRange(
                new Domain.Funcao
                {
                    Data1 = DateTime.Now.AddDays(2),
                    Data2 = "2021-01-01",
                    Descricao1 = "Bala 1",
                    Descricao2 = "Bala 1"
                },
                new Domain.Funcao
                {
                    Data1 = DateTime.Now.AddDays(1),
                    Data2 = "XX21-01-01",
                    Descricao1 = "Bola 2",
                    Descricao2 = "Bola 2"
                },
                new Domain.Funcao
                {
                    Data1 = DateTime.Now.AddDays(60),
                    Data2 = "XX21-01-01",
                    Descricao1 = "Tela",
                    Descricao2 = "Tela"
                }
            );

            db.SaveChanges();
        }

        public static void FuncoesDeDatas()
        {
            ApagarCriarBancoDados();

            using (var db = new ApplicationContext())
            {
                var script = db.Database.GenerateCreateScript();
                Console.WriteLine(script);

                var dados = db.Funcoes.AsNoTracking().Select(p =>
                new
                {
                    Dias = EF.Functions.DateDiffDay(DateTime.Now, p.Data1),
                    Meses = EF.Functions.DateDiffMonth(DateTime.Now, p.Data1),
                    Data = EF.Functions.DateFromParts(2021, 1, 2),
                    DataValida = EF.Functions.IsDate(p.Data2)
                });

                foreach (var item in dados)
                {
                    Console.WriteLine(item);
                }
            }
        }

        public static void FuncaoLike()
        {
            using (var db = new ApplicationContext())
            {
                var script = db.Database.GenerateCreateScript();

                Console.WriteLine(script);

                var dados = db
                    .Funcoes
                    .AsNoTracking()
                    //.Where(x => EF.Functions.Like(x.Descricao1, "%ala 1"))
                    .Where(x => EF.Functions.Like(x.Descricao1, "B[ao]%"))
                    .Select(p => p.Descricao1)
                    .ToArray();

                Console.WriteLine("Resultado:");
                foreach (var item in dados)
                {
                    Console.WriteLine(item);
                }
            }
        }
        public static void FuncaoDataLength()
        {
            using (var db = new ApplicationContext())
            {
                ApagarCriarBancoDados();

                var resultado = db
                    .Funcoes
                    .AsNoTracking()
                    .Select(p => new
                    {
                        TotalBytesCampoData = EF.Functions.DataLength(p.Data1),
                        TotalBytes1 = EF.Functions.DataLength(p.Descricao1),
                        TotalBytes2 = EF.Functions.DataLength(p.Descricao2),
                        Total1 = p.Descricao1.Length,
                        Total2 = p.Descricao2.Length,
                        //Data1 = p.Data1.Le
                    })
                    .FirstOrDefault();


                Console.WriteLine(resultado);

            }
        }

        public static void FuncaoProperty()
        {
            ApagarCriarBancoDados();

            using (var db = new ApplicationContext())
            {

                var resultado = db
                    .Funcoes
                    //.AsNoTracking()
                    .FirstOrDefault(p => EF.Property<string>(p, "PropriedadeDeSombra") == "Teste");

                var propriedadeSombra = db
                    .Entry(resultado)
                    .Property<string>("PropriedadeDeSombra")
                    .CurrentValue;

                Console.WriteLine("Resultado:");
                //Console.WriteLine(resultado);
                Console.WriteLine(propriedadeSombra);

            }
        }

        public static void FuncaoCollate()
        {
            using (var db = new ApplicationContext())
            {
                var consulta1 = db
                    .Funcoes
                    .FirstOrDefault(p => EF.Functions.Collate(p.Descricao1, "SQL_Latin1_General_CP1_CS_AS") == "tela");

                var consulta2 = db
                    .Funcoes
                    .FirstOrDefault(p => EF.Functions.Collate(p.Descricao1, "SQL_Latin1_General_CP1_CI_AS") == "tela");

                Console.WriteLine($"Consulta1: {consulta1?.Descricao1}");

                Console.WriteLine($"Consulta1: {consulta2?.Descricao1}");

            }
        }

    }
}