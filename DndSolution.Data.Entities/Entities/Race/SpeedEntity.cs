using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities.Entities;

[Table("speed")]
public class SpeedEntity
{
    [Key]
    [Column("id")]
    public string Id { get; set; }
    
    [Column("type")]
    public string Type { get; set; }
    
    [Column("value")]
    public int Value { get; set; }
}