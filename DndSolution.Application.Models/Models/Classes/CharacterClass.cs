using DndSolution.Application.Models.Enums;

namespace DndSolution.Application.Models.Models.Classes;

public class CharacterClass
{

    /// <summary>
    /// Класс, которому соответствует запись справочника
    /// </summary>
    public CharacterClassType Type { get; set; }

    /// <summary>
    /// Кость хитов: 6, 8, 10 или 12
    /// </summary>
    public int HitDie { get; set; }

    /// <summary>
    /// Спасброски, которыми владеет класс
    /// </summary>
    public List<Ability> SavingThrows { get; set; } = [];

    /// <summary>
    /// Сколько навыков выбирается из <see cref="SkillOptions"/>
    /// </summary>
    public int SkillChoices { get; set; }

    /// <summary>
    /// Навыки, из которых выбирает класс
    /// </summary>
    public List<Skill> SkillOptions { get; set; } = [];

    /// <summary>
    /// Сколько навыков получают компетентность на 1 уровне (плут — 2)
    /// </summary>
    public int ExpertiseChoices { get; set; }
    public string Name { get; set; }
    
    public string Description { get; set; }
    
    public string HitDice { get; set; }
    
    public string HpAt1stLevel { get; set; }
    
    public string HpAtHigherLevels { get; set; }
    
    public string ProfArmor { get; set; }
    
    public string ProfWeapon { get; set; }
    
    public string ProfTool { get; set; }
    
    public string ProfSavingThrows { get; set; }
    
    public string ProfSkills { get; set; }
    
    public string Equipment { get; set; }
    
    public string SpellCastingAbility { get; set; }

}