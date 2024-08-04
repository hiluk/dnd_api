using Core.Sdk.Dtos;
using Core.Sdk.Dtos.Characters;
using Core.Sdk.Enums;
using DndSolution.Application.Models.Enums;
using DndSolution.Application.Models.Models;
using DndSolution.Application.Models.Models.Character;
using DndSolution.Application.Models.Models.Classes;

namespace Core.Api.Mappings;

public static class CharacterMapper
{
    public static Character MapToModel(CharacterDto dto)
    {
        var character = new Character
        {
            Name = dto.Name,
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
                Constitution = dto.CharacterStats.Constitution
            }
        };

        return character;
    }

    public static CharacterDto MapToDto(Character model)
    {
        var character = new CharacterDto
        {
            Id = model.Id,
            Name = model.Name,
            Level = model.Level,
            CharacterClass = (CharacterClassTypeDto)model.CharacterClass,
            CharacterRace = (CharacterRaceTypeDto)model.CharacterRace,
            CharacterStats = new CharacterStatsDto()
            {
                Constitution = model.Stats.Constitution,
                Dexterity = model.Stats.Constitution,
                Intelligence = model.Stats.Intelligence,
                Wisdom = model.Stats.Wisdom,
                Charisma = model.Stats.Charisma,
                Strength = model.Stats.Strength,
            }
        };

        return character;
    }
}