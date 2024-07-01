using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Core.Api;

public class AuthOptions
{
    public const string Issuer = "MyAuthServer";
    public const string Audience = "MyAuthClient";
    const string Key = "mysupersecret_secretsecretsecretkey!123";
    
    public static SymmetricSecurityKey GetSymmetricSecurityKey() => new (Encoding.UTF8.GetBytes(Key));
}