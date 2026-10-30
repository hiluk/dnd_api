using Core.Sdk.Dtos;
using Core.Sdk.Dtos.Characters;
using Core.Sdk.Enums;
using DndSolution.Application.Models.Enums;
using DndSolution.Application.Models.Models;
using DndSolution.Application.Models.Models.Character;

namespace Core.Api.Mappings;

public static class CharacterMapper
{
    public static Character MapToModel(CharacterDto dto)
    {
        return new Character
        {
            Name = dto.Name.Trim(),
            Level = dto.Level,
            CharacterClass = (CharacterClassType)dto.CharacterClass,
            CharacterRace = (CharacterRaceType)dto.CharacterRace,
            Stats = new CharacterStats
            {
                Strength = dto.CharacterStats.Strength,
                Dexterity = dto.CharacterStats.Dexterity,
                Intelligence = dto.CharacterStats.Intelligence,
                Wisdom = dto.CharacterStats.Wisdom,
                Charisma = dto.CharacterStats.Charisma,
                Constitution = dto.CharacterStats.Constitution,
            },
        };
    }

    public static CharacterDto MapToDto(Character model)
    {
        return new CharacterDto
        {
            Id = model.Id,
            Name = model.Name,
            Level = model.Level,
            CharacterClass = (CharacterClassTypeDto)model.CharacterClass,
            CharacterRace = (CharacterRaceTypeDto)model.CharacterRace,
            CharacterStats = new CharacterStatsDto
            {
                Strength = model.Stats.Strength,
                Dexterity = model.Stats.Dexterity,
                Intelligence = model.Stats.Intelligence,
                Wisdom = model.Stats.Wisdom,
                Charisma = model.Stats.Charisma,
                Constitution = model.Stats.Constitution,
            },
        };
    }
}
