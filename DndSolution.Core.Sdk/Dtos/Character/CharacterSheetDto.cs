using Core.Sdk.Enums;

namespace Core.Sdk.Dtos.Characters;

/// <summary>
/// Вычисленные значения персонажа
/// </summary>
public class CharacterSheetDto
{
    /// <summary>
    /// Итоговые характеристики (с бонусами расы)
    /// </summary>
    public required CharacterStatsDto Stats { get; init; }

    /// <summary>
    /// Модификаторы характеристик
    /// </summary>
    public required CharacterStatsDto Modifiers { get; init; }

    /// <summary>
    /// Максимум хитов
    /// </summary>
    public int HitPoints { get; init; }

    /// <summary>
    /// Класс доспеха (без доспехов)
    /// </summary>
    public int ArmorClass { get; init; }

    /// <summary>
    /// Бонус инициативы
    /// </summary>
    public int Initiative { get; init; }

    /// <summary>
    /// Скорость, футов
    /// </summary>
    public int Speed { get; init; }

    /// <summary>
    /// Бонус мастерства
    /// </summary>
    public int ProficiencyBonus { get; init; }

    /// <summary>
    /// Спасброски
    /// </summary>
    public required List<SavingThrowDto> SavingThrows { get; init; }
}

/// <summary>
/// Спасбросок
/// </summary>
public class SavingThrowDto
{
    public AbilityDto Ability { get; init; }

    /// <summary>
    /// Итоговый бонус
    /// </summary>
    public int Bonus { get; init; }

    /// <summary>
    /// Владеет ли класс спасброском
    /// </summary>
    public bool Proficient { get; init; }
}
