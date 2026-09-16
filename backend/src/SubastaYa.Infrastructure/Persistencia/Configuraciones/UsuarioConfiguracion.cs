using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubastaYa.Domain.Entidades;

namespace SubastaYa.Infrastructure.Persistencia.Configuraciones;

public class UsuarioConfiguracion : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("usuarios");

        builder.HasKey(usuario => usuario.Id);

        builder.Property(usuario => usuario.Id)
            .HasColumnName("id");

        builder.Property(usuario => usuario.Nombre)
            .HasColumnName("nombre")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(usuario => usuario.Email)
            .HasColumnName("email")
            .HasMaxLength(256)
            .IsRequired();

        builder.HasIndex(usuario => usuario.Email)
            .IsUnique()
            .HasDatabaseName("ux_usuarios_email");

        builder.Property(usuario => usuario.PasswordHash)
            .HasColumnName("password_hash")
            .IsRequired();

        builder.Property(usuario => usuario.FechaRegistro)
            .HasColumnName("fecha_registro")
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}
