using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubastaYa.Domain.Entidades;

namespace SubastaYa.Infrastructure.Persistencia.Configuraciones;

public class CategoriaConfiguracion : IEntityTypeConfiguration<Categoria>
{
    public void Configure(EntityTypeBuilder<Categoria> builder)
    {
        builder.ToTable("categorias");

        builder.HasKey(categoria => categoria.Id);

        builder.Property(categoria => categoria.Id)
            .HasColumnName("id");

        builder.Property(categoria => categoria.Nombre)
            .HasColumnName("nombre")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(categoria => categoria.UrlIcono)
            .HasColumnName("url_icono")
            .HasMaxLength(2048);

        builder.HasIndex(categoria => categoria.Nombre)
            .IsUnique()
            .HasDatabaseName("ux_categorias_nombre");
    }
}
