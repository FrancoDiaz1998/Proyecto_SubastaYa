using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubastaYa.Domain.Entidades;
using SubastaYa.Infrastructure.Identidad;

namespace SubastaYa.Infrastructure.Persistencia.Configuraciones;

public class VentaConfiguracion : IEntityTypeConfiguration<Venta>
{
    public void Configure(EntityTypeBuilder<Venta> builder)
    {
        builder.ToTable("ventas", tabla =>
            tabla.HasCheckConstraint("ck_ventas_precio_final", "precio_final > 0"));

        builder.HasKey(venta => venta.Id);

        builder.Property(venta => venta.Id)
            .HasColumnName("id");

        builder.Property(venta => venta.SubastaId)
            .HasColumnName("subasta_id");

        builder.Property(venta => venta.CompradorId)
            .HasColumnName("comprador_id");

        builder.Property(venta => venta.VendedorId)
            .HasColumnName("vendedor_id");

        builder.Property(venta => venta.PrecioFinal)
            .HasColumnName("precio_final")
            .HasPrecision(18, 2);

        builder.Property(venta => venta.Fecha)
            .HasColumnName("fecha")
            .HasColumnType("timestamp with time zone");

        builder.HasOne<Subasta>()
            .WithOne()
            .HasForeignKey<Venta>(venta => venta.SubastaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(venta => venta.CompradorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(venta => venta.VendedorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(venta => venta.SubastaId)
            .IsUnique()
            .HasDatabaseName("ux_ventas_subasta_id");
    }
}
