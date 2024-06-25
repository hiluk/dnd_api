using Core.Sdk.Dtos;
using DndSolution.Application.Models.Enums;
using DndSolution.Application.Models.Models;

namespace Core.Api.Mappings;

public static class CharacterMapper
{
    // public static Character MapToModel(CharacterSaveRequest dto)
    // {
    //     return new Character
    //     {
    //         Name = dto.CharacterInformation.Name,
    //         Level = dto.CharacterInformation.Level,
    //         CharacterClass = (CharacterClass)dto.CharacterInformation.CharacterClass,
    //         CharacterRace = (CharacterRace)dto.CharacterInformation.CharacterRace,
    //         Stats = new CharacterStats
    //         {
    //             Strenght = dto.CharacterStats.Strenght,
    //             Charisma = dto.CharacterStats.Charisma,
    //             Intelligence = dto.CharacterStats.Intelligence,
    //             Wisdom = dto.CharacterStats.Wisdom,
    //             Dexterity = dto.CharacterStats.Dexterity,
    //             Constitution = dto.CharacterStats.Constitution
    //         }
    //     };
    // }
    
    public static CharacterFull MapToModel(CharacterSaveRequest dto)
    {
        var stats = new CharacterStats
        {
            Strenght = dto.CharacterStats.Strenght,
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
        };

        return new CharacterFull
        {
            Character = character,
            Stats = stats
        };
    }
}