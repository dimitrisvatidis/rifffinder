using rifffinder.Models;
using rifffinder.Repositories;
using rifffinder.DTO;

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
    public async Task<CreateMusicianDTO> CreateMusicianAsync(CreateMusicianDTO musicianDto)
    {
        var existingMusician = await _musicianRepository.GetMusicianByEmailAsync(musicianDto.Email);
        if (existingMusician != null)
        {
            throw new InvalidOperationException("A musician with the same email already exists.");
        }

        if (string.IsNullOrWhiteSpace(musicianDto.Password))
        {
            throw new InvalidOperationException("Password cannot be empty.");
        }

        var musician = new Musician
        {
            Name = musicianDto.Name,
            Surname = musicianDto.Surname,
            Email = musicianDto.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(musicianDto.Password),
            Instrument = musicianDto.Instrument,
        };

        await _musicianRepository.CreateMusicianAsync(musician);
        return musicianDto;
    }


}

