using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SubastaYa.Infrastructure.Persistencia;
using SubastaYa.Application.Interfaces;
using SubastaYa.Infrastructure.Identidad;

namespace SubastaYa.Infrastructure;

public static class InyeccionDependencias
{
    public static IServiceCollection AgregarInfraestructura(
        this IServiceCollection servicios, IConfiguration configuracion)
    {
        var cadenaConexion = configuracion.GetConnectionString("SubastaYa")
            ?? throw new InvalidOperationException(
                "No se encontró la cadena de conexión 'SubastaYa'.");

        servicios.AddDbContext<SubastaYaDbContext>(opciones =>
            opciones.UseNpgsql(cadenaConexion));

        servicios.AddScoped<IPasswordHasher, PasswordHasher>();    

        return servicios;
    }
}
