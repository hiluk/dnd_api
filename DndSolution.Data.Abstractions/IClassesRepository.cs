
using Data.Entities.Class;
using DndSolution.Application.Models.Models.Classes;

namespace Data.Abstractions;

public interface IClassesRepository
{
    public Task SaveClassAsync(CharacterClassEntity entity, CancellationToken token);

    public Task<List<CharacterClassEntity>> GetAllClassesAsync(CancellationToken token);
}