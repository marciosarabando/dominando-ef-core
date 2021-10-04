using System;
using System.Linq;
using System.Transactions;
using DominandoEntityFramework.Data;
using Microsoft.EntityFrameworkCore;

namespace DominandoEntityFramework.FuncoesDidaticas
{
    public static class Transacoes
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
                    Autor = "Rafael Almeida"
                });

                db.SaveChanges();
            }
        }

        public static void ComportamentoPadrao()
        {
            CadastrarLivro();

            using (var db = new ApplicationContext())
            {
                var livro = db.Livros.FirstOrDefault(p => p.Id == 1);

                livro.Autor = "Marcio Sarabando";

                db.Livros.Add(new Domain.Livro
                {
                    Titulo = "Introdução ao Entity Framework Core",
                    Autor = "Rafael Almeida"
                });

                db.SaveChanges();
            }
        }
        public static void GerenciandoTransacaoManualmente()
        {
            CadastrarLivro();

            using (var db = new ApplicationContext())
            {
                var transacao = db.Database.BeginTransaction();

                var livro = db.Livros.FirstOrDefault(p => p.Id == 1);
                livro.Autor = "Marcio Sarabando";
                db.SaveChanges();

                Console.ReadKey();

                db.Livros.Add(new Domain.Livro
                {
                    Titulo = "Dominando o Entity Framework Core",
                    Autor = "Rafael Almeida"
                });
                db.SaveChanges();

                transacao.Commit();
            }
        }

        public static void ReverterTransacao()
        {
            CadastrarLivro();

            using (var db = new ApplicationContext())
            {
                var transacao = db.Database.BeginTransaction();

                try
                {
                    var livro = db.Livros.FirstOrDefault(p => p.Id == 1);
                    livro.Autor = "Marcio Sarabando";

                    db.SaveChanges();

                    db.Livros.Add(new Domain.Livro
                    {
                        Titulo = "Dominando o Entity Framework Core",
                        Autor = "Rafael Almeida".PadLeft(80, '*')
                    });

                    db.SaveChanges();

                    transacao.Commit();
                }
                catch (System.Exception)
                {
                    transacao.Rollback();
                    throw;
                }
            }
        }

        public static void SalvarPontoTransacao()
        {
            using (var db = new ApplicationContext())
            {
                var transacao = db.Database.BeginTransaction();

                try
                {
                    var livro = db.Livros.FirstOrDefault(p => p.Id == 1);
                    livro.Autor = "Marcio Sarabando";

                    db.SaveChanges();

                    transacao.CreateSavepoint("comita_ate_alteracao");

                    db.Livros.Add(new Domain.Livro
                    {
                        Titulo = "Dominando o Entity Framework Core",
                        Autor = "Cassia Sarabando"
                    });

                    db.SaveChanges();

                    db.Livros.Add(new Domain.Livro
                    {
                        Titulo = "Dominando o Entity Framework Core",
                        Autor = "Rafael Almeida".PadLeft(80, '*')
                    });

                    db.SaveChanges();

                    transacao.Commit();
                }
                catch (DbUpdateException e)
                {
                    transacao.RollbackToSavepoint("comita_ate_alteracao");

                    if (e.Entries.Count(p => p.State == EntityState.Added) == e.Entries.Count)
                    {
                        transacao.Commit();
                    }
                }

            }
        }

        public static void CadastrarLivroDominandoEFCore()
        {
            using (var db = new ApplicationContext())
            {
                db.Livros.Add(new Domain.Livro
                {
                    Titulo = "Introdução o Entity Framework Core",
                    Autor = "Rafael Almeida"
                });

                db.SaveChanges();
            }
        }
        public static void CadastrarLivroEnterprise()
        {
            using (var db = new ApplicationContext())
            {
                db.Livros.Add(new Domain.Livro
                {
                    Titulo = "ASP .NET Core Enterprise Applications",
                    Autor = "Eduardo Pires"
                });

                db.SaveChanges();
            }
        }

        public static void ConsultarAtualizar()
        {
            using (var db = new ApplicationContext())
            {
                var livro = db.Livros.FirstOrDefault(p => p.Id == 1);
                livro.Autor = "Rafael Almeida Alterado";
                db.SaveChanges();
            }
        }

        public static void TransactionScope()
        {
            CadastrarLivro();

            var transactionOptions = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted
            };

            using (var scope = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
            {
                ConsultarAtualizar();
                CadastrarLivroEnterprise();
                CadastrarLivroDominandoEFCore();

                scope.Complete();
            }
        }
    }
}