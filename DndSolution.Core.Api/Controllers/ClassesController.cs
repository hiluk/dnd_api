using Core.Api.Mappings;
using Core.Sdk.Dtos.Classes;
using DndSolution.Application.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Core.Api.Controllers;

/// <summary>
/// Справочник классов
/// </summary>
[ApiController]
[Route("classes")]
public class ClassesController : ControllerBase
{
    private readonly IClassService _service;

    /// <summary>
    /// .ctor
    /// </summary>
    /// <param name="service">Сервис классов</param>
    public ClassesController(IClassService service)
    {
        _service = service;
    }

    /// <summary>
    /// Сохранить класс
    /// </summary>
    /// <param name="dto">Класс</param>
    /// <param name="token">Токен отмены операции</param>
    [HttpPost("save-new")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> SaveClassAsync([FromBody] CharacterClassDto dto, CancellationToken token)
    {
        await _service.SaveClassAsync(dto.ToModel(), token);

        return NoContent();
    }

    /// <summary>
    /// Получить все классы
    /// </summary>
    /// <param name="token">Токен отмены операции</param>
    [HttpGet("")]
    [ProducesResponseType<List<CharacterClassDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllClassesAsync(CancellationToken token)
    {
        var models = await _service.GetAllClassesAsync(token);

        return Ok(models.Select(c => c.ToDto()).ToList());
    }
}
