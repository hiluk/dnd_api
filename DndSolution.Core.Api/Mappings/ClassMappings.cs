using Core.Sdk.Dtos.Classes;
using DndSolution.Application.Models.Models.Classes;

namespace Core.Api.Mappings;

public static class ClassMappings
{
    public static CharacterClass ToModel(this CharacterClassDto dto) => new()
    {
        Name = dto.Name,
        Description = dto.Description,
        HitDie = dto.HitDie,
        SavingThrows = dto.SavingThrows.ToModel(),
        SkillChoices = dto.SkillChoices,
        SkillOptions = dto.SkillOptions.ToModel(),
        ExpertiseChoices = dto.ExpertiseChoices,
        HitDice = dto.HitDice,
        HpAt1stLevel = dto.HpAt1stLevel,
        HpAtHigherLevels = dto.HpAtHigherLevels,
        ProfArmor = dto.ProfArmor,
        ProfWeapon = dto.ProfWeapon,
        ProfTool = dto.ProfTool,
        ProfSavingThrows = dto.ProfSavingThrows,
        ProfSkills = dto.ProfSkills,
        Equipment = dto.Equipment,
        SpellCastingAbility = dto.SpellCastingAbility,
    };

    public static CharacterClassDto ToDto(this CharacterClass model) => new()
    {
        Name = model.Name,
        Description = model.Description,
        HitDie = model.HitDie,
        SavingThrows = model.SavingThrows.ToDto(),
        SkillChoices = model.SkillChoices,
        SkillOptions = model.SkillOptions.ToDto(),
        ExpertiseChoices = model.ExpertiseChoices,
        HitDice = model.HitDice,
        HpAt1stLevel = model.HpAt1stLevel,
        HpAtHigherLevels = model.HpAtHigherLevels,
        ProfArmor = model.ProfArmor,
        ProfWeapon = model.ProfWeapon,
        ProfTool = model.ProfTool,
        ProfSavingThrows = model.ProfSavingThrows,
        ProfSkills = model.ProfSkills,
        Equipment = model.Equipment,
        SpellCastingAbility = model.SpellCastingAbility,
    };
}
