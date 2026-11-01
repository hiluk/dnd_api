using DndSolution.Application.Models.Enums;

namespace DndSolution.Application.Models.Models.Races;

public class Race
{
    public int Id { get; set; }

    /// <summary>
    /// Раса, которой соответствует запись справочника
    /// </summary>
    public CharacterRaceType Type { get; set; }

    public string Name { get; init; }
    
    public string Description { get; init; }
    
    public string Age { get; init; }
    
    public string Size { get; init; }
    
    public string Language { get; init; }
    
    public string Vision { get; init; }
    
    public string Traits { get; init; }
    
    public List<Asi> Asi { get; init; }
    
    public List<Speed> Speed { get; init; }

    /// <summary>
    /// Навыки, которыми раса владеет всегда
    /// </summary>
    public List<Skill> Skills { get; set; } = [];

    /// <summary>
    /// Сколько любых навыков раса даёт выбрать (полуэльф — 2)
    /// </summary>
    public int SkillChoices { get; set; }
}