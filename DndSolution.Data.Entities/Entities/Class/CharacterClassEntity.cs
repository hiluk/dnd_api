using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities.Class;

[Table("class")]
public class CharacterClassEntity
{
    
    [Key]
    [Column("id")]
    public long Id { get; init; }
    
    [Column("name")]
    public string Name { get; set; }
    
    [Column("desc")]
    public string Description { get; set; }
    
    [Column("hit_dice")]
    public string HitDice { get; set; }
    
    [Column("hp_at_1st_level")]
    public string HpAt1stLevel { get; set; }
    
    [Column("hp_at_high_levels")]
    public string HpAtHigherLevels { get; set; }
    
    [Column("prof_armor")]
    public string ProfArmor { get; set; }
    
    [Column("prof_weapon")]
    public string ProfWeapon { get; set; }
    
    [Column("prof_tool")]
    public string ProfTool { get; set; }
    
    [Column("prof_saving_throws")]
    public string ProfSavingThrows { get; set; }
    
    [Column("prof_skills")]
    public string ProfSkills { get; set; }
    
    [Column("equipment")]
    public string Equipment { get; set; }
    
    [Column("spell_casting_ability")]
    public string SpellCastingAbility { get; set; }
}