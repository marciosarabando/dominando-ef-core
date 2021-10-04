using System;
using System.Collections.Generic;
using System.Linq;
using DominandoEntityFramework.Data;
using DominandoEntityFramework.Domain;
using Microsoft.EntityFrameworkCore;

namespace DominandoEntityFramework.FuncoesDidaticas
{
    public static class ModeloDeDados
    {
        public static void CollationsExemple()
        {
            using var db = new ApplicationContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
        }

        public static void PropagarDados()
        {
            using var db = new ApplicationContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            var script = db.Database.GenerateCreateScript();
            Console.WriteLine(script);
        }

        public static void Esquema()
        {
            using var db = new ApplicationContext();

            var script = db.Database.GenerateCreateScript();

            Console.WriteLine(script);
        }

        public static void ConversorValor() => Esquema();

        public static void ConversorCustomizado()
        {
            using var db = new ApplicationContext();

            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            db.Conversores.Add(
            new Domain.Conversor
            {
                Status = Domain.Status.Devolvido
            });

            db.SaveChanges();

            var conversorEmAnalise = db.Conversores.AsNoTracking().FirstOrDefault(p => p.Status == Status.Analise);

            var conversorDevolvido = db.Conversores.AsNoTracking().FirstOrDefault(p => p.Status == Status.Devolvido);
        }

        public static void PropriedadesDeSombra()
        {
            using var db = new ApplicationContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
        }

        public static void TrabalhandoComPropriedadesDeSombra()
        {
            using var db = new ApplicationContext();

            /*
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            var departamento = new Departamento
            {
                Descricao = "Departamento Propriedade de Sombra"
            };

            db.Departamentos.Add(departamento);
            db.Entry(departamento).Property("UltimaAtualizacao").CurrentValue = DateTime.Now;
            */

            var departamentos = db.Departamentos.Where(p => EF.Property<DateTime>(p, "UltimaAtualizacao") < DateTime.Now).ToArray();

            db.SaveChanges();
        }

        public static void TiposPropriedades()
        {
            using var db = new ApplicationContext();

            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            var cliente = new Cliente
            {
                Nome = "Fulano",
                Telefone = "13996586669",
                Endereco = new Endereco { Bairro = "Vila Nova", Cidade = "Cubatao" }
            };

            db.Clientes.Add(cliente);

            db.SaveChanges();

            var clientes = db.Clientes.AsNoTracking().ToList();

            var options = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };

            clientes.ForEach(cli =>
            {
                var json = System.Text.Json.JsonSerializer.Serialize(cli, options);

                Console.WriteLine(json);
            });
        }

        public static void Relacionamentos1Para1()
        {
            using var db = new ApplicationContext();

            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            var estado = new Estado
            {
                Nome = "São Paulo",
                Governador = new Governador { Nome = "Marcio Sarabando" }
            };

            db.Estados.Add(estado);

            db.SaveChanges();

            //var estados = db.Estados.AsNoTracking().Include(x => x.Governador).ToList();
            var estados = db.Estados.AsNoTracking().ToList();

            estados.ForEach(est =>
            {
                Console.WriteLine($"Estado: { est.Nome }, Governador: { est.Governador.Nome }");
            });
        }

        public static void Relacionamentos1ParaMuitos()
        {
            using (var db = new ApplicationContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                var estado = new Estado
                {
                    Nome = "São Paulo",
                    Governador = new Governador { Nome = "Marcio Sarabando" }
                };

                estado.Cidades.Add(new Cidade { Nome = "Cubatão" });

                db.Estados.Add(estado);

                db.SaveChanges();
            }

            using (var db = new ApplicationContext())
            {
                var estados = db.Estados.ToList();

                estados[0].Cidades.Add(new Cidade { Nome = "Santos" });

                db.SaveChanges();

                foreach (var est in db.Estados.Include(p => p.Cidades).AsNoTracking())
                {
                    Console.WriteLine($"Estado: { est.Nome }, Governador: { est.Governador.Nome }");

                    foreach (var cidade in est.Cidades)
                    {
                        Console.WriteLine($"\t  Cidade: { cidade.Nome }");
                    }
                }
            }
        }

