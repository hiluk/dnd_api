using BCrypt.Net;

namespace DndSolution.Neccessary;

public class PasswordHasher : IPasswordHasher
{
    public string Generate(string password) => BCrypt.Net.BCrypt.EnhancedHashPassword(password);
    
    public bool Verify(string password, string hasherdPassword) => BCrypt.Net.BCrypt.EnhancedVerify(password, hasherdPassword);
    
}