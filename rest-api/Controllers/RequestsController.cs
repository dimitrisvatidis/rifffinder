using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rifffinder.Data;
using rifffinder.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace rifffinder.Controllers
{
    [Route("api/[controller]")]
    [ApiController] 
    public class RequestsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RequestsController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RequestWithMusicianDto>>> GetRequests()
        {
            var musicianIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (musicianIdClaim == null)
            {
                return Unauthorized(new { message = "Musician ID not found in token." });
            }

            int musicianId = int.Parse(musicianIdClaim.Value);

            var musician = await _context.Musicians.FindAsync(musicianId);
            if (musician == null)
            {
                return BadRequest(new { message = "Musician not found." });
            }

            var requests = await _context.Requests
                .Where(r => r.MusicianId == musicianId || r.BandId == musician.BandId)
                .Select(r => new RequestWithMusicianDto
                {
                    Id = r.Id,
                    MusicianId = r.MusicianId,
                    BandId = r.BandId,
                    PostingId = r.PostingId,
                    Status = r.Status
                })
                .ToListAsync();

            return Ok(requests);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult> GetRequestById(int id)
        {
            var request = await _context.Requests.FindAsync(id);

            if (request == null)
            {
                return NotFound();
            }

            var response = new
            {
                request.Id,
                request.MusicianId,
                request.BandId,
                Status = request.Status.ToString()
            };

            return Ok(response);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateRequest([FromBody] Request request)
        {
            var musicianIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (musicianIdClaim == null)
            {
                return Unauthorized(new { message = "Musician ID not found in token." });
            }

            var musicianId = int.Parse(musicianIdClaim.Value);

            var posting = await _context.Postings.FindAsync(request.PostingId);
            if (posting == null)
            {
                return BadRequest(new { message = "Invalid PostingId." });
            }

            if (posting.BandId == musicianId)
            {
                return BadRequest(new { message = "You cannot request for your own band's posting." });
            }

            var existingRequest = await _context.Requests.FirstOrDefaultAsync(r =>
                r.PostingId == request.PostingId && r.MusicianId == musicianId);
            if (existingRequest != null)
            {
                return Conflict(new { message = "You have already requested for this posting." });
            }

            var newRequest = new Request
            {
                MusicianId = musicianId,
                BandId = posting.BandId,
                PostingId = request.PostingId,
                Status = RequestStatus.Pending
            };

            _context.Requests.Add(newRequest);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRequestById), new { id = newRequest.Id }, newRequest);
        }

        [Authorize]
        [HttpPatch("{id}/accept")]
        public async Task<IActionResult> AcceptRequest(int id)
        {
            var request = await _context.Requests.FindAsync(id);
            if (request == null)
            {
                return NotFound(new { message = "Request not found." });
            }

            request.Status = RequestStatus.Accepted;

            if (request.PostingId == null)
            {
                return BadRequest(new { message = "Request is not associated with any Posting." });
            }

            var posting = await _context.Postings.FindAsync(request.PostingId);
            if (posting == null)
            {
                return NotFound(new { message = "Associated Posting not found." });
            }

            posting.Status = PostingStatus.Closed;

            var musician = await _context.Musicians.FindAsync(request.MusicianId);
            if (musician == null)
            {
                return NotFound(new { message = "Musician not found." });
            }

            musician.BandId = request.BandId;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Request has been accepted, the associated posting has been closed, and the musician's band has been updated.",
                request,
                posting,
                musician
            });
        }

        [Authorize]
        [HttpPatch("{id}/deny")]
        public async Task<IActionResult> DenyRequest(int id)
        {
            var request = await _context.Requests.FindAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            request.Status = RequestStatus.Denied;
            _context.Entry(request).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return Ok(new
            {
                message = "Request has been denied.",
                request.Id,
                request.MusicianId,
                request.BandId,
                Status = request.Status.ToString()
            });
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRequest(int id)
        {
            var request = await _context.Requests.FindAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            _context.Requests.Remove(request);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
