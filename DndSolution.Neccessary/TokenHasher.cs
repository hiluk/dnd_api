using DndSolution.Application.Models;

namespace DndSolution.Neccessary;

public class TokenHasher : ITokenHasher
{

        public string Generate(string token) => BCrypt.Net.BCrypt.EnhancedHashPassword(token);

        public bool Verify(string password, User user)
        {
            var timeNow = DateTime.UtcNow;
            var lifeTime = user.RefreshToken.ValidUntil;

            if ((timeNow - lifeTime).Milliseconds > 0) return false;
            
           return BCrypt.Net.BCrypt.EnhancedVerify(password, user.RefreshToken.TokenHash);     
        } 
        
}