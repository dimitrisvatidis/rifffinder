using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rifffinder.Data;
using rifffinder.Models;
using System.Linq;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class MusiciansController : ControllerBase
{
    private readonly AppDbContext _context;

    public MusiciansController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public IActionResult CreateMusician([FromBody] Musician musician)
    {
        if (musician == null)
            return BadRequest("Musician cannot be null.");

        if (_context.Musicians.Any(m => m.Email == musician.Email))
        {
            return BadRequest(new { error = "A musician with this email already exists." });
        }

        musician.Password = BCrypt.Net.BCrypt.HashPassword(musician.Password);
        musician.BandId = musician.BandId == 0 ? null : musician.BandId;

        _context.Musicians.Add(musician);
        _context.SaveChanges();

        var musicianDto = new MusicianDTO
        {
            Id = musician.Id,
            Name = musician.Name,
            Surname = musician.Surname,
            Email = musician.Email,
            Instrument = musician.Instrument,
            BandId = musician.BandId
        };

        return CreatedAtAction(nameof(GetMusician), new { id = musician.Id }, musicianDto);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentMusician()
    {
        try
        {
            var musicianId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (musicianId == null)
            {
                return Unauthorized(new { message = "Invalid token. Musician ID not found." });
            }

            var musician = await _context.Musicians
                .Include(m => m.Band)
                .FirstOrDefaultAsync(m => m.Id == int.Parse(musicianId));

            if (musician == null)
            {
                return NotFound(new { message = "Musician not found." });
            }

            var response = new
            {
                musician.Id,
                musician.Email,
                musician.Name,
                musician.Surname,
                musician.BandId,
                musician.Instrument
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred.", error = ex.Message });
        }
    }

    [Authorize]
    [HttpGet("{id}")]
    public IActionResult GetMusician(int id)
    {
        var musician = _context.Musicians.FirstOrDefault(m => m.Id == id);

        if (musician == null)
            return NotFound();

        var musicianDto = new MusicianDTO
        {
            Id = musician.Id,
            Name = musician.Name,
            Surname = musician.Surname,
            Email = musician.Email,
            Instrument = musician.Instrument,
            BandId = musician.BandId
        };

        return Ok(musicianDto);
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Musician>>> GetMusiciansByBandId(int bandId)
    {
        return await _context.Musicians
            .Where(m => m.BandId == bandId)
            .ToListAsync();
    }

}
