using BankMore.Auth.Domain.Entities;
using BankMore.Auth.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankMore.Auth.Infrastructure.Persistence.Configurations
{
    public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> entity)
        {
            entity.ToTable("usuarios");
            entity.HasKey(e => e.Id).HasName("PK_Usuarios");
            entity.Property(e => e.Id).HasColumnName("id").IsRequired();
            entity.Property(e => e.Nome).HasColumnName("nome").HasMaxLength(100).IsRequired();

            entity.Property(e => e.Cpf)
                .HasColumnName("cpf")
                .HasMaxLength(11)
                .IsRequired()
                .HasConversion(
                    v => v.Numero,
                    v => new CPF(v)
                );
            entity.HasIndex(e => e.Cpf).IsUnique().HasDatabaseName("idx_usuarios_cpf");

            entity.Property(e => e.Email)
                .HasColumnName("email")
                .HasMaxLength(100)
                .IsRequired()
                .HasConversion(
                    v => v.Endereco,
                    v => new Email(v)
                );
            entity.HasIndex(e => e.Email).IsUnique().HasDatabaseName("idx_usuarios_email");

            entity.Property(e => e.SenhaHash).HasColumnName("senhaHash").HasMaxLength(255).IsRequired();
            entity.Property(e => e.Ativo).HasColumnName("ativo").HasDefaultValue(true).IsRequired();
            entity.Property(e => e.CriadoEm).HasColumnName("criadoEm").IsRequired();
        }
    }
}
