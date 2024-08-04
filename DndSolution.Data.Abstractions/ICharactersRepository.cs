using DndSolution.Application.Models.Models;
using DndSolution.Application.Models.Models.Character;

namespace Data.Abstractions;

/// <summary>
/// Интерфей репозитория для работы с персонажами
/// </summary>
public interface ICharactersRepository
{
    /// <summary>
    /// Создать нового персонажа
    /// </summary>
    /// <param name="character"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public Task SaveCharacterAsync(Character character, string email, CancellationToken token);

    /// <summary>
    /// Получить всех персонажей пользователя
    /// </summary>
    /// <param name="email">Эмейл пользователя</param>
    /// <param name="token">Токен отмены операции</param>
    /// <returns></returns>
    public Task<IReadOnlyList<Character>> GetAllUserCharactersAsync(string email, CancellationToken token);
    
    /// <summary>
    /// Получить персонажа пользователя по имени
    /// </summary>
    /// <param name="email">Эмейл пользователя</param>
    /// <param name="name">Имя персонажа</param>
    /// <param name="token">Токен отмены операции</param>
    public Task<Character> GetCharacterByIdAsync(string email, Guid id, CancellationToken token);
    
    /// <summary>
    /// Удалить персонажа
    /// </summary>
    /// <param name="email">Эмейл пользователя</param>
    /// <param name="name">Имя персонажа</param>
    /// <param name="token">Токен отмены операции</param>
    public Task DeleteCharacterAsync(string email, string name, CancellationToken token);
}