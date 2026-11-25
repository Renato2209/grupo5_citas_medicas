namespace CitasMedicas.Api.Exceptions;

/// <summary>Recurso inexistente → HTTP 404.</summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}
