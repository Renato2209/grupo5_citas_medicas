using CitasMedicas.Api.DTOs;

namespace CitasMedicas.Api.Services;

public interface ICitaService
{
    IEnumerable<CitaResponseDto> ObtenerTodas(int? medicoId, DateTime? fecha);
    IEnumerable<CitaResponseDto> ObtenerPorMedico(int medicoId);
    IEnumerable<CitaResponseDto> ObtenerPorPaciente(int pacienteId);
    CitaResponseDto ObtenerPorId(int id);
    CitaResponseDto Crear(CitaCreateDto dto);
    CitaResponseDto Actualizar(int id, CitaUpdateDto dto);
    CitaResponseDto Cancelar(int id);
    void Eliminar(int id);
}
