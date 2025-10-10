using BankMore.Auth.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankMore.Auth.Infrastructure.Persistence.Configurations
{
    public class ContaCorrenteConfiguration : IEntityTypeConfiguration<ContaCorrente>
    {
        public void Configure(EntityTypeBuilder<ContaCorrente> entity)
        { 
            entity.ToTable("contacorrente");
            entity.HasKey(e => e.Id).HasName("PK_ContaCorrente");
            entity.Property(e => e.Id).HasColumnName("idcontacorrente").IsRequired();
            entity.Property(e => e.Numero).HasColumnName("numero").IsRequired();
            entity.Property(e => e.Nome).HasColumnName("nome").HasMaxLength(200).IsRequired();
            entity.Property(e => e.Ativo).HasColumnName("ativo").HasDefaultValue(true).IsRequired();
            entity.Property(e => e.Senha).HasColumnName("senha").HasMaxLength(200).IsRequired();
            entity.Property(e => e.Salt).HasColumnName("salt").HasMaxLength(200).IsRequired();
            entity.Property(e => e.Saldo).HasColumnName("saldo").HasColumnType("decimal(18,2)").HasDefaultValue(0m).IsRequired();
            entity.Property(e => e.CriadoEm).HasColumnName("criadoEm").IsRequired();
            entity.Property(e => e.AtualizadoEm).HasColumnName("atualizadoEm");
        }
    }
}
