using DndSolution.Application.Models;

namespace DndSolution.Neccessary;

public class TokenHasher : ITokenHasher
{

        public string Generate(string token) => BCrypt.Net.BCrypt.EnhancedHashPassword(token);

        public bool Verify(string password, User user)
        {
            var tokens = user.RefreshTokens.ToList();

            foreach (var token in tokens)
            {
                if (BCrypt.Net.BCrypt.EnhancedVerify(password, token.TokenHash))
                {
                    return true;
                }
            }
            
            return false;
        } 
        
}