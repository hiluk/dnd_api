﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities.Class;

public class CharacterClassEntity
{
    
    public long Id { get; init; }
    

    public string Name { get; set; }
    
    public string Description { get; set; }
    
    public string HitDice { get; set; }
    
    public string HpAt1stLevel { get; set; }
    
    public string HpAtHigherLevels { get; set; }
    
    public string ProfArmor { get; set; }
    
    public string ProfWeapon { get; set; }
    
    public string ProfTool { get; set; }
    
    public string ProfSavingThrows { get; set; }
    
    public string ProfSkills { get; set; }
    
    public string Equipment { get; set; }
    
    public string SpellCastingAbility { get; set; }
}