using System.Security.Cryptography;
using API.Entities;

namespace API.Interfaces;

public interface ITokenService
{
    Task<string> CreateToken(AppUser user);
    public string GenerateRefreshToken()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(randomBytes);
    }
}