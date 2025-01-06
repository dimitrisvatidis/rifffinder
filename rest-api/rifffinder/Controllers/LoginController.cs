using Microsoft.AspNetCore.Mvc; // For ControllerBase and IActionResult
using rifffinder.DTO; // For LoginDTO
using rifffinder.Services; // For ILoginService
using System.Threading.Tasks; // For Task<>

[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly LoginService _loginService;

    public LoginController(LoginService loginService)
    {
        _loginService = loginService;
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] rifffinder.DTO.LoginDTO login)
    {
        var token = await _loginService.AuthenticateAsync(login.Email, login.Password);
        return Ok(new { Token = token });
    }
}