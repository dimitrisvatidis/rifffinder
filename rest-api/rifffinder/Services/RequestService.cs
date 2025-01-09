using rifffinder.Models;
using rifffinder.Repositories;
using rifffinder.DTOs;
namespace rifffinder.Services
{
    public class RequestService
    {
        private readonly RequestRepository _requestRepository;
        private readonly PostingRepository _postingRepository;
        private readonly MusicianRepository _musicianRepository;

        public RequestService(RequestRepository requestRepository, PostingRepository postingRepository, MusicianRepository musicianRepository)
        {
            _requestRepository = requestRepository;
            _postingRepository = postingRepository;
            _musicianRepository = musicianRepository;
        }

        public async Task<IEnumerable<RequestWithMusicianDTO>> GetRequestsAsync(int musicianId, int? bandId)
        {
            var requests = await _requestRepository.GetRequestsAsync(musicianId, bandId);
            return requests.Select(r => new RequestWithMusicianDTO
            {
                Id = r.Id,
                MusicianId = r.MusicianId,
                BandId = r.BandId,
                PostingId = r.PostingId,
                Status = r.Status
            });
        }

        public async Task<RequestWithMusicianDTO?> GetRequestByIdAsync(int id)
        {
            var request = await _requestRepository.GetRequestByIdAsync(id);
            if (request == null) return null;

            return new RequestWithMusicianDTO
            {
                Id = request.Id,
                MusicianId = request.MusicianId,
                BandId = request.BandId,
                PostingId = request.PostingId,
                Status = request.Status
            };
        }

        public async Task<RequestWithMusicianDTO> CreateRequestAsync(int musicianId, CreateRequestDTO requestDto)
        {
            var posting = await _postingRepository.GetPostingByIdAsync(requestDto.PostingId);
            if (posting == null) throw new InvalidOperationException("Invalid PostingId.");

            var musician = await _musicianRepository.GetMusicianByIdAsync(musicianId);
            if (musician == null)
            {
                throw new InvalidOperationException("Musician not found.");
            }

            if (musician.BandId != null)
            {
                throw new InvalidOperationException("You cannot send a request while already belonging to a band.");
            }

            var existingRequest = await _requestRepository.GetRequestsByPostingIdAndMusicianIdAsync(requestDto.PostingId, musicianId);
            if (existingRequest != null)
            {
                throw new InvalidOperationException("You have already sent a request for this posting.");
            }

            var newRequest = new Request
            {
                MusicianId = musicianId,
                BandId = posting.BandId,
                PostingId = requestDto.PostingId,
                Status = RequestStatus.Pending
            };

            await _requestRepository.CreateRequestAsync(newRequest);

            return new RequestWithMusicianDTO
            {
                Id = newRequest.Id,
                MusicianId = newRequest.MusicianId,
                BandId = newRequest.BandId,
                PostingId = newRequest.PostingId,
                Status = newRequest.Status
            };
        }

        public async Task AcceptRequestAsync(int id, int musicianId)
        {
            var request = await _requestRepository.GetRequestByIdAsync(id);
            if (request == null) throw new InvalidOperationException("Request not found.");

            var currentUser = await _musicianRepository.GetMusicianByIdAsync(musicianId);
            if (currentUser == null || currentUser.BandId != request.BandId)
            {
                throw new UnauthorizedAccessException("You are not authorized to accept this request.");
            }

            request.Status = RequestStatus.Accepted;
            await _requestRepository.UpdateRequestAsync(request);

            if (request.PostingId != null)
            {
                var posting = await _postingRepository.GetPostingByIdAsync(request.PostingId.Value);
                if (posting != null)
                {
                    posting.Status = PostingStatus.Closed;
                    await _postingRepository.UpdatePostingAsync(posting);
                }
            }

            var musician = await _musicianRepository.GetMusicianByIdAsync(request.MusicianId);
            if (musician != null)
            {
                musician.BandId = request.BandId;
                await _musicianRepository.UpdateMusicianAsync(musician);
            }
        }

        public async Task DenyRequestAsync(int id, int musicianId)
        {
            var request = await _requestRepository.GetRequestByIdAsync(id);
            if (request == null) throw new InvalidOperationException("Request not found.");

            var currentUser = await _musicianRepository.GetMusicianByIdAsync(musicianId);
            if (currentUser == null || currentUser.BandId != request.BandId)
            {
                throw new UnauthorizedAccessException("You are not authorized to deny this request.");
            }

            request.Status = RequestStatus.Denied;
            await _requestRepository.UpdateRequestAsync(request);
        }
    }
}