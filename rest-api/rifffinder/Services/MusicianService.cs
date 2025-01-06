using rifffinder.Models;
using rifffinder.Repositories;

public class MusicianService 
{
    private readonly MusicianRepository _musicianRepository;
    public MusicianService(MusicianRepository musicianRepository)
    {
        _musicianRepository = musicianRepository;
    }

    public async Task<MusicianDTO?> GetMusicianByIdAsync(int id)
    {
        var musician = await _musicianRepository.GetMusicianByIdAsync(id);
        return musician != null ? new MusicianDTO
        {
            Id = musician.Id,
            Name = musician.Name,
            Surname = musician.Surname,
            Email = musician.Email,
            Instrument = musician.Instrument,
            BandId = musician.BandId
        } : null;
    }

    public async Task<IEnumerable<MusicianDTO>> GetMusiciansByBandIdAsync(int bandId)
    {
        var musicians = await _musicianRepository.GetMusiciansByBandIdAsync(bandId);
        return musicians.Select(m => new MusicianDTO
        {
            Id = m.Id,
            Name = m.Name,
            Surname = m.Surname,
            Email = m.Email,
            Instrument = m.Instrument,
            BandId = m.BandId
        });
    }

    public async Task<MusicianDTO> CreateMusicianAsync(MusicianDTO musicianDto)
    {
        var musician = new Musician
        {
            Name = musicianDto.Name,
            Surname = musicianDto.Surname,
            Email = musicianDto.Email,
            Password = BCrypt.Net.BCrypt.HashPassword("default"),
            Instrument = musicianDto.Instrument,
            BandId = musicianDto.BandId
        };
        await _musicianRepository.CreateMusicianAsync(musician);
        return musicianDto;
    }
}

