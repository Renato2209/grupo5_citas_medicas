using CitasMedicas.Api.DTOs;
using CitasMedicas.Api.Exceptions;
using CitasMedicas.Api.Models;

namespace CitasMedicas.Api.Services;

/// <summary>Lógica de negocio de las citas. Simula la base de datos con una lista en memoria.</summary>
public class CitaService : ICitaService
{
    private static readonly TimeSpan DuracionCita = TimeSpan.FromMinutes(30);

    private readonly ICatalogoService _catalogo;
    private readonly List<Cita> _citas = new();
    private int _siguienteId = 1;
    private readonly object _lock = new(); // la API atiende peticiones en paralelo

    public CitaService(ICatalogoService catalogo) => _catalogo = catalogo;

    public IEnumerable<CitaResponseDto> ObtenerTodas(int? medicoId, DateTime? fecha)
    {
        lock (_lock)
        {
            return _citas
                .Where(c => medicoId is null || c.MedicoId == medicoId)
                .Where(c => fecha is null || c.FechaHora.Date == fecha.Value.Date)
                .OrderBy(c => c.FechaHora)
                .Select(Mapear)
                .ToList();
        }
    }

    public IEnumerable<CitaResponseDto> ObtenerPorMedico(int medicoId)
    {
        _catalogo.ObtenerMedico(medicoId); // lanza 404 si el médico no existe
        return ObtenerTodas(medicoId, null);
    }

    public IEnumerable<CitaResponseDto> ObtenerPorPaciente(int pacienteId)
    {
        _catalogo.ObtenerPaciente(pacienteId); // lanza 404 si el paciente no existe
        lock (_lock)
        {
            return _citas
                .Where(c => c.PacienteId == pacienteId)
                .OrderBy(c => c.FechaHora)
                .Select(Mapear)
                .ToList();
        }
    }

    public CitaResponseDto ObtenerPorId(int id)
    {
        lock (_lock) return Mapear(BuscarCita(id));
    }

    public CitaResponseDto Crear(CitaCreateDto dto)
    {
        lock (_lock)
        {
            _catalogo.ObtenerPaciente(dto.PacienteId);
            _catalogo.ObtenerMedico(dto.MedicoId);
            ValidarDisponibilidad(dto.MedicoId, dto.FechaHora!.Value, null);

            var cita = new Cita(_siguienteId++, dto.PacienteId, dto.MedicoId, dto.FechaHora.Value, dto.Motivo.Trim());
            _citas.Add(cita);
            return Mapear(cita);
        }
    }

    public CitaResponseDto Actualizar(int id, CitaUpdateDto dto)
    {
        lock (_lock)
        {
            var cita = BuscarCita(id);

            if (cita.Estado is EstadoCita.Atendida or EstadoCita.Cancelada)
                throw new BusinessRuleException($"No se puede modificar una cita en estado {cita.Estado}.");

            _catalogo.ObtenerMedico(dto.MedicoId);
            if (dto.Estado != EstadoCita.Cancelada)
                ValidarDisponibilidad(dto.MedicoId, dto.FechaHora!.Value, id);

            cita.Actualizar(dto.MedicoId, dto.FechaHora!.Value, dto.Motivo.Trim(), dto.Estado);
            return Mapear(cita);
        }
    }

    public CitaResponseDto Cancelar(int id)
    {
        lock (_lock)
        {
            var cita = BuscarCita(id);

            if (cita.Estado == EstadoCita.Atendida)
                throw new BusinessRuleException("No se puede cancelar una cita que ya fue atendida.");
            if (cita.Estado == EstadoCita.Cancelada)
                throw new BusinessRuleException("La cita ya se encuentra cancelada.");

            cita.Cancelar();
            return Mapear(cita);
        }
    }

    public void Eliminar(int id)
    {
        lock (_lock) _citas.Remove(BuscarCita(id));
    }

    // ---------- helpers privados ----------

    private Cita BuscarCita(int id) =>
        _citas.FirstOrDefault(c => c.Id == id)
        ?? throw new NotFoundException($"No existe una cita con id {id}.");

    private void ValidarDisponibilidad(int medicoId, DateTime fechaHora, int? ignorarCitaId)
    {
        bool ocupado = _citas.Any(c =>
            c.Id != ignorarCitaId &&
            c.MedicoId == medicoId &&
            c.Estado != EstadoCita.Cancelada &&
            Math.Abs((c.FechaHora - fechaHora).TotalMinutes) < DuracionCita.TotalMinutes);

        if (ocupado)
            throw new ConflictException($"El médico {medicoId} ya tiene una cita en ese horario.");
    }

    private CitaResponseDto Mapear(Cita c)
    {
        var paciente = _catalogo.ObtenerPaciente(c.PacienteId);
        var medico = _catalogo.ObtenerMedico(c.MedicoId);
        return new CitaResponseDto
        {
            Id = c.Id,
            PacienteId = paciente.Id,
            Paciente = paciente.Nombre,
            MedicoId = medico.Id,
            Medico = medico.Nombre,
            Especialidad = medico.Especialidad,
            FechaHora = c.FechaHora,
            Motivo = c.Motivo,
            Estado = c.Estado.ToString()
        };
    }
}
