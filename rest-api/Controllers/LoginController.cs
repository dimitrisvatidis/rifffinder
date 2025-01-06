using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using rifffinder.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public LoginController(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    [HttpPost]
    public IActionResult Login([FromBody] LoginRequest login)
    {
        var musician = _context.Musicians.FirstOrDefault(m => m.Email == login.Email);

        if (musician == null || !BCrypt.Net.BCrypt.Verify(login.Password, musician.Password))
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_config["JWTSecret"]);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, musician.Id.ToString()),
                new Claim(ClaimTypes.Name, musician.Id.ToString()),
                new Claim(ClaimTypes.Email, musician.Email)
            }),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = "rifffinder",
            Audience = "rifffinderAudience"
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return Ok(new { Token = tokenHandler.WriteToken(token) });
    }
}

public class LoginRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}
