namespace CitasMedicas.Api.Exceptions;

/// <summary>Conflicto con el estado actual (ej. horario ocupado) → HTTP 409.</summary>
public class ConflictException : Exception
{
    public ConflictException(string message) : base(message) { }
}
