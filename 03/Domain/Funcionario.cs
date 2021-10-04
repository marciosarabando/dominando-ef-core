namespace DominandoEntityFramework.Domain
{
    public class Funcionario
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public string Rg { get; set; }

        //Não há necessidade de configurar essa propriedade, caso use o Entity para criar a base.
        //public int DepartamentoId { get; set; }
        public virtual Departamento Departamento { get; set; }
    }
}