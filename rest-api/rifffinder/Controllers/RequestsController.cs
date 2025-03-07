﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rifffinder.Services;
using rifffinder.DTOs;
using rifffinder.Models;
using System.Security.Claims;

namespace rifffinder.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RequestsController : ControllerBase
    {
        private readonly RequestService _requestService;
        private readonly MusicianService _musicianService;

        public RequestsController(RequestService requestService, MusicianService musicianService)
        {
            _requestService = requestService;
            _musicianService = musicianService;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RequestWithMusicianDTO>>> GetRequests()
        {
            var musicianIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (musicianIdClaim == null)
            {
                return Unauthorized(new { message = "Musician ID not found in token." });
            }

            int musicianId = int.Parse(musicianIdClaim.Value);

            var musician = await _musicianService.GetMusicianByIdAsync(musicianId);
            if (musician == null)
            {
                return BadRequest(new { message = "Musician not found." });
            }

            var requests = await _requestService.GetRequestsAsync(musicianId, musician.BandId);
            return Ok(requests);
        }

        private async Task<ActionResult<RequestWithMusicianDTO>> GetRequestById(int id)
        {
            var request = await _requestService.GetRequestByIdAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            return Ok(request);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateRequest([FromBody] CreateRequestDTO requestDto)
        {
            var musicianIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (musicianIdClaim == null)
            {
                return Unauthorized(new { message = "Musician ID not found in token." });
            }

            int musicianId = int.Parse(musicianIdClaim.Value);
            var createdRequest = await _requestService.CreateRequestAsync(musicianId, requestDto);
            return Ok(new { message = "Request created." });
        }

        [Authorize]
        [HttpPatch("{id}/accept")]
        public async Task<IActionResult> AcceptRequest(int id)
        {
            var musicianIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (musicianIdClaim == null)
            {
                return Unauthorized(new { message = "Musician ID not found in token." });
            }

            await _requestService.AcceptRequestAsync(id, int.Parse(musicianIdClaim));
            return Ok(new { message = "Request accepted." });
        }

        [Authorize]
        [HttpPatch("{id}/deny")]
        public async Task<IActionResult> DenyRequest(int id)
        {
            var musicianIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (musicianIdClaim == null)
            {
                return Unauthorized(new { message = "Musician ID not found in token." });
            }

            await _requestService.DenyRequestAsync(id, int.Parse(musicianIdClaim));
            return Ok(new { message = "Request denied." });
        }
    }
}
