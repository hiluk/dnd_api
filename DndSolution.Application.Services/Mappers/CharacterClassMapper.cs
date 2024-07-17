using Data.Entities.Class;
using DndSolution.Application.Models.Models.Classes;

namespace DndSolution.Application.Services.Mappers;

public class CharacterClassMapper
{
    public static CharacterClass MapToModel(CharacterClassEntity entity)
    {
        return new CharacterClass
        {
            Equipment = entity.Equipment,
            Description = entity.Description,
            Name = entity.Name,
            HitDice = entity.HitDice,
            ProfArmor = entity.ProfArmor,
            ProfSkills = entity.ProfSkills,
            ProfWeapon = entity.ProfWeapon,
            ProfTool = entity.ProfTool,
            HpAt1stLevel = entity.HpAt1stLevel,
            ProfSavingThrows = entity.ProfSavingThrows,
            SpellCastingAbility = entity.SpellCastingAbility,
            HpAtHigherLevels = entity.SpellCastingAbility,
        };
    }
    
    public static CharacterClassEntity MapToEntity(CharacterClass model)
    {
        return new CharacterClassEntity
        {
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
            HpAtHigherLevels = model.SpellCastingAbility,
        };
    }
}