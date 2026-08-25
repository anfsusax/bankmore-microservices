using BankMore.Auth.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankMore.Auth.Infrastructure.Persistence.Configurations
{
    public class MovimentoConfiguration : IEntityTypeConfiguration<Movimento>
    {
        public void Configure(EntityTypeBuilder<Movimento> entity)
        {
            entity.ToTable("movimento");
            entity.HasKey(e => e.Id).HasName("PK_Movimento");
            entity.Property(e => e.Id).HasColumnName("idmovimento").IsRequired();
            entity.Property(e => e.IdContaCorrente).HasColumnName("idcontacorrente").IsRequired();
            entity.HasIndex(e => e.IdContaCorrente).HasDatabaseName("idx_movimento_conta");

            entity.Property(e => e.DataMovimento).HasColumnName("datamovimento").IsRequired();
            entity.HasIndex(e => e.DataMovimento).HasDatabaseName("idx_movimento_data");

            entity.Property(e => e.TipoMovimento).HasColumnName("tipomovimento").HasMaxLength(1).IsRequired();
            entity.Property(e => e.Valor).HasColumnName("valor").HasColumnType("decimal(18,2)").IsRequired();

            entity.Property(e => e.ChaveIdempotencia).HasColumnName("chave_idempotencia").HasMaxLength(255).IsRequired();
            entity.HasIndex(e => e.ChaveIdempotencia).IsUnique().HasDatabaseName("idx_movimento_idempotencia");

            entity.Property(e => e.Descricao).HasColumnName("descricao").HasMaxLength(255);
        }
    }
}
