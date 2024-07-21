using DndSolution.Application.Models;
using DndSolution.Application.Models.Models;
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
    /// <param name="character">Данные о персонаже</param>
    /// <param name="token">Токен отмены операции</param>
    public Task CreateCharacterAsync(string email, Character character, CancellationToken token);

    /// <summary>
    /// Получить всех персонажей пользователя
    /// </summary>
    /// <param name="email">Эмейл пользователя</param>
    /// <param name="token">Токен отмены операции</param>
    public Task<IReadOnlyList<Character>> GetAllUserCharactersAsync(string email, CancellationToken token);
    
    /// <summary>
    /// Получить персонажа пользователя по имени
    /// </summary>
    /// <param name="email">Эмейл пользователя</param>
    /// <param name="name">Имя персонажа</param>
    /// <param name="token">Токен отмены операции</param>
    public Task<Character> GetCharacterAsync(string email, string name, CancellationToken token);

    /// <summary>
    /// Удалить персонажа
    /// </summary>
    /// <param name="email">Эмейл пользователя</param>
    /// <param name="name">Имя персонажа</param>
    /// <param name="token">Токен отмены операции</param>
    public Task DeleteCharacterAsync(string email, string name, CancellationToken token);
}