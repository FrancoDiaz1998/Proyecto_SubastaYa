using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubastaYa.Domain.Entidades;

namespace SubastaYa.Infrastructure.Persistencia.Configuraciones;

public class MovimientoBilleteraConfiguracion : IEntityTypeConfiguration<MovimientoBilletera>
{
    public void Configure(EntityTypeBuilder<MovimientoBilletera> builder)
    {
        builder.ToTable("movimientos_billetera", tabla =>
            tabla.HasCheckConstraint("ck_movimientos_billetera_monto", "monto > 0"));

        builder.HasKey(movimiento => movimiento.Id);

        builder.Property(movimiento => movimiento.Id)
            .HasColumnName("id");

        builder.Property(movimiento => movimiento.BilleteraId)
            .HasColumnName("billetera_id");

        builder.Property(movimiento => movimiento.SubastaId)
            .HasColumnName("subasta_id");

        builder.Property(movimiento => movimiento.Tipo)
            .HasColumnName("tipo")
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(movimiento => movimiento.Monto)
            .HasColumnName("monto")
            .HasPrecision(18, 2);

        builder.Property(movimiento => movimiento.OperacionId)
            .HasColumnName("operacion_id");

        builder.Property(movimiento => movimiento.Fecha)
            .HasColumnName("fecha")
            .HasColumnType("timestamp with time zone");

        builder.HasOne<Billetera>()
            .WithMany()
            .HasForeignKey(movimiento => movimiento.BilleteraId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Subasta>()
            .WithMany()
            .HasForeignKey(movimiento => movimiento.SubastaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(movimiento => new { movimiento.BilleteraId, movimiento.Fecha })
            .HasDatabaseName("ix_movimientos_billetera_fecha");

        builder.HasIndex(movimiento => movimiento.OperacionId)
            .HasDatabaseName("ix_movimientos_operacion_id");
    }
}
