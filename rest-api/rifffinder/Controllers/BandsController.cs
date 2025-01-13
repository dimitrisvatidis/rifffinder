using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rifffinder.Models;
using rifffinder.Services;
using System.Security.Claims;

namespace rifffinder.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BandsController : ControllerBase
    {
        private readonly BandService _bandService;

        public BandsController(BandService bandService)
        {
            _bandService = bandService;
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Band>> GetBandById(int id)
        {
            var band = await _bandService.GetBandByIdAsync(id);

            if (band == null)
            {
                return NotFound();
            }

            return Ok(band);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Band>> CreateBand(Band band)
        {
            var musicianId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (musicianId == null)
            {
                return Unauthorized(new { message = "Invalid token. Musician ID not found." });
            }

            try
            {
                var createdBand = await _bandService.CreateBandAsync(band, int.Parse(musicianId));
                return CreatedAtAction(nameof(GetBandById), new { id = createdBand.Id }, createdBand);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
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

            try
            {
                await _bandService.LeaveBandAsync(int.Parse(musicianIdClaim.Value));
                return Ok(new { message = "You have successfully left the band." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
