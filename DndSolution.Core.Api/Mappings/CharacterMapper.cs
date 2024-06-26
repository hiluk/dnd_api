using Core.Sdk.Dtos;
using Core.Sdk.Dtos.Characters;
using DndSolution.Application.Models.Enums;
using DndSolution.Application.Models.Models;

namespace Core.Api.Mappings;

public static class CharacterMapper
{
    public static CharacterFull MapToModel(CharacterSaveRequest dto)
    {
        var stats = new CharacterStats
        {
            Strength = dto.CharacterStats.Strength,
            Dexterity = dto.CharacterStats.Dexterity,
            Intelligence = dto.CharacterStats.Intelligence,
            Wisdom = dto.CharacterStats.Wisdom,
            Charisma = dto.CharacterStats.Charisma,
            Constitution = dto.CharacterStats.Constitution
        };

        var character = new Character
        {
            Name = dto.CharacterInformation.Name,
            Level = dto.CharacterInformation.Level,
            CharacterClass = (CharacterClass)dto.CharacterInformation.CharacterClass,
            CharacterRace = (CharacterRace)dto.CharacterInformation.CharacterRace,
            Email = dto.CharacterInformation.Email
        };

        return new CharacterFull
        {
            Character = character,
            Stats = stats
        };
    }
}