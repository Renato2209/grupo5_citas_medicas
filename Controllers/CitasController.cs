using CitasMedicas.Api.DTOs;
using CitasMedicas.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace CitasMedicas.Api.Controllers;

/// <summary>Gestión de citas médicas (CRUD y cancelación).</summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CitasController : ControllerBase
{
    private readonly ICitaService _service;

    public CitasController(ICitaService service) => _service = service;

    /// <summary>Lista las citas, con filtros opcionales por médico y fecha.</summary>
    /// <param name="medicoId">Id del médico (opcional).</param>
    /// <param name="fecha">Fecha en formato yyyy-MM-dd (opcional).</param>
    /// <response code="200">Lista de citas (puede estar vacía).</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CitaResponseDto>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<CitaResponseDto>> Listar([FromQuery] int? medicoId, [FromQuery] DateTime? fecha)
        => Ok(_service.ObtenerTodas(medicoId, fecha));

    /// <summary>Obtiene una cita por su id.</summary>
    /// <response code="200">Cita encontrada.</response>
    /// <response code="404">La cita no existe.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CitaResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public ActionResult<CitaResponseDto> ObtenerPorId(int id) => Ok(_service.ObtenerPorId(id));

    /// <summary>Registra una nueva cita.</summary>
    /// <response code="201">Cita creada. El header Location apunta al nuevo recurso.</response>
    /// <response code="400">Datos de entrada inválidos.</response>
    /// <response code="404">Paciente o médico inexistente.</response>
    /// <response code="409">El médico ya tiene una cita en ese horario.</response>
    [HttpPost]
    [ProducesResponseType(typeof(CitaResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public ActionResult<CitaResponseDto> Crear([FromBody] CitaCreateDto dto)
    {
        var creada = _service.Crear(dto);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = creada.Id }, creada);
    }

    /// <summary>Actualiza una cita existente (reprogramar, cambiar médico o estado).</summary>
    /// <response code="200">Cita actualizada.</response>
    /// <response code="400">Datos inválidos o cita no modificable.</response>
    /// <response code="404">Cita, o médico, inexistente.</response>
    /// <response code="409">Horario ocupado.</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(CitaResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public ActionResult<CitaResponseDto> Actualizar(int id, [FromBody] CitaUpdateDto dto)
        => Ok(_service.Actualizar(id, dto));

    /// <summary>Cancela una cita (cambia su estado a Cancelada, sin borrarla del registro).</summary>
    /// <response code="200">Cita cancelada.</response>
    /// <response code="400">La cita ya estaba cancelada o ya fue atendida.</response>
    /// <response code="404">La cita no existe.</response>
    [HttpPatch("{id:int}/cancelar")]
    [ProducesResponseType(typeof(CitaResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public ActionResult<CitaResponseDto> Cancelar(int id) => Ok(_service.Cancelar(id));

    /// <summary>Elimina definitivamente una cita del registro.</summary>
    /// <response code="204">Cita eliminada.</response>
    /// <response code="404">La cita no existe.</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult Eliminar(int id)
    {
        _service.Eliminar(id);
        return NoContent();
    }
}
