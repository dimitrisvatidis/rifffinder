using rifffinder.Models;
using rifffinder.Data;
using Microsoft.EntityFrameworkCore;
namespace rifffinder.Repositories
{
    public class RequestRepository
    {
        private readonly AppDbContext _context;

        public RequestRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Request>> GetRequestsAsync(int musicianId, int? bandId)
        {
            return await _context.Requests
                .Where(r => r.MusicianId == musicianId || r.BandId == bandId)
                .ToListAsync();
        }

        public async Task<Request?> GetRequestByIdAsync(int id)
        {
            return await _context.Requests.FindAsync(id);
        }

        public async Task CreateRequestAsync(Request request)
        {
            _context.Requests.Add(request);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRequestAsync(Request request)
        {
            _context.Entry(request).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteRequestAsync(int id)
        {
            var request = await _context.Requests.FindAsync(id);
            if (request != null)
            {
                _context.Requests.Remove(request);
                await _context.SaveChangesAsync();
            }
        }
    }
}