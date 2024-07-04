using Data.Abstractions;
using DndSolution.Application.Abstractions;
using DndSolution.Application.Models.Models.Classes;
using DndSolution.Application.Services.Mappers;

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
        var entity = CharacterClassMapper.MapToEntity(model);

        await _repository.SaveClassAsync(entity, token);
    }

    /// <inheritdoc />
    public async Task<List<CharacterClass>> GetAllClassesAsync(CancellationToken token)
    {
        var entities = await _repository.GetAllClassesAsync(token);
        List<CharacterClass> models = [];

        foreach (var classEntity in entities)
        {
            models.Add(CharacterClassMapper.MapToModel(classEntity));
        }

        return models;
    }
}
    
    