using DndSolution.Application.Models.Models.Rules;

namespace DndSolution.Application.Abstractions;

/// <summary>
/// Интерфейс сервиса работы с персонажами
/// </summary>
public interface ICharacterService
{
    /// <summary>
    /// Проверить черновик по правилам и посчитать лист, ничего не сохраняя
    /// </summary>
    /// <param name="userId">Id пользователя (нужен для проверки бросков)</param>
    /// <param name="draft">Черновик персонажа</param>
    /// <param name="rollToken">Подпись бросков для способа «Броски»</param>
    /// <param name="token">Токен отмены операции</param>
    /// <exception cref="CharacterRulesException">Черновик нарушает правила</exception>
    public Task<CharacterSheet> PreviewAsync(Guid userId, CharacterDraft draft, string? rollToken, CancellationToken token);

    /// <summary>
    /// Создать нового персонажа 1 уровня
    /// </summary>
    /// <exception cref="CharacterRulesException">Черновик нарушает правила</exception>
    public Task<CharacterWithSheet> CreateCharacterAsync(
        Guid userId, string name, CharacterDraft draft, string? rollToken, CancellationToken token);

    /// <summary>
    /// Получить всех персонажей пользователя
    /// </summary>
    /// <param name="userId">Id пользователя</param>
    /// <param name="token">Токен отмены операции</param>
    public Task<IReadOnlyList<CharacterWithSheet>> GetAllUserCharactersAsync(Guid userId, CancellationToken token);

    /// <summary>
    /// Получить персонажа пользователя по Id
    /// </summary>
    /// <param name="id">Id персонажа</param>
    /// <param name="userId">Id пользователя</param>
    /// <param name="token">Токен отмены операции</param>
    public Task<CharacterWithSheet?> GetCharacterAsync(Guid id, Guid userId, CancellationToken token);

    /// <summary>
    /// Удалить персонажа
    /// </summary>
    /// <param name="id">Id персонажа</param>
    /// <param name="userId">Id пользователя</param>
    /// <param name="token">Токен отмены операции</param>
    public Task<bool> DeleteCharacterAsync(Guid id, Guid userId, CancellationToken token);
}
