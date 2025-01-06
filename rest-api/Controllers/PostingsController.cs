using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rifffinder.Data; 
using rifffinder.Models;

namespace rifffinder.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostingsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PostingsController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize]
            [HttpGet]
            public async Task<ActionResult<IEnumerable<object>>> GetAllPostings()
            {
                var postings = await _context.Postings
                    .Include(p => p.Band)
                    .Select(p => new
                    {
                        p.Id,
                        p.Title,
                        p.Text,
                        p.InstrumentWanted,
                        p.Status,
                        p.BandId,
                        BandName = p.Band != null ? p.Band.Name : "Unknown"
                    })
                    .ToListAsync();

                return Ok(postings);
            }


        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Posting>> GetPostingById(int id)
        {
            var posting = await _context.Postings.FindAsync(id);

            if (posting == null)
            {
                return NotFound();
            }

            return posting;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Posting>> CreatePosting(Posting posting)
        {
            // Check if the band exists
            var band = await _context.Bands.FindAsync(posting.BandId);
            if (band == null)
            {
                return BadRequest("Band does not exist.");
            }

            _context.Postings.Add(posting);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPostingById), new { id = posting.Id }, posting);
        }
    }
}
