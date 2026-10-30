using DndSolution.Application.Models.Models.Character;

namespace DndSolution.Application.Abstractions;

/// <summary>
/// Интерфейс сервиса работы с персонажами
/// </summary>
public interface ICharacterService
{
    /// <summary>
    /// Создать нового персонажа
    /// </summary>
    /// <param name="userId">Id владельца</param>
    /// <param name="character">Данные о персонаже</param>
    /// <param name="token">Токен отмены операции</param>
    public Task CreateCharacterAsync(Guid userId, Character character, CancellationToken token);

    /// <summary>
    /// Получить всех персонажей пользователя
    /// </summary>
    /// <param name="userId">Id пользователя</param>
    /// <param name="token">Токен отмены операции</param>
    public Task<IReadOnlyList<Character>> GetAllUserCharactersAsync(Guid userId, CancellationToken token);

    /// <summary>
    /// Получить персонажа пользователя по Id
    /// </summary>
    /// <param name="id">Id персонажа</param>
    /// <param name="userId">Id пользователя</param>
    /// <param name="token">Токен отмены операции</param>
    public Task<Character?> GetCharacterAsync(Guid id, Guid userId, CancellationToken token);

    /// <summary>
    /// Удалить персонажа
    /// </summary>
    /// <param name="id">Id персонажа</param>
    /// <param name="userId">Id пользователя</param>
    /// <param name="token">Токен отмены операции</param>
    public Task<bool> DeleteCharacterAsync(Guid id, Guid userId, CancellationToken token);
}
