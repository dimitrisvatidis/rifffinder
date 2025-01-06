using rifffinder.Models;
using rifffinder.Data;
using rifffinder.Repositories;
namespace rifffinder.Services
{
    public class BandService
    {
        private readonly BandRepository _bandRepository;
        private readonly AppDbContext _context;

        public BandService(BandRepository bandRepository, AppDbContext context)
        {
            _bandRepository = bandRepository;
            _context = context;
        }

        public async Task<Band?> GetBandByIdAsync(int id)
        {
            return await _bandRepository.GetBandByIdAsync(id);
        }

        public async Task<Band> CreateBandAsync(Band band, int musicianId)
        {
            var musician = await _context.Musicians.FindAsync(musicianId);
            if (musician == null || musician.BandId.HasValue)
            {
                throw new InvalidOperationException("Invalid musician or already in a band.");
            }

            var createdBand = await _bandRepository.CreateBandAsync(band);
            musician.BandId = createdBand.Id;
            await _context.SaveChangesAsync();
            return createdBand;
        }

        public async Task LeaveBandAsync(int musicianId)
        {
            var musician = await _context.Musicians.FindAsync(musicianId);
            if (musician == null || musician.BandId == null)
            {
                throw new InvalidOperationException("Musician is not part of a band.");
            }

            musician.BandId = null;
            await _context.SaveChangesAsync();
        }
    }
}