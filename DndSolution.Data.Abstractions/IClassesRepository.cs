

using DndSolution.Application.Models.Models.Classes;

namespace Data.Abstractions;

public interface IClassesRepository
{
    public Task SaveClassAsync(CharacterClass entity, CancellationToken token);

    public Task<List<CharacterClass>> GetAllClassesAsync(CancellationToken token);
}