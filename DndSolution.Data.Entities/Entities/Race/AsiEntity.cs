using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities.Entities;

[Table("asi")]
public class AsiEntity
{
    [Key]
    [Column("id")]
    public long Id { get; init; }
    
    [Column("stat")]
    public string Stat { get; init; }
    
    [Column("value")]
    public string Value { get; init; }
}