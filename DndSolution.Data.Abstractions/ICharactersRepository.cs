using DndSolution.Application.Models.Models.Avatars;
using DndSolution.Application.Models.Models.Character;

namespace Data.Abstractions;

/// <summary>
/// Интерфейс репозитория для работы с персонажами
/// </summary>
public interface ICharactersRepository
{
    /// <summary>
    /// Создать нового персонажа
    /// </summary>
    /// <param name="character">Персонаж</param>
    /// <param name="userId">Id владельца</param>
    /// <param name="token">Токен отмены операции</param>
    public Task SaveCharacterAsync(Character character, Guid userId, CancellationToken token);

    /// <summary>
    /// Получить всех персонажей пользователя
    /// </summary>
    /// <param name="userId">Id пользователя</param>
    /// <param name="token">Токен отмены операции</param>
    public Task<IReadOnlyList<Character>> GetAllUserCharactersAsync(Guid userId, CancellationToken token);

    /// <summary>
    /// Получить персонажа пользователя по Id
    /// </summary>
    /// <param name="userId">Id пользователя</param>
    /// <param name="id">Id персонажа</param>
    /// <param name="token">Токен отмены операции</param>
    /// <returns>Персонаж или null, если он не найден</returns>
    public Task<Character?> GetCharacterByIdAsync(Guid userId, Guid id, CancellationToken token);

    /// <summary>
    /// Удалить персонажа
    /// </summary>
    /// <param name="userId">Id пользователя</param>
    /// <param name="id">Id персонажа</param>
    /// <param name="token">Токен отмены операции</param>
    /// <returns>true, если персонаж был удален</returns>
    public Task<bool> DeleteCharacterAsync(Guid userId, Guid id, CancellationToken token);

    /// <summary>
    /// Записать или убрать (null) свою картинку персонажа
    /// </summary>
    /// <returns>false — персонаж не найден</returns>
    public Task<bool> SetAvatarAsync(Guid userId, Guid id, CharacterAvatar? avatar, CancellationToken token);
}
