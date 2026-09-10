using Microsoft.AspNetCore.Identity;
using SubastaYa.Application.Interfaces;
using SubastaYa.Application.Servicios;
using SubastaYa.Infrastructure;
using SubastaYa.Infrastructure.Identidad;
using SubastaYa.Infrastructure.Persistencia;
using SubastaYa.Infrastructure.Persistencia.Repositorios;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AgregarInfraestructura(builder.Configuration);
builder.Services.AddAuthorization();
builder.Services.AddScoped<ISubastaRepository, SubastaRepository>();
builder.Services.AddScoped<IBilleteraRepository, BilleteraRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<PujaService>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
