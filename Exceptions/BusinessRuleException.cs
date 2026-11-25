namespace CitasMedicas.Api.Exceptions;

/// <summary>Regla de negocio incumplida → HTTP 400.</summary>
public class BusinessRuleException : Exception
{
    public BusinessRuleException(string message) : base(message) { }
}
