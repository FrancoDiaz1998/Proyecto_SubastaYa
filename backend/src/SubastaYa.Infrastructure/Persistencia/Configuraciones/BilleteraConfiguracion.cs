using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubastaYa.Domain.Entidades;
using SubastaYa.Infrastructure.Identidad;

namespace SubastaYa.Infrastructure.Persistencia.Configuraciones;

public class BilleteraConfiguracion : IEntityTypeConfiguration<Billetera>
{
    public void Configure(EntityTypeBuilder<Billetera> builder)
    {
        builder.ToTable("billeteras", tabla =>
            tabla.HasCheckConstraint(
                "ck_billeteras_saldos",
                "saldo_total >= 0 AND saldo_retenido >= 0 AND saldo_retenido <= saldo_total"));

        builder.HasKey(billetera => billetera.Id);

        builder.Property(billetera => billetera.Id)
            .HasColumnName("id");

        builder.Property(billetera => billetera.UsuarioId)
            .HasColumnName("usuario_id");

        builder.Property(billetera => billetera.SaldoTotal)
            .HasColumnName("saldo_total")
            .HasPrecision(18, 2);

        builder.Property(billetera => billetera.SaldoRetenido)
            .HasColumnName("saldo_retenido")
            .HasPrecision(18, 2);

        builder.Property(billetera => billetera.Version)
            .HasColumnName("version")
            .HasDefaultValue(0L)
            .IsConcurrencyToken();

        builder.HasOne<Usuario>()
            .WithOne()
            .HasForeignKey<Billetera>(billetera => billetera.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(billetera => billetera.UsuarioId)
            .IsUnique()
            .HasDatabaseName("ux_billeteras_usuario_id");
    }
}
