using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class MusiciansController : ControllerBase
{
    private readonly MusicianService _musicianService;

    public MusiciansController(MusicianService musicianService)
    {
        _musicianService = musicianService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateMusician([FromBody] MusicianDTO musicianDto)
    {
        var createdMusician = await _musicianService.CreateMusicianAsync(musicianDto);
        return CreatedAtAction(nameof(GetMusician), new { id = createdMusician.Id }, createdMusician);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetMusician(int id)
    {
        var musician = await _musicianService.GetMusicianByIdAsync(id);
        return musician != null ? Ok(musician) : NotFound();
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentMusician()
    {
        var musicianId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (musicianId == null)
        {
            return Unauthorized(new { message = "Invalid token. Musician ID not found." });
        }

        var musician = await _musicianService.GetMusicianByIdAsync(int.Parse(musicianId));
        return musician != null ? Ok(musician) : NotFound();
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetMusiciansByBandId([FromQuery] int bandId)
    {
        var musicians = await _musicianService.GetMusiciansByBandIdAsync(bandId);
        return Ok(musicians);
    }
}