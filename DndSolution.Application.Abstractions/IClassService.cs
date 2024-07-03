using DndSolution.Application.Models.Models.Classes;

namespace DndSolution.Application.Abstractions;

public interface IClassService
{
    public Task SaveClassAsync(CharacterClass model, CancellationToken token);

    public Task<List<CharacterClass>> GetAllClassesAsync(CancellationToken token);
}