namespace DndSolution.Application.Models;

public class RefreshToken
{
    public int UserId { get; set; }
    public User User { get; set; }
    
    public string TokenHash { get; set; }

    public DateTime ValidUntil { get; set; } = DateTime.UtcNow.AddDays(7);
}