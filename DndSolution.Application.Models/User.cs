using Microsoft.AspNetCore.Identity;

namespace DndSolution.Application.Models;

public class User : IdentityUser<Guid>
{
    public string Email { get; init; }
    public string UserName { get; init; }
    public string PasswordHash { get; init; }
}