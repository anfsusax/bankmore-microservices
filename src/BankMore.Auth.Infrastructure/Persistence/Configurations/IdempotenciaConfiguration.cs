using BankMore.Auth.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankMore.Auth.Infrastructure.Persistence.Configurations
{
    public class IdempotenciaConfiguration : IEntityTypeConfiguration<Idempotencia>
    {
        public void Configure(EntityTypeBuilder<Idempotencia> entity)
        {
            entity.ToTable("idempotencia");
            entity.HasKey(e => e.Chave).HasName("PK_Idempotencia");
            entity.Property(e => e.Chave).HasColumnName("chave_idempotencia").HasMaxLength(255).IsRequired();
            entity.Property(e => e.Requisicao).HasColumnName("requisicao");
            entity.Property(e => e.Resultado).HasColumnName("resultado");
            entity.Property(e => e.CriadoEm).HasColumnName("criadoEm").IsRequired();
        }
    }
}
