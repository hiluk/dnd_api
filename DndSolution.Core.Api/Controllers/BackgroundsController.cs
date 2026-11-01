using Core.Sdk.Dtos;
using Core.Sdk.Enums;
using Data.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Core.Api.Controllers;

/// <summary>
/// Справочник предысторий
/// </summary>
[ApiController]
[Route("backgrounds")]
public class BackgroundsController : ControllerBase
{
    private readonly IBackgroundsRepository _repository;

    /// <summary>
    /// .ctor
    /// </summary>
    public BackgroundsController(IBackgroundsRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Получить все предыстории
    /// </summary>
    [HttpGet("")]
    [ProducesResponseType<List<BackgroundDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken token)
    {
        var backgrounds = await _repository.GetAllAsync(token);

        return Ok(backgrounds.Select(b => new BackgroundDto
        {
            Type = (BackgroundTypeDto)b.Type,
            Name = b.Name,
            Description = b.Description,
            Skills = b.Skills.Select(s => (SkillDto)s).ToList(),
            SkillChoices = b.SkillChoices,
            Tools = b.Tools,
            Languages = b.Languages,
            Equipment = b.Equipment,
            Feature = b.Feature,
        }).ToList());
    }
}
