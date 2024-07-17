namespace DndSolution.Application.Models;

public class User
{
    public int Id { get; set; }
    public string Email { get; init; }
    
    public string Password { get; init; }
}