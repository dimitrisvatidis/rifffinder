using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rifffinder.Services;
using rifffinder.DTOs;
using rifffinder.DTO;

namespace rifffinder.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostingsController : ControllerBase
    {
        private readonly PostingService _postingService;

        public PostingsController(PostingService postingService)
        {
            _postingService = postingService;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PostingDTO>>> GetAllPostings()
        {
            var postings = await _postingService.GetAllPostingsAsync();
            return Ok(postings);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<PostingDTO>> GetPostingById(int id)
        {
            var posting = await _postingService.GetPostingByIdAsync(id);
            if (posting == null)
            {
                return NotFound();
            }

            return Ok(posting);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<PostingDTO>> CreatePosting(CreatePostingDTO postingDto)
        {
            var createdPosting = await _postingService.CreatePostingAsync(postingDto);
            return CreatedAtAction(nameof(GetPostingById), new { id = createdPosting.Id }, createdPosting);
        }
    }
}
