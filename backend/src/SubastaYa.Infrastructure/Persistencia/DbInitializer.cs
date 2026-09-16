using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SubastaYa.Application.Interfaces;
using SubastaYa.Domain.Entidades;

namespace SubastaYa.Infrastructure.Persistencia;

public static class DbInitializer
{
    public static async Task InicializarAsync(
        IServiceProvider services, CancellationToken cancellationToken = default)
    {
        var dbContext = services.GetRequiredService<SubastaYaDbContext>();
        var passwordHasher = services.GetRequiredService<IPasswordHasher>();

        var vendedorId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var comprador1Id = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var comprador2Id = Guid.Parse("33333333-3333-3333-3333-333333333333");
        var sinFondosId = Guid.Parse("44444444-4444-4444-4444-444444444444");
        const string passwordDemo = "Demo123!";

        var usuarioExistente = await dbContext.Usuarios
            .FirstOrDefaultAsync(usuario => usuario.Id == vendedorId, cancellationToken);

        if (usuarioExistente is not null)
        {
            var ids = new[] { vendedorId, comprador1Id, comprador2Id, sinFondosId };
            var usuariosSemilla = await dbContext.Usuarios
                .Where(usuario => ids.Contains(usuario.Id))
                .ToListAsync(cancellationToken);

            foreach (var usuario in usuariosSemilla)
            {
                if (usuario.PasswordHash == "seed")
                    usuario.PasswordHash = passwordHasher.HashPassword(usuario, passwordDemo);
            }

            await AsegurarLedgerSemillaAsync(dbContext, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            return;
        }

        await using var transaccion = await dbContext.Database
            .BeginTransactionAsync(cancellationToken);

        try
        {
            var ahora = DateTimeOffset.UtcNow;

            Usuario CrearUsuario(Guid id, string nombre, string email)
            {
                var usuario = new Usuario
                {
                    Id = id,
                    Nombre = nombre,
                    Email = email,
                    PasswordHash = string.Empty
                };

                usuario.PasswordHash = passwordHasher.HashPassword(usuario, passwordDemo);
                return usuario;
            }

            var usuarios = new[]
            {
                CrearUsuario(vendedorId, "Vendedor Demo", "vendedor@test.com"),
                CrearUsuario(comprador1Id, "Comprador 1", "comprador1@test.com"),
                CrearUsuario(comprador2Id, "Comprador 2", "comprador2@test.com"),
                CrearUsuario(sinFondosId, "Sin Fondos", "sinfondos@test.com")
            };

            var categorias = new[]
            {
                new Categoria { Id = -1, Nombre = "Tecnología", UrlIcono = "Laptop" },
                new Categoria { Id = -2, Nombre = "Coleccionables", UrlIcono = "Sparkles" },
                new Categoria { Id = -3, Nombre = "Indumentaria", UrlIcono = "Shirt" },
                new Categoria { Id = -4, Nombre = "Vehículos", UrlIcono = "Car" }
            };

            var billeteras = new[]
            {
                new Billetera { Id = -1, UsuarioId = vendedorId, SaldoTotal = 0, SaldoRetenido = 0 },
                new Billetera { Id = -2, UsuarioId = comprador1Id, SaldoTotal = 150000, SaldoRetenido = 45000 },
                new Billetera { Id = -3, UsuarioId = comprador2Id, SaldoTotal = 200000, SaldoRetenido = 0 },
                new Billetera { Id = -4, UsuarioId = sinFondosId, SaldoTotal = 500, SaldoRetenido = 0 }
            };

            var subastas = new[]
            {
                new Subasta
                {
                    Id = -1, VendedorId = vendedorId, CategoriaId = -1,
                    Titulo = "MacBook Pro 16",
                    Descripcion = "Notebook profesional en excelente estado con cargador original.",
                    UrlImagen = "https://images.unsplash.com/photo-1517336714731-489689fd1ca8?auto=format&fit=crop&w=1000&q=80",
                    PrecioBase = 35000, IncrementoMinimo = 2000,
                    FechaInicio = ahora.AddHours(-2), FechaFin = ahora.AddMinutes(25),
                    Estado = EstadoSubasta.Activa
                },
                new Subasta
                {
                    Id = -2, VendedorId = vendedorId, CategoriaId = -2,
                    Titulo = "PlayStation 5 Edición Limitada",
                    Descripcion = "Consola coleccionable sellada con accesorios incluidos.",
                    UrlImagen = "https://images.unsplash.com/photo-1606813907291-d86efa9b94db?auto=format&fit=crop&w=1000&q=80",
                    PrecioBase = 60000, IncrementoMinimo = 3000,
                    FechaInicio = ahora.AddHours(-2), FechaFin = ahora.AddSeconds(90),
                    Estado = EstadoSubasta.Activa
                },
                new Subasta
                {
                    Id = -3, VendedorId = vendedorId, CategoriaId = -4,
                    Titulo = "BMW M3 E46 2004 Manual",
                    Descripcion = "Vehículo clásico con transmisión manual e historial de servicio.",
                    UrlImagen = "https://images.unsplash.com/photo-1555215695-3004980ad54e?auto=format&fit=crop&w=1000&q=80",
                    PrecioBase = 1250000, IncrementoMinimo = 50000,
                    FechaInicio = ahora.AddHours(24), FechaFin = ahora.AddHours(72),
                    Estado = EstadoSubasta.Programada
                },
                new Subasta
                {
                    Id = -4, VendedorId = vendedorId, CategoriaId = -3,
                    Titulo = "Campera de cuero edición vintage",
                    Descripcion = "Campera de cuero en excelente estado, talle M.",
                    UrlImagen = "https://images.unsplash.com/photo-1551028719-00167b16eac5?auto=format&fit=crop&w=1000&q=80",
                    PrecioBase = 30000, IncrementoMinimo = 5000,
                    FechaInicio = ahora.AddDays(-2), FechaFin = ahora.AddHours(-4),
                    Estado = EstadoSubasta.Activa
                },
                new Subasta
                {
                    Id = -5, VendedorId = vendedorId, CategoriaId = -2,
                    Titulo = "Colección de monedas antiguas",
                    Descripcion = "Lote de monedas coleccionables de distintas épocas.",
                    UrlImagen = "https://images.unsplash.com/photo-1605792657660-596af9009e82?auto=format&fit=crop&w=1000&q=80",
                    PrecioBase = 25000, IncrementoMinimo = 2500,
                    FechaInicio = ahora.AddDays(-2), FechaFin = ahora.AddHours(-12),
                    Estado = EstadoSubasta.Activa
                }
            };

            var pujas = new[]
            {
                new Puja { Id = -101, SubastaId = -1, PostorId = comprador2Id, Monto = 39000, Fecha = ahora.AddMinutes(-50) },
                new Puja { Id = -102, SubastaId = -1, PostorId = comprador1Id, Monto = 45000, Fecha = ahora.AddMinutes(-15) },
                new Puja { Id = -401, SubastaId = -4, PostorId = comprador2Id, Monto = 50000, Fecha = ahora.AddHours(-5) }
            };

            var movimientos = CrearLedgerSemilla(ahora);

            dbContext.Usuarios.AddRange(usuarios);
            dbContext.Categorias.AddRange(categorias);
            dbContext.Billeteras.AddRange(billeteras);
            dbContext.Subastas.AddRange(subastas);
            dbContext.Pujas.AddRange(pujas);
            dbContext.Set<MovimientoBilletera>().AddRange(movimientos);

            await dbContext.SaveChangesAsync(cancellationToken);
            await transaccion.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaccion.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private static MovimientoBilletera[] CrearLedgerSemilla(DateTimeOffset ahora) =>
    [
        new MovimientoBilletera
        {
            Id = -1001, BilleteraId = -2, Tipo = TipoMovimientoBilletera.Deposito,
            Monto = 150000,
            OperacionId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaa1001"),
            Fecha = ahora.AddHours(-3)
        },
        new MovimientoBilletera
        {
            Id = -1002, BilleteraId = -2, SubastaId = -1,
            Tipo = TipoMovimientoBilletera.Retencion, Monto = 45000,
            OperacionId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaa1002"),
            Fecha = ahora.AddMinutes(-15)
        },
        new MovimientoBilletera
        {
            Id = -1003, BilleteraId = -3, Tipo = TipoMovimientoBilletera.Deposito,
            Monto = 200000,
            OperacionId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaa1003"),
            Fecha = ahora.AddHours(-3)
        },
        new MovimientoBilletera
        {
            Id = -1004, BilleteraId = -4, Tipo = TipoMovimientoBilletera.Deposito,
            Monto = 500,
            OperacionId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaa1004"),
            Fecha = ahora.AddHours(-3)
        }
    ];

    private static async Task AsegurarLedgerSemillaAsync(
        SubastaYaDbContext dbContext, CancellationToken cancellationToken)
    {
        var ids = new long[] { -1001, -1002, -1003, -1004 };
        var existentes = await dbContext.Set<MovimientoBilletera>()
            .AsNoTracking()
            .Where(movimiento => ids.Contains(movimiento.Id))
            .Select(movimiento => movimiento.Id)
            .ToListAsync(cancellationToken);

        var existentesSet = existentes.ToHashSet();
        var ahora = DateTimeOffset.UtcNow;
        var faltantes = CrearLedgerSemilla(ahora)
            .Where(movimiento => !existentesSet.Contains(movimiento.Id))
            .ToList();

        if (faltantes.Count > 0)
            dbContext.Set<MovimientoBilletera>().AddRange(faltantes);
    }
}
