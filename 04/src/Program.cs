using System;
using Microsoft.EntityFrameworkCore;
using src.Data;
using src.FuncoesDidaticas;

namespace EFCore_DevIO
{
    class Program
    {
        static void Main(string[] args)
        {
            //ClassMigrations.GetMigrations();

            //OutrosBancoDados.SetupPostgres();
            OutrosBancoDados.AddPessoaSQLite();

            Console.WriteLine("Hello World!");
        }
    }
}
