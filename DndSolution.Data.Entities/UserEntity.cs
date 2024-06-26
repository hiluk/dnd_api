using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

[Table("users")]
public class UserEntity
{
    [Key]
    [Column("id")]
    public long Id { get; set; }
   
    [Column("email")]
    public string Email { get; init; }
    
    [Column("password")]
    public string Password { get; init; }
}