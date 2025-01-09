using rifffinder.Models;
using rifffinder.Data;
using Microsoft.EntityFrameworkCore;
namespace rifffinder.Repositories
{
    public class PostingRepository
    {
        private readonly AppDbContext _context;

        public PostingRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Posting>> GetAllPostingsAsync()
        {
            return await _context.Postings.Include(p => p.Band).ToListAsync();
        }

        public async Task<Posting?> GetPostingByIdAsync(int id)
        {
            return await _context.Postings.Include(p => p.Band).FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task CreatePostingAsync(Posting posting)
        {
            _context.Postings.Add(posting);
            await _context.SaveChangesAsync();
        }

         public async Task UpdatePostingAsync(Posting posting)
        {
            _context.Entry(posting).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}