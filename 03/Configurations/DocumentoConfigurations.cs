using DominandoEntityFramework.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DominandoEntityFramework.Configurations
{
    public class DocumentoConfigurations : IEntityTypeConfiguration<Documento>
    {
        public void Configure(EntityTypeBuilder<Documento> builder)
        {
            builder
                //Para popular a propriedade privada
                //.Property(p => p.CPF)
                //.HasField("_cpf");

                //Popular diretamente a propriedade privada
                .Property("_cpf").HasColumnName("CPF").HasMaxLength(11);
        }
    }
}