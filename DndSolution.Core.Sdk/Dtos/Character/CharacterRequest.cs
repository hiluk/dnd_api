﻿namespace Core.Sdk.Dtos.Characters;

/// <summary>
/// Запрос на получение персонажа по имени
/// </summary>
public class CharacterRequest
{
    /// <summary>
    /// Эмейл владельца персонажа
    /// </summary>
    public required string Email { get; init; }
    
    /// <summary>
    /// Имя персонажа
    /// </summary>
    public required string Name { get; init; }
}