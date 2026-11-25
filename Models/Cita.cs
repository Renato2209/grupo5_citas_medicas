namespace CitasMedicas.Api.Models;

public class Cita
{
    public int Id { get; }
    public int PacienteId { get; private set; }
    public int MedicoId { get; private set; }
    public DateTime FechaHora { get; private set; }
    public string Motivo { get; private set; }
    public EstadoCita Estado { get; private set; }

    public Cita(int id, int pacienteId, int medicoId, DateTime fechaHora, string motivo)
    {
        Id = id;
        PacienteId = pacienteId;
        MedicoId = medicoId;
        FechaHora = fechaHora;
        Motivo = motivo;
        Estado = EstadoCita.Pendiente;
    }

    public void Actualizar(int medicoId, DateTime fechaHora, string motivo, EstadoCita estado)
    {
        MedicoId = medicoId;
        FechaHora = fechaHora;
        Motivo = motivo;
        Estado = estado;
    }

    public void Cancelar() => Estado = EstadoCita.Cancelada;
}
