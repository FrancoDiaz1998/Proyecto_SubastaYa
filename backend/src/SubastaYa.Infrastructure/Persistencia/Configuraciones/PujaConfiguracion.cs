using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubastaYa.Domain.Entidades;
using SubastaYa.Infrastructure.Identidad;

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

        builder.HasOne<Subasta>()
            .WithMany()
            .HasForeignKey(puja => puja.SubastaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(puja => puja.PostorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(puja => new { puja.SubastaId, puja.Fecha })
            .HasDatabaseName("ix_pujas_subasta_fecha");

        builder.HasIndex(puja => new { puja.SubastaId, puja.Monto })
            .HasDatabaseName("ix_pujas_subasta_monto");
    }
}
