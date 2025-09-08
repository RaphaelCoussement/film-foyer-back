using AppliFilms.Api.DTOs;
using AppliFilms.Api.DTOs.Movie;
using AppliFilms.Api.DTOs.Requests;
using AppliFilms.Api.Entities;
using AppliFilms.Api.Repositories.Interfaces;
using AppliFilms.Api.Services.Interfaces;

namespace AppliFilms.Api.Services
{
    public class RequestService(IRequestRepository requestRepository, IMovieRepository movieRepository, IUserRepository userRepository, IMovieService movieService)
        : IRequestService
    {
        public async Task<RequestDto> CreateRequestAsync(CreateRequestDto dto, Guid userId)
        {
            // Vérifier si le film existe déjà en DB
            var movie = await movieRepository.GetByTitleAsync(dto.Title);
            if (movie == null)
            {
                // Appel OMDb API pour récupérer les infos
                var movieDto = await movieService.GetMovieByTitleAsync(dto.Title);

                movie = new Movie
                {
                    Id = Guid.NewGuid(),
                    ImdbId = movieDto.ImdbId,
                    Title = movieDto.Title,
                    Plot = movieDto.Plot,
                    PosterUrl = movieDto.PosterUrl,
                    Year = movieDto.Year
                };

                await movieRepository.AddAsync(movie);
                await movieRepository.SaveChangesAsync();
            }

            // Ne garder que la date (heure = 00:00 UTC)
            var eventDateUtc = dto.EventDate.Date.ToUniversalTime();

            // Vérifier si une demande pour ce film existe déjà ce jour
            var existingRequest = await requestRepository.GetByMovieAndDateAsync(movie.Id, eventDateUtc);

            if (existingRequest != null)
                throw new Exception("Vous ou quelqu’un a déjà demandé ce film pour cette période");

            // Créer la demande
            var request = new Request
            {
                Id = Guid.NewGuid(),
                MovieId = movie.Id,
                RequestedById = userId,
                EventDate = eventDateUtc,
                CreatedAt = DateTime.UtcNow
            };

            await requestRepository.AddAsync(request);
            await requestRepository.SaveChangesAsync();

            var user = await userRepository.GetByIdAsync(userId);

            return new RequestDto
            {
                Id = request.Id,
                EventDate = request.EventDate,
                CreatedAt = request.CreatedAt,
                RequestedBy = user?.DisplayName ?? "Inconnu",
                ApprovalCount = 0,
                Movie = new MovieDto
                {
                    ImdbId = movie.ImdbId,
                    Title = movie.Title,
                    PosterUrl = movie.PosterUrl,
                    Plot = movie.Plot
                }
            };
        }

        public async Task<List<RequestDto>> GetRequestsByDateAsync(DateTime eventDate)
        {
            var requests = await requestRepository.GetByDateAsync(eventDate);

            return requests.Select(r => new RequestDto
            {
                Id = r.Id,
                EventDate = r.EventDate,
                CreatedAt = r.CreatedAt,
                RequestedBy = r.RequestedBy?.DisplayName ?? "Inconnu",
                ApprovalCount = r.Approvals?.Count ?? 0,
                Movie = new MovieDto
                {
                    ImdbId = r.Movie.ImdbId,
                    Title = r.Movie.Title,
                    PosterUrl = r.Movie.PosterUrl
                }
            })
            .OrderByDescending(r => r.ApprovalCount)
            .ToList();
        }

        public async Task<bool> DeleteRequestAsync(Guid requestId, Guid userId)
        {
            var request = await requestRepository.GetByIdAsync(requestId);
            if (request == null || request.RequestedById != userId) return false;

            requestRepository.RemoveAsync(request);
            await requestRepository.SaveChangesAsync();
            return true;
        }
    }
}
