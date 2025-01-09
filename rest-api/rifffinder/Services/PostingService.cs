using rifffinder.Models;
using rifffinder.Repositories;
using rifffinder.DTOs;
using rifffinder.DTO;
using System.Security.Claims;
namespace rifffinder.Services
{
    public class PostingService
    {
        private readonly PostingRepository _postingRepository;
        private readonly MusicianRepository _musicianRepository;

        public PostingService(PostingRepository postingRepository, MusicianRepository musicianRepository)
        {
            _postingRepository = postingRepository;
            _musicianRepository = musicianRepository;
        }

        public async Task<IEnumerable<PostingDTO>> GetAllPostingsAsync()
        {
            var postings = await _postingRepository.GetAllPostingsAsync();
            return postings.Select(p => new PostingDTO
            {
                Id = p.Id,
                Title = p.Title,
                Text = p.Text,
                InstrumentWanted = p.InstrumentWanted,
                BandId = p.BandId,
                BandName = p.Band != null ? p.Band.Name : "Unknown",
                Status = p.Status
            });
        }

        public async Task<PostingDTO?> GetPostingByIdAsync(int id)
        {
            var posting = await _postingRepository.GetPostingByIdAsync(id);
            if (posting == null) return null;

            return new PostingDTO
            {
                Id = posting.Id,
                Title = posting.Title,
                Text = posting.Text,
                InstrumentWanted = posting.InstrumentWanted,
                BandId = posting.BandId,
                BandName = posting.Band != null ? posting.Band.Name : "Unknown",
                Status = posting.Status
            };
        }

        public async Task<PostingDTO> CreatePostingAsync(CreatePostingDTO postingDto, int musicianId)
        {
            var musician = await _musicianRepository.GetMusicianByIdAsync(musicianId);
            if (musician == null || musician.BandId == null || musician.BandId != postingDto.BandId)
            {
                throw new InvalidOperationException("You must belong to the band to create a posting.");
            }

            var posting = new Posting
            {
                Title = postingDto.Title,
                Text = postingDto.Text,
                InstrumentWanted = postingDto.InstrumentWanted,
                BandId = postingDto.BandId,
                Status = PostingStatus.Open
            };

            await _postingRepository.CreatePostingAsync(posting);

            return new PostingDTO
            {
                Id = posting.Id,
                Title = posting.Title,
                Text = posting.Text,
                InstrumentWanted = posting.InstrumentWanted,
                BandId = posting.BandId,
                BandName = "Unknown",
                Status = posting.Status
            };
        }
    }
}