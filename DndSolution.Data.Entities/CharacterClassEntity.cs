using DndSolution.Application.Models.Enums;

namespace Data.Entities;

/// <summary>
/// Класс (таблица classes)
/// </summary>
public class CharacterClassEntity
{
    public int Id { get; set; }

    public CharacterClassType Type { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int HitDie { get; set; }

    public List<Ability> SavingThrows { get; set; } = [];

    public int SkillChoices { get; set; }

    public List<Skill> SkillOptions { get; set; } = [];

    public int ExpertiseChoices { get; set; }

    public string HitDice { get; set; } = string.Empty;

    public string HpAt1stLevel { get; set; } = string.Empty;

    public string HpAtHigherLevels { get; set; } = string.Empty;

    public string ProfArmor { get; set; } = string.Empty;

    public string ProfWeapon { get; set; } = string.Empty;

    public string ProfTool { get; set; } = string.Empty;

    public string ProfSavingThrows { get; set; } = string.Empty;

    public string ProfSkills { get; set; } = string.Empty;

    public string Equipment { get; set; } = string.Empty;

    public string SpellCastingAbility { get; set; } = string.Empty;
}
