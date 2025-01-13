using rifffinder.Models;
using rifffinder.Data;
using rifffinder.Repositories;
using Microsoft.EntityFrameworkCore;
namespace rifffinder.Services
{
    public class BandService
    {
        private readonly BandRepository _bandRepository;
        private readonly MusicianRepository _musicianRepository;
        private readonly RequestRepository _requestRepository;
        private readonly AppDbContext _context;

        public BandService(BandRepository bandRepository, AppDbContext context, MusicianRepository musicianRepository, RequestRepository requestRepository)
        {
            _bandRepository = bandRepository;
            _musicianRepository = musicianRepository;
            _requestRepository = requestRepository;
            _context = context;
        }

        public async Task<Band?> GetBandByIdAsync(int id)
        {
            return await _bandRepository.GetBandByIdAsync(id);
        }

        public async Task<Band> CreateBandAsync(Band band, int musicianId)
        {
            var musician = await _musicianRepository.GetMusicianByIdAsync(musicianId);
            if (musician == null || musician.BandId.HasValue)
            {
                throw new InvalidOperationException("Invalid musician or already in a band.");
            }

            // Delete all pending requests for the musician
            await _requestRepository.DeleteRequestsByMusicianIdAsync(musicianId);

            // Create the band using the repository
            var createdBand = await _bandRepository.CreateBandAsync(band);

            // Associate the musician with the newly created band
            musician.BandId = createdBand.Id;
            await _musicianRepository.UpdateMusicianAsync(musician);

            return createdBand;
        }


        public async Task LeaveBandAsync(int musicianId)
        {
            var musician = await _musicianRepository.GetMusicianByIdAsync(musicianId);
            if (musician == null || musician.BandId == null)
            {
                throw new InvalidOperationException("Musician is not part of a band.");
            }

            int bandId = musician.BandId.Value;

            musician.BandId = null;
            await _musicianRepository.UpdateMusicianAsync(musician);

            // Check if there are any musicians left in the band
            var remainingMusicians = await _musicianRepository.GetMusiciansByBandIdAsync(bandId);
            if (!remainingMusicians.Any())
            {
                await _bandRepository.DeleteBandAsync(bandId);
            }
        }


    }
}