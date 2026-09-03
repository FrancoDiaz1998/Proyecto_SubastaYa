using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubastaYa.Domain.Entidades;
using SubastaYa.Infrastructure.Identidad;

namespace SubastaYa.Infrastructure.Persistencia.Configuraciones;

public class RegistroAuditoriaConfiguracion : IEntityTypeConfiguration<RegistroAuditoria>
{
    public void Configure(EntityTypeBuilder<RegistroAuditoria> builder)
    {
        builder.ToTable("registros_auditoria");

        builder.HasKey(registro => registro.Id);

        builder.Property(registro => registro.Id)
            .HasColumnName("id");

        builder.Property(registro => registro.UsuarioActorId)
            .HasColumnName("usuario_actor_id");

        builder.Property(registro => registro.TipoEntidad)
            .HasColumnName("tipo_entidad")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(registro => registro.EntidadId)
            .HasColumnName("entidad_id")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(registro => registro.Accion)
            .HasColumnName("accion")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(registro => registro.DetallesJson)
            .HasColumnName("detalles_json")
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(registro => registro.Fecha)
            .HasColumnName("fecha")
            .HasColumnType("timestamp with time zone");

        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(registro => registro.UsuarioActorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(registro => new
            {
                registro.TipoEntidad,
                registro.EntidadId,
                registro.Fecha
            })
            .HasDatabaseName("ix_auditoria_entidad_fecha");
    }
}
