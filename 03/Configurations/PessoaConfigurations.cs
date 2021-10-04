using DominandoEntityFramework.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DominandoEntityFramework.Configurations
{
    public class PessoaConfigurations : IEntityTypeConfiguration<Pessoa>
    {
        public void Configure(EntityTypeBuilder<Pessoa> builder)
        {
            /*CONF DE TABELA POR HIERARQUIA
            builder
                .ToTable("Pessoas")
                .HasDiscriminator<int>("TipoPessoa")
                .HasValue<Pessoa>(3)
                .HasValue<Instrutor>(6)
                .HasValue<Aluno>(99);*/

            /* CONF para Tabela por Tipo (TPT)*/
            builder
                .ToTable("Pessoas");
        }
    }

    public class InstrutorConfigurations : IEntityTypeConfiguration<Instrutor>
    {
        public void Configure(EntityTypeBuilder<Instrutor> builder)
        {
            /* CONF para Tabela por Tipo (TPT)*/
            builder
                .ToTable("Instrutores");
        }
    }

    public class AlunoConfigurations : IEntityTypeConfiguration<Aluno>
    {
        public void Configure(EntityTypeBuilder<Aluno> builder)
        {
            /* CONF para Tabela por Tipo (TPT)*/
            builder
                .ToTable("Alunos");
        }
    }
}