        public static void RelacionamentosMuitosParaMuitos()
        {
            using (var db = new ApplicationContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                var ator1 = new Ator { Nome = "Marcio" };
                var ator2 = new Ator { Nome = "Cassia" };
                var ator3 = new Ator { Nome = "Sofia" };

                var filme1 = new Filme { Descricao = "A volta dos que não foram" };
                var filme2 = new Filme { Descricao = "De volta para o futuro" };
                var filme3 = new Filme { Descricao = "Poeira em alto mar filme" };

                ator1.Filmes.Add(filme1);
                ator1.Filmes.Add(filme2);

                ator2.Filmes.Add(filme1);

                filme3.Atores.Add(ator1);
                filme3.Atores.Add(ator2);
                filme3.Atores.Add(ator3);

                db.AddRange(ator1, ator2, filme3);

                db.SaveChanges();

                foreach (var ator in db.Atores.Include(p => p.Filmes))
                {
                    Console.WriteLine($"Ator: { ator.Nome }");

                    foreach (var filme in ator.Filmes)
                    {
                        Console.WriteLine($"tFilme: { filme.Descricao }");

                    }
                }
            }
        }

        public static void CampoDeApoio()
        {
            using (var db = new ApplicationContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                var documento = new Documento();
                documento.SetCPF("28595134855");

                db.Documentos.Add(documento);
                db.SaveChanges();

                foreach (var doc in db.Documentos.AsNoTracking())
                {
                    Console.WriteLine($"CPF -> { doc.getCPF() }");
                }
            }
        }

        public static void ExemploTPH()
        {
            using (var db = new ApplicationContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                var pessoa = new Pessoa
                {
                    Nome = "Fulano de Tal"
                };

                var instrutor = new Instrutor
                {
                    Nome = "Marcio Sarabando",
                    Tecnologia = ".NET",
                    Desde = DateTime.Now
                };

                var aluno = new Aluno { Nome = "Maria da Silva", Idade = 31, DataContrato = DateTime.Now.AddDays(-1) };

                db.AddRange(pessoa, instrutor, aluno);
                db.SaveChanges();

                var pessoas = db.Pessoas.AsNoTracking().ToArray();
                var instrutores = db.Instrutores.AsNoTracking().ToArray();
                //var alunos = db.Alunos.AsNoTracking().ToArray();
                var alunos = db.Pessoas.OfType<Aluno>().AsNoTracking().ToArray();

                Console.WriteLine("Pessoas ********************");
                foreach (var p in pessoas)
                {
                    Console.WriteLine($"Id: {p.Id} -> {p.Nome}");
                }

                Console.WriteLine("Instrutores ********************");
                foreach (var i in instrutores)
                {
                    Console.WriteLine($"Id: {i.Id} -> {i.Nome}, Tecnologia: {i.Tecnologia}, Desde: {i.Desde}");
                }

                Console.WriteLine("Alunos ********************");
                foreach (var p in alunos)
                {
                    Console.WriteLine($"Id: {p.Id} -> {p.Nome}, Idade: {p.Idade}, Data do Contrato: {p.DataContrato}");
                }


            }
        }

        public static void PacotesDePropriedade()
        {
            using (var db = new ApplicationContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                var configuracao = new Dictionary<string, object>
                {
                    ["Chave"] = "SenhaBancoDados",
                    ["Valor"] = Guid.NewGuid().ToString()
                };

                db.Configuracoes.Add(configuracao);
                db.SaveChanges();

                var configuracoes = db
                    .Configuracoes
                    .AsNoTracking()
                    .Where(p => p["Chave"] == "SenhaBancoDados")
                    .ToArray();

                foreach (var dic in configuracoes)
                {
                    Console.WriteLine($"Chave: {dic["Chave"]} - Valor: {dic["Valor"]}");
                };
            }
        }
    }
}