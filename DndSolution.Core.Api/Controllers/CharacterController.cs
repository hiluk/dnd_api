using System.Security.Claims;
using Core.Api.Mappings;
using Core.Sdk.Dtos.Characters;
using DndSolution.Application.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Core.Api.Controllers;

/// <summary>
/// Контроллер работы с персонажами текущего пользователя
/// </summary>
[ApiController]
[Authorize]
[Route("/characters")]
public class CharacterController : ControllerBase
{
    private readonly ICharacterService _service;

    /// <summary>
    /// .ctor
    /// </summary>
    /// <param name="service">Сервис для работы с персонажами</param>
    public CharacterController(ICharacterService service)
    {
        _service = service;
    }

    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
                   ?? throw new InvalidOperationException("В токене нет Id пользователя"));

    /// <summary>
    /// Создать нового персонажа
    /// </summary>
    /// <param name="dto">Дто с информацией о персонаже</param>
    /// <param name="token">Токен отмены операции</param>
    /// <returns>Созданный персонаж</returns>
    [HttpPost("create")]
    [ProducesResponseType<CharacterDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCharacter([FromBody] CharacterDto dto, CancellationToken token)
    {
        var character = CharacterMapper.MapToModel(dto);

        await _service.CreateCharacterAsync(CurrentUserId, character, token);

        var result = CharacterMapper.MapToDto(character);
        return CreatedAtAction(nameof(GetCharacterById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Получить всех персонажей пользователя
    /// </summary>
    /// <param name="token">Токен отмены операции</param>
    [HttpGet("")]
    [ProducesResponseType<List<CharacterDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllUserCharacters(CancellationToken token)
    {
        var models = await _service.GetAllUserCharactersAsync(CurrentUserId, token);

        return Ok(models.Select(CharacterMapper.MapToDto).ToList());
    }

    /// <summary>
    /// Получить персонажа по Id
    /// </summary>
    /// <param name="id">Id персонажа</param>
    /// <param name="token">Токен отмены операции</param>
    [HttpGet("{id:guid}")]
    [ProducesResponseType<CharacterDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCharacterById([FromRoute] Guid id, CancellationToken token)
    {
        var model = await _service.GetCharacterAsync(id, CurrentUserId, token);

        return model is null
            ? Problem("Персонаж не найден", statusCode: StatusCodes.Status404NotFound)
            : Ok(CharacterMapper.MapToDto(model));
    }

    /// <summary>
    /// Удалить персонажа пользователя
    /// </summary>
    /// <param name="id">Id персонажа</param>
    /// <param name="token">Токен отмены операции</param>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCharacter([FromRoute] Guid id, CancellationToken token)
    {
        var deleted = await _service.DeleteCharacterAsync(id, CurrentUserId, token);

        return deleted
            ? NoContent()
            : Problem("Персонаж не найден", statusCode: StatusCodes.Status404NotFound);
    }
}
