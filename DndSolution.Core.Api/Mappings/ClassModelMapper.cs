using Core.Sdk.Dtos.Classes;
using DndSolution.Application.Models.Models.Classes;

namespace Core.Api.Mappings;

public class ClassModelMapper
{
    public static CharacterClass MapToModel(CharacterClassDto dto)
    {
        return new CharacterClass
        {
            Equipment = dto.Equipment,
            Description = dto.Equipment,
            Name = dto.Name,
            HitDice = dto.HitDice,
            ProfArmor = dto.ProfArmor,
            ProfSkills = dto.ProfSkills,
            ProfWeapon = dto.ProfWeapon,
            ProfTool = dto.ProfTool,
            HpAt1stLevel = dto.HpAt1stLevel,
            ProfSavingThrows = dto.ProfSavingThrows,
            SpellCastingAbility = dto.SpellCastingAbility,
            HpAtHigherLevels = dto.SpellCastingAbility,
        };
    }
    
    public static CharacterClassDto MapToDto(CharacterClass model)
    {
        return new CharacterClassDto
        {
            Equipment = model.Equipment,
            Description = model.Equipment,
            Name = model.Name,
            HitDice = model.HitDice,
            ProfArmor = model.ProfArmor,
            ProfSkills = model.ProfSkills,
            ProfWeapon = model.ProfWeapon,
            ProfTool = model.ProfTool,
            HpAt1stLevel = model.HpAt1stLevel,
            ProfSavingThrows = model.ProfSavingThrows,
            SpellCastingAbility = model.SpellCastingAbility,
            HpAtHigherLevels = model.SpellCastingAbility,
        };
    }
}