namespace DndSolution.Application.Models;

public class User
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public string Email { get; init; }
    public string UserName { get; init; }
    public RefreshToken RefreshToken { get; init; }
    public string PasswordHash { get; init; }
}