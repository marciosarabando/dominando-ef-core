using Microsoft.EntityFrameworkCore;

namespace DominandoEntityFramework.Domain
{
    public class Documento
    {
        private string _cpf;
        public int Id { get; set; }

        public void SetCPF(string cpf)
        {
            //Validações
            if (string.IsNullOrWhiteSpace(cpf))
            {
                throw new System.Exception("CPF Inválido");
            }

            _cpf = cpf;
        }

        [BackingField(nameof(_cpf))]
        public string CPF => _cpf;

        public string getCPF() => _cpf;
    }
}