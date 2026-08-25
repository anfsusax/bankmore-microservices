using BankMore.Auth.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankMore.Auth.Infrastructure.Persistence.Configurations
{
    public class TransferenciaConfiguration : IEntityTypeConfiguration<Transferencia>
    {
        public void Configure(EntityTypeBuilder<Transferencia> entity)
        {
            entity.ToTable("transferencia");
            entity.HasKey(e => e.Id).HasName("PK_Transferencia");
            entity.Property(e => e.Id).HasColumnName("idtransferencia").IsRequired();

            entity.Property(e => e.IdContaOrigem).HasColumnName("idcontaorigem").IsRequired();
            entity.HasIndex(e => e.IdContaOrigem).HasDatabaseName("idx_transferencia_origem");

            entity.Property(e => e.IdContaDestino).HasColumnName("idcontadestino").IsRequired();
            entity.HasIndex(e => e.IdContaDestino).HasDatabaseName("idx_transferencia_destino");

            entity.Property(e => e.DataMovimento).HasColumnName("datamovimento").IsRequired();
            entity.Property(e => e.Valor).HasColumnName("valor").HasColumnType("decimal(18,2)").IsRequired();
        }
    }
}
