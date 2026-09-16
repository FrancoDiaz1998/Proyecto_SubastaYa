using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SubastaYa.API.BackgroundServices;
using SubastaYa.API.ExceptionHandling;
using SubastaYa.Infrastructure;
using SubastaYa.Infrastructure.Persistencia;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddOpenApi();
builder.Services.AgregarInfraestructura(builder.Configuration);
builder.Services.AddHostedService<SubastasFinalizacionWorker>();

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("No se configuró 'Jwt:Key'.");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "SubastaYa.Api";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "SubastaYa.Frontend";

if (Encoding.UTF8.GetByteCount(jwtKey) < 32)
    throw new InvalidOperationException("La clave JWT debe tener al menos 32 caracteres.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30),
            NameClaimType = JwtRegisteredClaimNames.Name
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        var (title, defaultDetail) = context.ProblemDetails.Status switch
        {
            StatusCodes.Status400BadRequest => ("Solicitud inválida", "Uno o más datos de la solicitud no son válidos."),
            StatusCodes.Status401Unauthorized => ("No autenticado", "Se requiere un token de acceso válido."),
            StatusCodes.Status403Forbidden => ("Acceso denegado", "No tenés permisos suficientes para realizar esta operación."),
            StatusCodes.Status404NotFound => ("Recurso no encontrado", "No se encontró el recurso solicitado."),
            StatusCodes.Status405MethodNotAllowed => ("Método no permitido", "El método HTTP utilizado no está permitido para este recurso."),
            _ => (context.ProblemDetails.Title, context.ProblemDetails.Detail)
        };

        context.ProblemDetails.Title = title;
        context.ProblemDetails.Detail ??= defaultDetail;
        context.ProblemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;
    };
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var app = builder.Build();

app.UseExceptionHandler();

app.Use(async (context, next) =>
{
    context.Response.OnStarting(() =>
    {
        context.Response.Headers["X-Api-version"] = "1.0";
        return Task.CompletedTask;
    });

    await next();
});

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    using var scope = app.Services.CreateScope();
    await DbInitializer.InicializarAsync(scope.ServiceProvider);
}

app.UseHttpsRedirection();
app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
