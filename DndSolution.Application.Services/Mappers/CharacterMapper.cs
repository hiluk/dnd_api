using Data.Entities.Entities;
using Data.Entities.Enums;
using DndSolution.Application.Models.Enums;
using DndSolution.Application.Models.Models;
using DndSolution.Application.Models.Models.Classes;
using DndSolution.Neccessary;

namespace DndSolution.Application.Services.Mappers;

public static class CharacterMapper
{
    public static CharacterFullEntity MapToEntity(CharacterFull model)
    {
        var characterId = GetNewId();
        
        var stats = new CharacterStatsEntity
        {
            CharacterId = characterId,
            Strength = model.Stats.Strength,
            Dexterity = model.Stats.Dexterity,
            Intelligence = model.Stats.Intelligence,
            Wisdom = model.Stats.Wisdom,
            Charisma = model.Stats.Charisma,
            Constitution = model.Stats.Constitution
        };

        var character = new CharacterEntity
        {
            Name = model.Character.Name,
            Level = model.Character.Level,
            CharacterClass = CharacterClassMapper.MapToEntity(model.Character.CharacterClass),
            CharacterRace = RaceMapper.MapToEntity(model.Character.CharacterRace),
            CharacterId = characterId,
            Xp = 0,
            Email = model.Character.Email,
            DateCreate = DateTime.UtcNow
        };

        return new CharacterFullEntity
        {
            Character = character,
            Stats = stats
        };
    }
    
    public static CharacterFull MapToModel(CharacterFullEntity entity)
    {
        var stats = new CharacterStats
        {
            Strength = entity.Stats.Strength,
            Dexterity = entity.Stats.Dexterity,
            Intelligence = entity.Stats.Intelligence,
            Wisdom = entity.Stats.Wisdom,
            Charisma = entity.Stats.Charisma,
            Constitution = entity.Stats.Constitution
        };

        var character = new Character
        {
            Name = entity.Character.Name,
            Level = entity.Character.Level,
            CharacterClass = CharacterClassMapper.MapToModel(entity.Character.CharacterClass),
            CharacterRace = RaceMapper.MapToModel(entity.Character.CharacterRace),
            Email = entity.Character.Email,
        };

        return new CharacterFull
        {
            Character = character,
            Stats = stats
        };
    }
    
    private static string GetNewId()
    {
        var chars = DndConstants.Alphanumeric;
        var characterId = new char[16];
        var rand = new Random();
        
        for (int i = 0; i < characterId.Length; i++)
        {
            characterId[i] = chars[rand.Next(chars.Length)];
        }

        return new String(characterId);
    }
}