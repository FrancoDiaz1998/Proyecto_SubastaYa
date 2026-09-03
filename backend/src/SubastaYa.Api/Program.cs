using Microsoft.AspNetCore.Identity;
using SubastaYa.Infrastructure;
using SubastaYa.Infrastructure.Identidad;
using SubastaYa.Infrastructure.Persistencia;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AgregarInfraestructura(builder.Configuration);
builder.Services.AddAuthorization();

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
