using Core.Api.Mappings;
using Core.Sdk.Dtos.Race;
using DndSolution.Application.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Core.Api.Controllers;

/// <summary>
/// Справочник рас
/// </summary>
[ApiController]
[Route("races")]
public class RacesController : ControllerBase
{
    private readonly IRaceService _service;

    /// <summary>
    /// .ctor
    /// </summary>
    /// <param name="service">Сервис рас</param>
    public RacesController(IRaceService service)
    {
        _service = service;
    }

    /// <summary>
    /// Сохранить расу
    /// </summary>
    /// <param name="dto">Раса</param>
    /// <param name="token">Токен отмены операции</param>
    [HttpPost("save-new")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> SaveRace([FromBody] RaceDto dto, CancellationToken token)
    {
        await _service.SaveRaceAsync(RaceModelMapper.MapToModel(dto), token);

        return NoContent();
    }

    /// <summary>
    /// Получить все расы
    /// </summary>
    /// <param name="token">Токен отмены операции</param>
    [HttpGet("")]
    [ProducesResponseType<List<RaceDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllRacesAsync(CancellationToken token)
    {
        var races = await _service.GetAllRacesAsync(token);

        return Ok(races.Select(RaceModelMapper.MapToDto).ToList());
    }
}
