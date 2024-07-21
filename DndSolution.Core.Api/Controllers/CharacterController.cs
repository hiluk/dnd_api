using System.Security.Claims;
using Core.Api.Mappings;
using Core.Sdk.Dtos.Character;
using Core.Sdk.Dtos.Characters;
using DndSolution.Application.Abstractions;
using DndSolution.Application.Models.Models.Character;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Core.Api.Controllers;

/// <summary>
/// Контроллер работы с персонажами
/// </summary>
[ApiController]
[Route("/characters")]
public class CharacterController : ControllerBase
{
    private readonly ICharacterService _service;
    private readonly ILogger<CharacterController> _logger;

    /// <summary>
    /// .ctor
    /// </summary>
    /// <param name="service">Сервис для работы с персонажами</param>
    /// <param name="logger">Логгер</param>
    public CharacterController(ICharacterService service, ILogger<CharacterController> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>
    /// Создать нового персонажа
    /// </summary>
    /// <param name="dto">Дто с информацеий о персонаже</param>
    /// <param name="token">Токен отмены операции</param>
    [HttpPost("create")]
    [Authorize]
    [ProducesResponseType<string>(200)]
    public async Task<IActionResult> CreateCharacter([FromBody]CharacterDto dto, CancellationToken token)
    {
        try
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            
            if (userEmail == null) return BadRequest("Ошибка");
            
            var character = CharacterMapper.MapToModel(dto);
            
            await _service.CreateCharacterAsync(userEmail, character, token);
            
            return Ok("Персонаж успешно создан");
        }
        catch (Exception e)
        {
            _logger.LogError(e,"Не удалось сохранить персонажа");
            throw;
        }        
    }

    /// <summary>
    /// Получить всех персонажей пользователя
    /// </summary>
    /// <param name="email">Эмейл пользователя</param>
    /// <param name="token">Токен отмены операции</param>
    [HttpGet("")]
    [Authorize]
    [ProducesResponseType<List<CharacterDto>>(200)]
    public async Task<IActionResult> GetAllUserCharacters(CancellationToken token)
    {
        try
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            
            if (userEmail == null) return BadRequest("Пользователь не найден");
            
            var model = await _service.GetAllUserCharactersAsync(userEmail ,token);
            var result = model.Select(CharacterMapper.MapToDto).ToList();

            return Ok(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Не удалось получить персонажей пользователя");
            throw;
        }
    }

    /// <summary>
    /// Получить персонажа по имени
    /// </summary>
    /// <param name="request">Запрос на получение персонажа</param>
    /// <param name="token">Токен отмены операции</param>
    [HttpPost("get-by-name")]
    [Authorize]
    public async Task<IActionResult> GetCharacterByName([FromBody] CharacterRequest request, CancellationToken token)
    {
        try
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            
            if (userEmail == null) return BadRequest("Пользователь не найден");

            var model = await _service.GetCharacterAsync(userEmail, request.Name, token);
            
            return Ok(CharacterMapper.MapToDto(model));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Не удалось получить персонажа");
            throw;
        }
    }

    /// <summary>
    /// Удалить персонажа пользователя
    /// </summary>
    /// <param name="request"></param>
    /// <param name="token"></param>
    [HttpDelete("delete")]
    [Authorize]
    public async Task<IActionResult> DeleteCharacter([FromBody] CharacterRequest request, CancellationToken token)
    {
        try
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            
            if (userEmail == null) return BadRequest("Пользователь не найден");
            
            await _service.DeleteCharacterAsync(userEmail, request.Name, token);

            return Ok("Персонаж удален");
        }
        catch (Exception e)
        {
            _logger.LogError("Не удалось удалить персонажа");
            throw;
        }
    }
}