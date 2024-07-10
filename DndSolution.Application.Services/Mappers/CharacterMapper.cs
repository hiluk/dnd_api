using Data.Entities.Entities;
using Data.Entities.Enums;
using DndSolution.Application.Models.Enums;
using DndSolution.Application.Models.Models;
using DndSolution.Application.Models.Models.Classes;
using DndSolution.Neccessary;

namespace DndSolution.Application.Services.Mappers;

public static class CharacterMapper
{
    public static CharacterEntity MapToEntity(Character model, string email)
    {
        var characterId = GetNewId();
        

        return new CharacterEntity
        {
            Name = model.Name,
            Level = model.Level,
            Email = email,
            CharacterClass = (CharacterClassTypeEntity)model.CharacterClass,
            CharacterRace = (CharacterRaceTypeEntity)model.CharacterRace,
            Xp = 0,
            DateCreate = DateTime.UtcNow,
            Stats = new CharacterStatsEntity
            {
                Strength = model.Stats.Strength,
                Dexterity = model.Stats.Dexterity,
                Intelligence = model.Stats.Intelligence,
                Wisdom = model.Stats.Wisdom,
                Charisma = model.Stats.Charisma,
                Constitution = model.Stats.Constitution
            }
        };
    }
    
    public static Character MapToModel(CharacterEntity entity)
    {
        

         return new Character
        {
            Name = entity.Name,
            Level = entity.Level,
            CharacterClass = (CharacterClassType)entity.CharacterClass,
            CharacterRace = (CharacterRaceType)entity.CharacterRace,
            Stats = new CharacterStats
            {
                Strength = entity.Stats.Strength,
                Dexterity = entity.Stats.Dexterity,
                Intelligence = entity.Stats.Intelligence,
                Wisdom = entity.Stats.Wisdom,
                Charisma = entity.Stats.Charisma,
                Constitution = entity.Stats.Constitution
            }
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