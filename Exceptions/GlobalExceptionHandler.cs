using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CitasMedicas.Api.Exceptions;

/// <summary>
/// Manejo centralizado de excepciones. Toda excepción no controlada en controladores
/// o servicios termina aquí y se convierte en una respuesta JSON uniforme (ProblemDetails).
/// </summary>
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) => _logger = logger;

    public async ValueTask<bool> TryHandleAsync(
        HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        var (status, title, detail) = exception switch
        {
            NotFoundException e     => (StatusCodes.Status404NotFound, "Recurso no encontrado", e.Message),
            ConflictException e     => (StatusCodes.Status409Conflict, "Conflicto de horario", e.Message),
            BusinessRuleException e => (StatusCodes.Status400BadRequest, "Regla de negocio incumplida", e.Message),
            _                       => (StatusCodes.Status500InternalServerError, "Error interno del servidor",
                                        "Ocurrió un error inesperado. Intente nuevamente más tarde.")
        };

        if (status == StatusCodes.Status500InternalServerError)
            _logger.LogError(exception, "Error no controlado");
        else
            _logger.LogWarning("{Titulo}: {Detalle}", title, detail);

        var problem = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };

        context.Response.StatusCode = status;
        await context.Response.WriteAsJsonAsync(problem, cancellationToken);
        return true;
    }
}
