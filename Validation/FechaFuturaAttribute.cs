using System.ComponentModel.DataAnnotations;

namespace CitasMedicas.Api.Validation;

/// <summary>Validación personalizada: la fecha de la cita debe ser posterior al momento actual.</summary>
[AttributeUsage(AttributeTargets.Property)]
public class FechaFuturaAttribute : ValidationAttribute
{
    public FechaFuturaAttribute() : base("La fecha y hora de la cita debe ser futura.") { }

    public override bool IsValid(object? value) =>
        value is DateTime fecha && fecha > DateTime.Now;
}
