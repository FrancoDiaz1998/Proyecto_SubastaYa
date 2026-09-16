using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SubastaYa.Application.Interfaces;
using SubastaYa.Application.UseCases.Actividades.ListarMisPublicaciones;
using SubastaYa.Application.UseCases.Actividades.ListarMisPujas;
using SubastaYa.Application.UseCases.Autenticacion.Login;
using SubastaYa.Application.UseCases.Billeteras.AcreditarSaldo;
using SubastaYa.Application.UseCases.Billeteras.ListarMovimientos;
using SubastaYa.Application.UseCases.Billeteras.ObtenerSaldo;
using SubastaYa.Application.UseCases.Categorias.ListarCategorias;
using SubastaYa.Application.UseCases.Subastas.CrearSubasta;
using SubastaYa.Application.UseCases.Subastas.FinalizarVencidas;
using SubastaYa.Application.UseCases.Subastas.ListarSubastas;
using SubastaYa.Application.UseCases.Subastas.ObtenerSubasta;
using SubastaYa.Application.UseCases.Subastas.RegistrarPuja;
using SubastaYa.Domain.Interfaces;
using SubastaYa.Infrastructure.Identidad;
using SubastaYa.Infrastructure.Persistencia;
using SubastaYa.Infrastructure.Persistencia.Repositories;

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

        servicios.AddScoped<ISubastaRepository, SubastaRepository>();
        servicios.AddScoped<ICategoriaRepository, CategoriaRepository>();
        servicios.AddScoped<IUsuarioRepository, UsuarioRepository>();
        servicios.AddScoped<IPujaRepository, PujaRepository>();
        servicios.AddScoped<IBilleteraRepository, BilleteraRepository>();
        servicios.AddScoped<IActividadRepository, ActividadRepository>();
        servicios.AddScoped<IFinalizacionSubastasRepository, FinalizacionSubastasRepository>();
        servicios.AddScoped<IUnidadDeTrabajo, UnidadDeTrabajo>();

        servicios.AddScoped<IPasswordHasher, PasswordHasher>();
        servicios.AddScoped<IJwtService, JwtService>();

        servicios.AddScoped<LoginUseCase>();
        servicios.AddScoped<ListarCategoriasUseCase>();
        servicios.AddScoped<ListarSubastasUseCase>();
        servicios.AddScoped<ObtenerSubastaUseCase>();
        servicios.AddScoped<CrearSubastaUseCase>();
        servicios.AddScoped<RegistrarPujaUseCase>();
        servicios.AddScoped<ObtenerSaldoBilleteraUseCase>();
        servicios.AddScoped<ListarMovimientosBilleteraUseCase>();
        servicios.AddScoped<AcreditarSaldoUseCase>();
        servicios.AddScoped<ListarMisPujasUseCase>();
        servicios.AddScoped<ListarMisPublicacionesUseCase>();
        servicios.AddScoped<FinalizarSubastasVencidasUseCase>();

        return servicios;
    }
}
