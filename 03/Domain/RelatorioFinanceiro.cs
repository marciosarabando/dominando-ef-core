using System;
using Microsoft.EntityFrameworkCore;

namespace DominandoEntityFramework.Domain
{
    [Keyless]
    public class RelatorioFinanceiro
    {
        public string Descricao { get; set; }
        public string Total { get; set; }
        public DateTime Data { get; set; }
    }
}