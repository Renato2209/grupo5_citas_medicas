using CitasMedicas.Api.DTOs;
using CitasMedicas.Api.Models;
using CitasMedicas.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace CitasMedicas.Api.Controllers;

/// <summary>Consulta de pacientes (datos de prueba) y de su historial de citas.</summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class PacientesController : ControllerBase
{
    private readonly ICatalogoService _catalogo;
    private readonly ICitaService _citas;

    public PacientesController(ICatalogoService catalogo, ICitaService citas)
    {
        _catalogo = catalogo;
        _citas = citas;
    }

    /// <summary>Lista los pacientes registrados.</summary>
    /// <response code="200">Lista de pacientes.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Paciente>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<Paciente>> Listar() => Ok(_catalogo.ObtenerPacientes());

    /// <summary>Obtiene un paciente por su id.</summary>
    /// <response code="200">Paciente encontrado.</response>
    /// <response code="404">El paciente no existe.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Paciente), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public ActionResult<Paciente> ObtenerPorId(int id) => Ok(_catalogo.ObtenerPaciente(id));

    /// <summary>Lista las citas de un paciente (ordenadas por fecha).</summary>
    /// <response code="200">Historial del paciente (puede estar vacío).</response>
    /// <response code="404">El paciente no existe.</response>
    [HttpGet("{id:int}/citas")]
    [ProducesResponseType(typeof(IEnumerable<CitaResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public ActionResult<IEnumerable<CitaResponseDto>> ObtenerCitas(int id) => Ok(_citas.ObtenerPorPaciente(id));
}
