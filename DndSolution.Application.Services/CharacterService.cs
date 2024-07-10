using Data.Abstractions;
using DndSolution.Application.Abstractions;
using DndSolution.Application.Models.Models;
using DndSolution.Application.Services.Mappers;

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
        var entity = CharacterMapper.MapToEntity(character, email);
        await _repository.SaveCharacterAsync(entity, token);
    }

    /// <inheritdoc />>
    public async Task<IReadOnlyList<Character>> GetAllUserCharactersAsync(string email, CancellationToken token)
    {
        var entity= await _repository.GetAllUserCharactersAsync(email, token);
        var model = entity.Select(CharacterMapper.MapToModel).ToList();
        
        return model;
    }

    /// <inheritdoc />
    public async Task<Character> GetCharacterAsync(string email, string name, CancellationToken token)
    {
        var entity = await _repository.GetCharacterAsync(email, name, token);
        return CharacterMapper.MapToModel(entity);
    }

    /// <inheritdoc />
    public async Task DeleteCharacterAsync(string email, string name, CancellationToken token)
    {
        await _repository.DeleteCharacterAsync(email, name, token);
    }
}