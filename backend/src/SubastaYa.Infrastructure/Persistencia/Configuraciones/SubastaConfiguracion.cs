using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubastaYa.Domain.Entidades;
using SubastaYa.Infrastructure.Identidad;

namespace SubastaYa.Infrastructure.Persistencia.Configuraciones;

public class SubastaConfiguracion : IEntityTypeConfiguration<Subasta>
{
    public void Configure(EntityTypeBuilder<Subasta> builder)
    {
        builder.ToTable("subastas", tabla =>
        {
            tabla.HasCheckConstraint(
                "ck_subastas_precios_positivos",
                "precio_base > 0 AND incremento_minimo > 0");
            tabla.HasCheckConstraint(
                "ck_subastas_fechas",
                "fecha_fin > fecha_inicio");
        });

        builder.HasKey(subasta => subasta.Id);

        builder.Property(subasta => subasta.Id)
            .HasColumnName("id");

        builder.Property(subasta => subasta.VendedorId)
            .HasColumnName("vendedor_id");

        builder.Property(subasta => subasta.CategoriaId)
            .HasColumnName("categoria_id");

        builder.Property(subasta => subasta.Titulo)
            .HasColumnName("titulo")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(subasta => subasta.Descripcion)
            .HasColumnName("descripcion")
            .HasMaxLength(4000)
            .IsRequired();

        builder.Property(subasta => subasta.UrlImagen)
            .HasColumnName("url_imagen")
            .HasMaxLength(2048)
            .IsRequired();

        builder.Property(subasta => subasta.PrecioBase)
            .HasColumnName("precio_base")
            .HasPrecision(18, 2);

        builder.Property(subasta => subasta.IncrementoMinimo)
            .HasColumnName("incremento_minimo")
            .HasPrecision(18, 2);

        builder.Property(subasta => subasta.FechaInicio)
            .HasColumnName("fecha_inicio")
            .HasColumnType("timestamp with time zone");

        builder.Property(subasta => subasta.FechaFin)
            .HasColumnName("fecha_fin")
            .HasColumnType("timestamp with time zone");

        builder.Property(subasta => subasta.Estado)
            .HasColumnName("estado")
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(subasta => subasta.Version)
            .HasColumnName("version")
            .HasDefaultValue(0L)
            .IsConcurrencyToken();

        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(subasta => subasta.VendedorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Categoria>()
            .WithMany()
            .HasForeignKey(subasta => subasta.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(subasta => subasta.Estado)
            .HasDatabaseName("ix_subastas_estado");

        builder.HasIndex(subasta => subasta.FechaFin)
            .HasDatabaseName("ix_subastas_fecha_fin");

        builder.HasIndex(subasta => new { subasta.CategoriaId, subasta.Estado })
            .HasDatabaseName("ix_subastas_categoria_estado");
    }
}
