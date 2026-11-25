using System.ComponentModel.DataAnnotations;
using CitasMedicas.Api.Models;
using CitasMedicas.Api.Validation;

namespace CitasMedicas.Api.DTOs;

public class CitaUpdateDto
{
    [Required(ErrorMessage = "El médico es obligatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "MedicoId debe ser mayor a 0.")]
    public int MedicoId { get; set; }

    [Required(ErrorMessage = "La fecha y hora son obligatorias.")]
    [FechaFutura]
    public DateTime? FechaHora { get; set; }

    [Required(ErrorMessage = "El motivo es obligatorio.")]
    [StringLength(200, MinimumLength = 5, ErrorMessage = "El motivo debe tener entre 5 y 200 caracteres.")]
    public string Motivo { get; set; } = string.Empty;

    [EnumDataType(typeof(EstadoCita), ErrorMessage = "Estado no válido.")]
    public EstadoCita Estado { get; set; } = EstadoCita.Pendiente;
}
