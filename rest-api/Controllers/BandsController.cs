using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rifffinder.Data; 
using rifffinder.Models;
using System.Security.Claims;

namespace rifffinder.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BandsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BandsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Bands/{id}
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Band>> GetBandById(int id)
        {
            var band = await _context.Bands.FindAsync(id);

            if (band == null)
            {
                return NotFound();
            }

            return band;
        }

        // POST: api/Band
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Band>> CreateBand(Band band)
        {
            var musicianId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (musicianId == null)
            {
                return Unauthorized(new { message = "Invalid token. Musician ID not found." });
            }

            var musician = await _context.Musicians.FindAsync(int.Parse(musicianId));

            if (musician == null)
            {
                return BadRequest(new { message = "Musician not found." });
            }

            if (musician.BandId.HasValue)
            {
                return BadRequest(new { message = "You are already part of a band." });
            }

            _context.Bands.Add(band);
            await _context.SaveChangesAsync();

            musician.BandId = band.Id;
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBandById), new { id = band.Id }, band);
        }

        [Authorize]
        [HttpPut("leave-band")]
        public async Task<IActionResult> LeaveBand()
        {
            var musicianIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (musicianIdClaim == null)
            {
                return Unauthorized(new { message = "Musician ID not found in token." });
            }

            int musicianId = int.Parse(musicianIdClaim.Value);
            var musician = await _context.Musicians.FindAsync(musicianId);

            if (musician == null || musician.BandId == null)
            {
                return BadRequest(new { message = "You are not part of a band." });
            }

            musician.BandId = null;
            await _context.SaveChangesAsync();

            return Ok(new { message = "You have successfully left the band." });
        }
    }
}
