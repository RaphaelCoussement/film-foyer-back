using AppliFilms.Api.DTOs;
using AppliFilms.Api.DTOs.Movie;
using AppliFilms.Api.DTOs.Requests;
using AppliFilms.Api.Entities;
using AppliFilms.Api.Repositories.Interfaces;
using AppliFilms.Api.Services.Interfaces;

namespace AppliFilms.Api.Services
{
    public class RequestService : IRequestService
    {
        private readonly IRequestRepository _requestRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMovieService _movieService;

        public RequestService(
            IRequestRepository requestRepository,
            IMovieRepository movieRepository,
            IUserRepository userRepository,
            IMovieService movieService)
        {
            _requestRepository = requestRepository;
            _movieRepository = movieRepository;
            _userRepository = userRepository;
            _movieService = movieService;
        }

        public async Task<RequestDto> CreateRequestAsync(CreateRequestDto dto, Guid userId)
        {
            // Vérifier si le film existe déjà en DB
            var existingMovie = await _movieRepository.GetByTitleAsync(dto.Title);

            // Vérifier si une demande pour ce film existe déjà
            if (existingMovie != null)
            {
                var existingRequest = await _requestRepository.GetByMovieAsync(existingMovie.Id);
                if (existingRequest != null)
                    throw new Exception("Ce film a déjà été demandé");
            }

            // Créer le film seulement s'il n'existe pas
            Movie movie;
            if (existingMovie == null)
            {
                var movieDto = await _movieService.GetMovieByTitleAsync(dto.Title);

                movie = new Movie
                {
                    Id = Guid.NewGuid(),
                    ImdbId = movieDto.ImdbId,
                    Title = movieDto.Title,
                    Plot = movieDto.Plot,
                    PosterUrl = movieDto.PosterUrl,
                    Year = movieDto.Year,
                    Duration = movieDto.Duration // si tu as ajouté la durée
                };

                await _movieRepository.AddAsync(movie);
            }
            else
            {
                movie = existingMovie;
            }

            // Créer la demande
            var request = new Request
            {
                Id = Guid.NewGuid(),
                MovieId = movie.Id,
                RequestedById = userId,
                CreatedAt = DateTime.UtcNow,
                ApprovalIds = Array.Empty<Guid>()
            };

            await _requestRepository.AddAsync(request);

            var user = await _userRepository.GetByIdAsync(userId);

            return new RequestDto
            {
                Id = request.Id,
                CreatedAt = request.CreatedAt,
                RequestedBy = user?.DisplayName ?? "Inconnu",
                ApprovalCount = 0,
                Movie = new MovieDto
                {
                    ImdbId = movie.ImdbId,
                    Title = movie.Title,
                    PosterUrl = movie.PosterUrl,
                    Plot = movie.Plot,
                    Year = movie.Year,
                    Duration = movie.Duration // si tu l'as ajouté
                }
            };
        }


        public async Task<List<RequestDto>> GetAllRequestsAsync()
        {
            var requests = await _requestRepository.GetAllAsync();

            var result = new List<RequestDto>();
            foreach (var r in requests)
            {
                var movie = await _movieRepository.GetByIdAsync(r.MovieId);
                var user = await _userRepository.GetByIdAsync(r.RequestedById ?? Guid.Empty);

                result.Add(new RequestDto
                {
                    Id = r.Id,
                    CreatedAt = r.CreatedAt,
                    RequestedBy = user?.DisplayName ?? "Inconnu",
                    ApprovalCount = r.ApprovalIds.Length,
                    Movie = new MovieDto
                    {
                        ImdbId = movie?.ImdbId,
                        Title = movie?.Title,
                        PosterUrl = movie?.PosterUrl,
                        Plot = movie?.Plot,
                        Year = movie?.Year,
                        Duration = movie?.Duration
                    }
                });
            }

            return result
                .OrderByDescending(r => r.ApprovalCount)
                .ThenBy(r => r.CreatedAt)
                .ToList();
        }

        public async Task<bool> DeleteRequestAsync(Guid requestId, Guid userId)
        {
            var request = await _requestRepository.GetByIdAsync(requestId);
            if (request == null || request.RequestedById != userId) return false;

            await _requestRepository.RemoveAsync(request);
            return true;
        }
        
        public async Task DeleteAllRequestsAsync()
        {
            await _requestRepository.DeleteAllAsync();
        }

    }
}