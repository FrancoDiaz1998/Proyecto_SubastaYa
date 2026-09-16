using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SubastaYa.Application.Common.Exceptions;

namespace SubastaYa.API.ExceptionHandling;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title, detail) = exception switch
        {
            CredencialesInvalidasException error => (
                StatusCodes.Status401Unauthorized,
                "Credenciales inválidas",
                error.Message),

            CuentaNoDisponibleException error => (
                StatusCodes.Status403Forbidden,
                "Cuenta no disponible",
                error.Message),

            RecursoDuplicadoException error => (
                StatusCodes.Status409Conflict,
                "El recurso ya existe",
                error.Message),

            PujaRechazadaException error => (
                StatusCodes.Status409Conflict,
                "Puja rechazada",
                error.Message),

            ConflictoConcurrenciaException error => (
                StatusCodes.Status409Conflict,
                "Conflicto de concurrencia",
                error.Message),

            RecursoNoEncontradoException error => (
                StatusCodes.Status404NotFound,
                "Recurso no encontrado",
                error.Message),

            ArgumentException error => (
                StatusCodes.Status400BadRequest,
                "Solicitud inválida",
                error.Message),

            _ => (
                StatusCodes.Status500InternalServerError,
                "Error interno del servidor",
                "Ocurrió un error inesperado.")
        };

        if (statusCode >= 500)
        {
            _logger.LogError(
                exception,
                "Error inesperado procesando {Method} {Path}. TraceId: {TraceId}",
                httpContext.Request.Method,
                httpContext.Request.Path,
                httpContext.TraceIdentifier);
        }
        else
        {
            _logger.LogInformation(
                "Solicitud rechazada por {ExceptionType}: {Message}. TraceId: {TraceId}",
                exception.GetType().Name,
                exception.Message,
                httpContext.TraceIdentifier);
        }

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path
        };

        problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/problem+json";
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }
}
