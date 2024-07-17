using Data.Abstractions;
using DndSolution.Application.Abstractions;
using DndSolution.Application.Models.Models.Classes;

namespace DndSolution.Application.Services;

public class ClassService : IClassService
{
    private readonly IClassesRepository _repository;

    public ClassService(IClassesRepository repository)
    {
        _repository = repository;
    }

    /// <inheritdoc />
    public async Task SaveClassAsync(CharacterClass model, CancellationToken token)
    {
        await _repository.SaveClassAsync(model, token);
    }

    /// <inheritdoc />
    public async Task<List<CharacterClass>> GetAllClassesAsync(CancellationToken token)
    {
        return  await _repository.GetAllClassesAsync(token);
    }
}
    
    