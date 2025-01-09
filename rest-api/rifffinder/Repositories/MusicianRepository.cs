using rifffinder.Models;
using rifffinder.Data;
using Microsoft.EntityFrameworkCore;
namespace rifffinder.Repositories;
public class MusicianRepository
{
    private readonly AppDbContext _context;
    public MusicianRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Musician?> GetMusicianByEmailAsync(string email)
    {
        return await _context.Musicians.FirstOrDefaultAsync(m => m.Email == email);
    }

    public async Task<Musician?> GetMusicianByIdAsync(int id)
    {
        return await _context.Musicians.FindAsync(id);
    }

    public async Task<IEnumerable<Musician>> GetMusiciansByBandIdAsync(int bandId)
    {
        return await _context.Musicians.Where(m => m.BandId == bandId).ToListAsync();
    }

    public async Task<Musician> CreateMusicianAsync(Musician musician)
    {
        _context.Musicians.Add(musician);
        await _context.SaveChangesAsync();
        return musician;
    }


    public async Task UpdateMusicianAsync(Musician musician)
    {
        _context.Entry(musician).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }
}
