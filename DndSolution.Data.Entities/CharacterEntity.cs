using System.ComponentModel.DataAnnotations;

namespace Data.Entities;

public class CharacterEntity
{   
    [Key]
    public string Name { get; init; }
    
    public int Level  { get; set; }
    
    public string Class { get; init; }
    
    public int Strenght { get; set; }
    
    public int Intelligence { get; set; }
    
    public int Wisdom { get; set; }
    
    public int Constitution { get; set; }
    
    public int Dexterity { get; set; }

    public int Charisma { get; set; }
}