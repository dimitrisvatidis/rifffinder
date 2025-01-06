using rifffinder.Models;
using rifffinder.Data;
using Microsoft.EntityFrameworkCore;
namespace rifffinder.Repositories
{
    public class BandRepository
    {
        private readonly AppDbContext _context;
        public BandRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Band?> GetBandByIdAsync(int id)
        {
            return await _context.Bands.FindAsync(id);
        }

        public async Task<Band> CreateBandAsync(Band band)
        {
            _context.Bands.Add(band);
            await _context.SaveChangesAsync();
            return band;
        }
    }
}