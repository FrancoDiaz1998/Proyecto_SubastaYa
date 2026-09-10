using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubastaYa.Domain.Entidades;

namespace SubastaYa.Infrastructure.Persistencia.Configuraciones;

public class ConfiguracionDatosSemilla :
    IEntityTypeConfiguration<Categoria>,
    IEntityTypeConfiguration<Usuario>,
    IEntityTypeConfiguration<Billetera>,
    IEntityTypeConfiguration<Subasta>,
    IEntityTypeConfiguration<Puja>,
    IEntityTypeConfiguration<MovimientoBilletera>
{
    private static readonly Guid VendedorId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid Comprador1Id = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private static readonly Guid Comprador2Id = Guid.Parse("33333333-3333-3333-3333-333333333333");
    private static readonly Guid SinFondosId = Guid.Parse("44444444-4444-4444-4444-444444444444");

    // Fecha fija UTC compatible con DateTime
    private static readonly DateTime BaseFecha = new(2026, 9, 10, 12, 0, 0, DateTimeKind.Utc);

    public void Configure(EntityTypeBuilder<Categoria> builder)
    {
        builder.HasData(
            new Categoria { Id = 1, Nombre = "Tecnología" },
            new Categoria { Id = 2, Nombre = "Coleccionables" },
            new Categoria { Id = 3, Nombre = "Indumentaria" },
            new Categoria { Id = 4, Nombre = "Vehículos" }
        );
    }

    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.HasData(
            new Usuario
            {
                Id = VendedorId,
                NombreCompleto = "Usuario Vendedor",
                Email = "vendedor@test.com",
                PasswordHash = "HASH_SIMULADO",
                FechaRegistro = BaseFecha
            },
            new Usuario
            {
                Id = Comprador1Id,
                NombreCompleto = "Comprador Líder",
                Email = "comprador1@test.com",
                PasswordHash = "HASH_SIMULADO",
                FechaRegistro = BaseFecha
            },
            new Usuario
            {
                Id = Comprador2Id,
                NombreCompleto = "Comprador Habilitado",
                Email = "comprador2@test.com",
                PasswordHash = "HASH_SIMULADO",
                FechaRegistro = BaseFecha
            },
            new Usuario
            {
                Id = SinFondosId,
                NombreCompleto = "Usuario Sin Fondos",
                Email = "sinfondos@test.com",
                PasswordHash = "HASH_SIMULADO",
                FechaRegistro = BaseFecha
            }
        );
    }

    public void Configure(EntityTypeBuilder<Billetera> builder)
    {
        builder.HasData(
            new Billetera
            {
                Id = 1,
                UsuarioId = VendedorId,
                SaldoTotal = 0m,
                SaldoRetenido = 0m,
                Version = 1
            },
            new Billetera
            {
                Id = 2,
                UsuarioId = Comprador1Id,
                SaldoTotal = 150000m,
                SaldoRetenido = 45000m,
                Version = 1
            },
            new Billetera
            {
                Id = 3,
                UsuarioId = Comprador2Id,
                SaldoTotal = 200000m,
                SaldoRetenido = 0m,
                Version = 1
            },
            new Billetera
            {
                Id = 4,
                UsuarioId = SinFondosId,
                SaldoTotal = 500m,
                SaldoRetenido = 0m,
                Version = 1
            }
        );
    }

    public void Configure(EntityTypeBuilder<Subasta> builder)
    {
        builder.HasData(
            new Subasta
            {
                Id = 1,
                VendedorId = VendedorId,
                CategoriaId = 1,
                Titulo = "Notebook Gamer Pro",
                Descripcion = "Excelente estado, 16GB RAM",
                UrlImagen = "https://via.placeholder.com/150",
                PrecioBase = 30000m,
                IncrementoMinimo = 5000m,
                FechaInicio = BaseFecha.AddHours(-1),
                FechaFin = BaseFecha.AddMinutes(25),
                Estado = EstadoSubasta.Activa,
                Version = 1
            },
            new Subasta
            {
                Id = 2,
                VendedorId = VendedorId,
                CategoriaId = 2,
                Titulo = "Figura Coleccionable Rare",
                Descripcion = "Edición limitada año 1990",
                UrlImagen = "https://via.placeholder.com/150",
                PrecioBase = 10000m,
                IncrementoMinimo = 2000m,
                FechaInicio = BaseFecha.AddHours(-1),
                FechaFin = BaseFecha.AddSeconds(45),
                Estado = EstadoSubasta.Activa,
                Version = 1
            },
            new Subasta
            {
                Id = 3,
                VendedorId = VendedorId,
                CategoriaId = 4,
                Titulo = "Auto Deportivo 2020",
                Descripcion = "Pocos kilómetros",
                UrlImagen = "https://via.placeholder.com/150",
                PrecioBase = 1000000m,
                IncrementoMinimo = 50000m,
                FechaInicio = BaseFecha.AddDays(1),
                FechaFin = BaseFecha.AddDays(2),
                Estado = EstadoSubasta.Programada,
                Version = 1
            },
            new Subasta
            {
                Id = 4,
                VendedorId = VendedorId,
                CategoriaId = 1,
                Titulo = "Smartphone Ultima Generación",
                Descripcion = "Nuevo en caja sellada",
                UrlImagen = "https://via.placeholder.com/150",
                PrecioBase = 50000m,
                IncrementoMinimo = 5000m,
                FechaInicio = BaseFecha.AddHours(-5),
                FechaFin = BaseFecha.AddHours(-1),
                Estado = EstadoSubasta.Activa,
                Version = 1
            },
            new Subasta
            {
                Id = 5,
                VendedorId = VendedorId,
                CategoriaId = 3,
                Titulo = "Chaqueta de Cuero Vintage",
                Descripcion = "Talle L",
                UrlImagen = "https://via.placeholder.com/150",
                PrecioBase = 20000m,
                IncrementoMinimo = 2000m,
                FechaInicio = BaseFecha.AddHours(-5),
                FechaFin = BaseFecha.AddHours(-1),
                Estado = EstadoSubasta.Activa,
                Version = 1
            }
        );
    }

    public void Configure(EntityTypeBuilder<Puja> builder)
    {
        builder.HasData(
            new Puja
            {
                Id = 1,
                SubastaId = 1,
                PostorId = Comprador2Id,
                Monto = 35000m,
                Fecha = BaseFecha.AddMinutes(-40)
            },
            new Puja
            {
                Id = 2,
                SubastaId = 1,
                PostorId = Comprador1Id,
                Monto = 45000m,
                Fecha = BaseFecha.AddMinutes(-20)
            },
            new Puja
            {
                Id = 3,
                SubastaId = 4,
                PostorId = Comprador1Id,
                Monto = 60000m,
                Fecha = BaseFecha.AddHours(-2)
            }
        );
    }

    public void Configure(EntityTypeBuilder<MovimientoBilletera> builder)
    {
        builder.HasData(
            new MovimientoBilletera
            {
                Id = 1,
                BilleteraId = 2,
                SubastaId = null,
                Tipo = TipoMovimientoBilletera.Deposito,
                Monto = 150000m,
                OperacionId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                Fecha = BaseFecha.AddDays(-2)
            },
            new MovimientoBilletera
            {
                Id = 2,
                BilleteraId = 2,
                SubastaId = 1,
                Tipo = TipoMovimientoBilletera.Retencion,
                Monto = 45000m,
                OperacionId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                Fecha = BaseFecha.AddMinutes(-20)
            }
        );
    }
}