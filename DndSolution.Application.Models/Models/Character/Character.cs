using DndSolution.Application.Models.Enums;

namespace DndSolution.Application.Models.Models.Character;

public class Character
{
    public Guid Id { get; set; }
    
    /// <summary>
    /// Имя персонажа
    /// </summary>
    public string Name { get; init; }
    
    /// <summary>
    /// Уровень персонажа
    /// </summary>
    public byte Level { get; set; }
    
    /// <summary>
    /// Класс персонажа
    /// </summary>
    public CharacterClassType CharacterClass { get; init; }

    /// <summary>
    /// Раса персонажа
    /// </summary>
    public CharacterRaceType CharacterRace { get; set; }
    
    /// <summary>
    /// Базовые характеристики (до бонусов расы)
    /// </summary>
    public CharacterStats Stats { get; init; }

    /// <summary>
    /// Способ генерации характеристик
    /// </summary>
    public GenerationMethod GenerationMethod { get; init; }

    /// <summary>
    /// Характеристики, выбранные для бонусов расы «на выбор»
    /// </summary>
    public List<Ability> BonusChoices { get; init; } = [];
}