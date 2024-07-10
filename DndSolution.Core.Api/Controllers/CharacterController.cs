﻿using Core.Api.Mappings;
using Core.Sdk.Dtos.Character;
using Core.Sdk.Dtos.Characters;
using DndSolution.Application.Abstractions;
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
    public async Task CreateCharacter([FromBody]CharacterFullDto dto, CancellationToken token)
    {
        try
        {
            var character = CharacterMapper.MapToModel(dto.Character);
            await _service.CreateCharacterAsync(character, dto.email, token);
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
    [HttpPost("")]
    public async Task<IReadOnlyList<CharacterDto>> GetAllUserCharacters(string email, CancellationToken token)
    {
        try
        {
            var model = await _service.GetAllUserCharactersAsync(email, token);
            var result = model.Select(CharacterMapper.MapToDto).ToList();

            return result;
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
    public async Task<CharacterDto> GetCharacterByName(CharacterRequest request, CancellationToken token)
    {
        try
        {
            var model = await _service.GetCharacterAsync(request.Email, request.Name, token);
            return CharacterMapper.MapToDto(model);
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
    public async Task DeleteCharacter([FromBody] CharacterRequest request, CancellationToken token)
    {
        try
        {
            await _service.DeleteCharacterAsync(request.Email, request.Name, token);
        }
        catch (Exception e)
        {
            _logger.LogError("Не удалось удалить персонажа");
            throw;
        }
    }
}