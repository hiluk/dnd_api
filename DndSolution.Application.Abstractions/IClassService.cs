using DndSolution.Application.Models.Models.Classes;

namespace DndSolution.Application.Abstractions;

public interface IClassService
{
    /// <summary>
    /// Функция сохранения класса 
    /// </summary>
    /// <param name="model">Модель класса</param>
    /// <returns></returns>
    public Task SaveClassAsync(CharacterClass model, CancellationToken token);

    /// <summary>
    /// Фукция получения всех классов из репозитория
    /// </summary>
    /// <returns>Возвращает список всех классов из репозитория</returns>
    public Task<List<CharacterClass>> GetAllClassesAsync(CancellationToken token);
}