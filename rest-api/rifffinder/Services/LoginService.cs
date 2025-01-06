using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using rifffinder.Repositories;
using System.Text;
using Microsoft.EntityFrameworkCore;
using rifffinder.Models;
namespace rifffinder.Services;

public class LoginService
{
    private readonly MusicianRepository _musicianRepository;
    private readonly IConfiguration _config;

    public LoginService(MusicianRepository musicianRepository, IConfiguration config)
    {
        _musicianRepository = musicianRepository;
        _config = config;
    }

    public async Task<string> AuthenticateAsync(string email, string password)
    {
        var musician = await _musicianRepository.GetMusicianByEmailAsync(email);
        if (musician == null || !BCrypt.Net.BCrypt.Verify(password, musician.Password))
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_config["JWTSecret"]);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.NameIdentifier, musician.Id.ToString()),
                    new Claim(ClaimTypes.Email, musician.Email)
                }),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = "rifffinder",
            Audience = "rifffinderAudience"
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}