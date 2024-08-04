using Data.Abstractions;
using DndSolution.Application.Abstractions;
using DndSolution.Application.Models;
using DndSolution.Application.Models.Models;
using DndSolution.Application.Models.Models.Character;
using Microsoft.AspNetCore.Identity;

namespace DndSolution.Application.Services;

/// <summary>
/// Сервис для работы с персонажами
/// </summary>
public class CharacterService : ICharacterService
{
    private readonly ICharactersRepository _repository;
    private readonly UserManager<User> _manager;

    /// <summary>
    /// .ctor
    /// </summary>
    /// <param name="repository">Репозиторий для работы с персонажами</param>
    public CharacterService(ICharactersRepository repository, UserManager<User> manager)
    {
        _repository = repository;
        _manager = manager;
    }

    /// <inheritdoc />>
    public async Task CreateCharacterAsync(string email, Character character, CancellationToken token)
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
    public async Task<Character> GetCharacterAsync(Guid id, string email, CancellationToken token)
    {
        return await _repository.GetCharacterByIdAsync(email, id, token);
    }

    /// <inheritdoc />
    public async Task DeleteCharacterAsync(string email, string name, CancellationToken token)
    {
        await _repository.DeleteCharacterAsync(email, name, token);
    }
}