using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DndSolution.Application.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DndSolution.Neccessary;

public class JwtProvider : IJwtProvider

    
{
    
    private readonly JwtOptions _options;

    public JwtProvider(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }
    
    public string GenerateToken(User user)
    {
        Claim[] claims = [
            new("user_id", user.UserId.ToString()), 
            new ("email", user.Email)
        ];
        
        
        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey)),
            SecurityAlgorithms.HmacSha256Signature);

        var token = new JwtSecurityToken(
            signingCredentials: signingCredentials,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(_options.ExpiresHours));

        var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

        return tokenValue;
    }
}