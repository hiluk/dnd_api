﻿using DndSolution.Application.Models.Enums;

namespace DndSolution.Application.Models.Models;

public class Character
{
    /// <summary>
    /// Имя персонажа
    /// </summary>
    public string Name { get; init; }
    
    /// <summary>
    /// Уровень персонажа
    /// </summary>
    public byte Level { get; set; }
    
    /// <summary>
    /// Эмейл пользователя
    /// </summary>
    public string Email { get; init; }
    
    /// <summary>
    /// Класс персонажа
    /// </summary>
    public CharacterClass CharacterClass { get; init; }

    /// <summary>
    /// Расса персонажа
    /// </summary>
    public CharacterRace CharacterRace { get; set; }
}