﻿using Data.Entities.Class;

namespace Data.Abstractions;

public interface IClassesRepository
{
    public Task SaveClassAsync(CharacterClassEntity entity, CancellationToken token);

    public Task<List<CharacterClassEntity>> GetAllClassesAsync(CancellationToken token);
}