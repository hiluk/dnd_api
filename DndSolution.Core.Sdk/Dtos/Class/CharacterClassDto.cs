using Core.Sdk.Enums;

namespace Core.Sdk.Dtos.Classes;

public class CharacterClassDto
{
    public string Name { get; set; }

    /// <summary>
    /// Кость хитов: 6, 8, 10 или 12
    /// </summary>
    public int HitDie { get; set; }

    /// <summary>
    /// Спасброски, которыми владеет класс
    /// </summary>
    public List<AbilityDto> SavingThrows { get; set; } = [];

    /// <summary>
    /// Сколько навыков выбирается из <see cref="SkillOptions"/>
    /// </summary>
    public int SkillChoices { get; set; }

    /// <summary>
    /// Навыки, из которых выбирает класс
    /// </summary>
    public List<SkillDto> SkillOptions { get; set; } = [];

    /// <summary>
    /// Сколько навыков получают компетентность на 1 уровне
    /// </summary>
    public int ExpertiseChoices { get; set; }
    
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