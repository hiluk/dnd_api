using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

public class UserEntity
{
    public long Id { get; set; }
    
    public string Email { get; init; }
    
    public string Password { get; init; }
}