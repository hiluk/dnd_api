using Core.Sdk.Dtos;
using Core.Sdk.Dtos.Characters;
using Core.Sdk.Enums;
using DndSolution.Application.Models.Enums;
using DndSolution.Application.Models.Models;

namespace Core.Api.Mappings;

public static class CharacterMapper
{
    public static CharacterFull MapToModel(CharacterFullDto dto)
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

    public static CharacterFullDto MapToDto(CharacterFull model)
    {
        var stats = new CharacterStatsDto
        {
            Strength = model.Stats.Strength,
            Dexterity = model.Stats.Dexterity,
            Intelligence = model.Stats.Intelligence,
            Wisdom = model.Stats.Wisdom,
            Charisma = model.Stats.Charisma,
            Constitution = model.Stats.Constitution
        };

        var character = new CharacterDto
        {
            Name = model.Character.Name,
            Level = model.Character.Level,
            CharacterClass = (DtoCharacterClass)model.Character.CharacterClass,
            CharacterRace = (DtoCharacterRace)model.Character.CharacterRace,
            Email = model.Character.Email
        };

        return new CharacterFullDto
        {
            CharacterInformation = character,
            CharacterStats = stats
        };
    }
}