using CitasMedicas.Api.DTOs;
using CitasMedicas.Api.Models;
using CitasMedicas.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace CitasMedicas.Api.Controllers;

/// <summary>Consulta de médicos (datos de prueba) y de su agenda de citas.</summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class MedicosController : ControllerBase
{
    private readonly ICatalogoService _catalogo;
    private readonly ICitaService _citas;

    public MedicosController(ICatalogoService catalogo, ICitaService citas)
    {
        _catalogo = catalogo;
        _citas = citas;
    }

    /// <summary>Lista los médicos disponibles.</summary>
    /// <response code="200">Lista de médicos.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Medico>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<Medico>> Listar() => Ok(_catalogo.ObtenerMedicos());

    /// <summary>Obtiene un médico por su id.</summary>
    /// <response code="200">Médico encontrado.</response>
    /// <response code="404">El médico no existe.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Medico), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public ActionResult<Medico> ObtenerPorId(int id) => Ok(_catalogo.ObtenerMedico(id));

    /// <summary>Lista las citas de un médico (ordenadas por fecha).</summary>
    /// <response code="200">Agenda del médico (puede estar vacía).</response>
    /// <response code="404">El médico no existe.</response>
    [HttpGet("{id:int}/citas")]
    [ProducesResponseType(typeof(IEnumerable<CitaResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public ActionResult<IEnumerable<CitaResponseDto>> ObtenerCitas(int id) => Ok(_citas.ObtenerPorMedico(id));
}
