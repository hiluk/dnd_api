using Data.Abstractions;
using DndSolution.Application.Abstractions;
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

    /// <inheritdoc />
    public Task CreateCharacterAsync(Guid userId, Character character, CancellationToken token)
        => _repository.SaveCharacterAsync(character, userId, token);

    /// <inheritdoc />
    public Task<IReadOnlyList<Character>> GetAllUserCharactersAsync(Guid userId, CancellationToken token)
        => _repository.GetAllUserCharactersAsync(userId, token);

    /// <inheritdoc />
    public Task<Character?> GetCharacterAsync(Guid id, Guid userId, CancellationToken token)
        => _repository.GetCharacterByIdAsync(userId, id, token);

    /// <inheritdoc />
    public Task<bool> DeleteCharacterAsync(Guid id, Guid userId, CancellationToken token)
        => _repository.DeleteCharacterAsync(userId, id, token);
}
