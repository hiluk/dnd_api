namespace Core.Sdk.Dtos;

public class CustomRegisterRequest
{
    
    
    public required string Email { get; init; }
    
    
    public required string Login { get; init; }
    
    public required string Password { get; init; }
}