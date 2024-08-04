namespace DndSolution.Application.Models.Models;

public class CharacterStats
{
    /// <summary>
    /// 
    /// </summary>
    public int Strength { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int Intelligence { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int Wisdom { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public int Constitution { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public int Dexterity { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public int Charisma { get; set; }

    public Guid CharacterId { get; set; }

    public Character.Character Character { get; set; }
}