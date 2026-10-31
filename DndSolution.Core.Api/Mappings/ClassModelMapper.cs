using Core.Sdk.Dtos.Classes;
using Core.Sdk.Enums;
using DndSolution.Application.Models.Enums;
using DndSolution.Application.Models.Models.Classes;

namespace Core.Api.Mappings;

public static class ClassModelMapper
{
    public static CharacterClass MapToModel(CharacterClassDto dto)
    {
        return new CharacterClass
        {
            HitDie = dto.HitDie,
            SavingThrows = dto.SavingThrows.Select(a => (Ability)a).ToList(),
            Equipment = dto.Equipment,
            Description = dto.Description,
            Name = dto.Name,
            HitDice = dto.HitDice,
            ProfArmor = dto.ProfArmor,
            ProfSkills = dto.ProfSkills,
            ProfWeapon = dto.ProfWeapon,
            ProfTool = dto.ProfTool,
            HpAt1stLevel = dto.HpAt1stLevel,
            ProfSavingThrows = dto.ProfSavingThrows,
            SpellCastingAbility = dto.SpellCastingAbility,
            HpAtHigherLevels = dto.HpAtHigherLevels,
        };
    }
    
    public static CharacterClassDto MapToDto(CharacterClass model)
    {
        return new CharacterClassDto
        {
            HitDie = model.HitDie,
            SavingThrows = model.SavingThrows.Select(a => (AbilityDto)a).ToList(),
            Equipment = model.Equipment,
            Description = model.Description,
            Name = model.Name,
            HitDice = model.HitDice,
            ProfArmor = model.ProfArmor,
            ProfSkills = model.ProfSkills,
            ProfWeapon = model.ProfWeapon,
            ProfTool = model.ProfTool,
            HpAt1stLevel = model.HpAt1stLevel,
            ProfSavingThrows = model.ProfSavingThrows,
            SpellCastingAbility = model.SpellCastingAbility,
            HpAtHigherLevels = model.HpAtHigherLevels,
        };
    }
}