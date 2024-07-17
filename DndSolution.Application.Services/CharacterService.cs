﻿using Data.Abstractions;
using DndSolution.Application.Abstractions;
using DndSolution.Application.Models.Models;
using DndSolution.Application.Models.Models.Character;

namespace DndSolution.Application.Services;

/// <summary>
/// Сервис для работы с персонажами
/// </summary>
public class CharacterService : ICharacterService
{
    private readonly ICharactersRepository _repository;

    /// <summary>
    /// .ctor
    /// </summary>
    /// <param name="repository">Репозиторий для работы с персонажами</param>
    public CharacterService(ICharactersRepository repository)
    {
        _repository = repository;
    }

    /// <inheritdoc />>
    public async Task CreateCharacterAsync(Character character, string email, CancellationToken token)
    {
        
        await _repository.SaveCharacterAsync(character, email, token);
    }

    /// <inheritdoc />>
    public async Task<IReadOnlyList<Character>> GetAllUserCharactersAsync(string email, CancellationToken token)
    {
        var charactes= await _repository.GetAllUserCharactersAsync(email, token);
        
        return charactes;
    }

    /// <inheritdoc />
    public async Task<Character> GetCharacterAsync(string email, string name, CancellationToken token)
    {
        return await _repository.GetCharacterAsync(email, name, token);
    }

    /// <inheritdoc />
    public async Task DeleteCharacterAsync(string email, string name, CancellationToken token)
    {
        await _repository.DeleteCharacterAsync(email, name, token);
    }
}