namespace Core.Sdk.Dtos;

public class UserDto
{
    public Guid UserId { get; set; }
    
    public string Email { get; init; }
    
    public string UserName { get; init; }
    
    public string PasswordHash { get; init; }
}