using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubastaYa.Domain.Entidades;

namespace SubastaYa.Infrastructure.Persistencia.Configuraciones;

public class PujaConfiguracion : IEntityTypeConfiguration<Puja>
{
    public void Configure(EntityTypeBuilder<Puja> builder)
    {
        builder.ToTable("pujas", tabla =>
            tabla.HasCheckConstraint("ck_pujas_monto", "monto > 0"));

        builder.HasKey(puja => puja.Id);

        builder.Property(puja => puja.Id)
            .HasColumnName("id");

        builder.Property(puja => puja.SubastaId)
            .HasColumnName("subasta_id");

        builder.Property(puja => puja.PostorId)
            .HasColumnName("postor_id");

        builder.Property(puja => puja.Monto)
            .HasColumnName("monto")
            .HasPrecision(18, 2);

        builder.Property(puja => puja.Fecha)
            .HasColumnName("fecha")
            .HasColumnType("timestamp with time zone");

        // Relación explícita con Subasta (Una subasta tiene muchas pujas)
        builder.HasOne(puja => puja.Subasta)
            .WithMany(subasta => subasta.Pujas)
            .HasForeignKey(puja => puja.SubastaId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación explícita con Usuario (Un usuario/postor realiza muchas pujas)
        builder.HasOne(puja => puja.Postor)
            .WithMany(usuario => usuario.Pujas)
            .HasForeignKey(puja => puja.PostorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Índices para optimizar búsquedas de pujas por fecha y monto
        builder.HasIndex(puja => new { puja.SubastaId, puja.Fecha })
            .HasDatabaseName("ix_pujas_subasta_fecha");

        builder.HasIndex(puja => new { puja.SubastaId, puja.Monto })
            .HasDatabaseName("ix_pujas_subasta_monto");
    }
}