namespace DndSolution.Application.Models.Enums;

/// <summary>
/// Предыстория персонажа
/// </summary>
public enum BackgroundType
{
    Acolyte = 1,
    Charlatan = 2,
    Criminal = 3,
    Entertainer = 4,
    FolkHero = 5,
    GuildArtisan = 6,
    Hermit = 7,
    Noble = 8,
    Outlander = 9,
    Sage = 10,
    Sailor = 11,
    Soldier = 12,
    Urchin = 13,

    /// <summary>
    /// Своя предыстория: два любых навыка
    /// </summary>
    Custom = 14,
}
