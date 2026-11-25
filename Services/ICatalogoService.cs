using CitasMedicas.Api.Models;

namespace CitasMedicas.Api.Services;

/// <summary>Consulta de médicos y pacientes (datos de prueba en memoria).</summary>
public interface ICatalogoService
{
    IEnumerable<Medico> ObtenerMedicos();
    Medico ObtenerMedico(int id);
    IEnumerable<Paciente> ObtenerPacientes();
    Paciente ObtenerPaciente(int id);
}
