using Microsoft.EntityFrameworkCore;
using SubastaYa.Domain.Entidades;

namespace SubastaYa.Infrastructure.Persistencia;

public class SubastaYaDbContext : DbContext
{
    public SubastaYaDbContext(
        DbContextOptions<SubastaYaDbContext> options): base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();

    public DbSet<Categoria> Categorias => Set<Categoria>();

    public DbSet<Subasta> Subastas => Set<Subasta>();

    public DbSet<Puja> Pujas => Set<Puja>();

    public DbSet<Billetera> Billeteras => Set<Billetera>();

    public DbSet<MovimientoBilletera> MovimientosBilletera
        => Set<MovimientoBilletera>();

    public DbSet<Venta> Ventas => Set<Venta>();

    public DbSet<RegistroAuditoria> RegistrosAuditoria
        => Set<RegistroAuditoria>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(SubastaYaDbContext).Assembly);
    }
}