namespace CitasMedicas.Api.DTOs;

public class CitaResponseDto
{
    public int Id { get; set; }
    public int PacienteId { get; set; }
    public string Paciente { get; set; } = string.Empty;
    public int MedicoId { get; set; }
    public string Medico { get; set; } = string.Empty;
    public string Especialidad { get; set; } = string.Empty;
    public DateTime FechaHora { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
}
