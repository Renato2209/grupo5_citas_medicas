using CitasMedicas.Api.Exceptions;
using CitasMedicas.Api.Models;

namespace CitasMedicas.Api.Services;

/// <summary>Almacena en listas los pacientes y médicos precargados para las pruebas.</summary>
public class CatalogoService : ICatalogoService
{
    private readonly List<Paciente> _pacientes = new()
    {
        new(1, "Ana Torres", "45871236", 28),
        new(2, "Luis Ramírez", "70123456", 35),
        new(3, "Carla Mendoza", "41236987", 42)
    };

    private readonly List<Medico> _medicos = new()
    {
        new(1, "Dr. Jorge Salas", "Medicina General", "CMP-54120"),
        new(2, "Dra. María Quispe", "Pediatría", "CMP-48912"),
        new(3, "Dr. Pedro Vega", "Cardiología", "CMP-50377")
    };

    public IEnumerable<Medico> ObtenerMedicos() => _medicos;

    public Medico ObtenerMedico(int id) =>
        _medicos.FirstOrDefault(m => m.Id == id)
        ?? throw new NotFoundException($"No existe un médico con id {id}.");

    public IEnumerable<Paciente> ObtenerPacientes() => _pacientes;

    public Paciente ObtenerPaciente(int id) =>
        _pacientes.FirstOrDefault(p => p.Id == id)
        ?? throw new NotFoundException($"No existe un paciente con id {id}.");
}
