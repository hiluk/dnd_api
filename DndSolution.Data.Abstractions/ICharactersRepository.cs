
using Data.Entities.Entities;
using DndSolution.Application.Models.Models;

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
    public Task SaveCharacterAsync(CharacterEntity character, CancellationToken token);

    /// <summary>
    /// Получить всех персонажей пользователя
    /// </summary>
    /// <param name="email">Эмейл пользователя</param>
    /// <param name="token">Токен отмены операции</param>
    /// <returns></returns>
    public Task<IReadOnlyList<CharacterEntity>> GetAllUserCharactersAsync(string email, CancellationToken token);
    
    /// <summary>
    /// Получить персонажа пользователя по имени
    /// </summary>
    /// <param name="email">Эмейл пользователя</param>
    /// <param name="name">Имя персонажа</param>
    /// <param name="token">Токен отмены операции</param>
    public Task<CharacterEntity> GetCharacterAsync(string email, string name, CancellationToken token);
    
    /// <summary>
    /// Удалить персонажа
    /// </summary>
    /// <param name="email">Эмейл пользователя</param>
    /// <param name="name">Имя персонажа</param>
    /// <param name="token">Токен отмены операции</param>
    public Task DeleteCharacterAsync(string email, string name, CancellationToken token);
}