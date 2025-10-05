using System.Net.Http.Headers;
using AppliFilms.Api.DTOs.Movie;
using AppliFilms.Api.Entities;
using AppliFilms.Api.Repositories.Interfaces;
using AppliFilms.Api.Services.Interfaces;
using System.Text.Json.Serialization;

namespace AppliFilms.Api.Services
{
    public class MovieService : IMovieService
    {
        private readonly HttpClient _httpClient;
        private readonly string _bearerToken;
        private readonly IMovieRepository _movieRepository;

        public MovieService(HttpClient httpClient, IConfiguration configuration, IMovieRepository movieRepository)
        {
            _httpClient = httpClient;
            _bearerToken = configuration["TMDb:BearerToken"];
            _movieRepository = movieRepository;

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _bearerToken);
        }

        public async Task<MovieDto> GetMovieByTitleAsync(string title)
        {
            // Vérifie si le film existe déjà en base
            var existingMovie = await _movieRepository.GetByTitleAsync(title);
            if (existingMovie != null)
            {
                return new MovieDto
                {
                    ImdbId = existingMovie.ImdbId,
                    Title = existingMovie.Title,
                    PosterUrl = existingMovie.PosterUrl,
                    Plot = existingMovie.Plot,
                    Year = existingMovie.Year,
                    Duration = existingMovie.Duration
                };
            }

            // 1️⃣ Recherche du film sur TMDb
            var searchUrl = $"https://api.themoviedb.org/3/search/movie?query={Uri.EscapeDataString(title)}&language=fr";
            var searchResponse = await _httpClient.GetFromJsonAsync<TmdbSearchResponse>(searchUrl);

            var movieResult = searchResponse?.Results?.FirstOrDefault();
            if (movieResult == null)
                throw new Exception($"Aucun film trouvé pour '{title}' sur TMDb.");

            // 2️⃣ Récupération des détails du film (inclut le runtime)
            var detailsUrl = $"https://api.themoviedb.org/3/movie/{movieResult.Id}?language=fr";
            var details = await _httpClient.GetFromJsonAsync<TmdbMovieDetails>(detailsUrl);

            if (details == null)
                throw new Exception($"Impossible de récupérer les détails du film '{title}'.");

            // 3️⃣ Création et sauvegarde de l'entité Movie
            var movieEntity = new Movie
            {
                Id = Guid.NewGuid(),
                ImdbId = details.ImdbId ?? details.Id.ToString(),
                Title = details.Title,
                PosterUrl = string.IsNullOrEmpty(details.PosterPath)
                    ? null
                    : $"https://image.tmdb.org/t/p/w500{details.PosterPath}",
                Plot = details.Overview,
                Year = !string.IsNullOrEmpty(details.ReleaseDate)
                    ? DateTime.Parse(details.ReleaseDate).Year.ToString()
                    : null,
                Duration = details.Runtime ?? 0, // ✅ Runtime garanti non nul
                CreatedAt = DateTime.UtcNow
            };

            await _movieRepository.AddAsync(movieEntity);
            await _movieRepository.SaveChangesAsync();

            // 4️⃣ Retourne un DTO propre
            return new MovieDto
            {
                ImdbId = movieEntity.ImdbId,
                Title = movieEntity.Title,
                PosterUrl = movieEntity.PosterUrl,
                Plot = movieEntity.Plot,
                Year = movieEntity.Year,
                Duration = movieEntity.Duration
            };
        }
        
        public async Task<List<MovieSearchResultDto>> SearchMoviesAsync(string title)
        {
            var searchUrl = $"https://api.themoviedb.org/3/search/movie?query={Uri.EscapeDataString(title)}&language=fr";
            var searchResponse = await _httpClient.GetFromJsonAsync<TmdbSearchResponse>(searchUrl);

            if (searchResponse?.Results == null || !searchResponse.Results.Any())
                return new List<MovieSearchResultDto>();

            return searchResponse.Results
                .Take(10)
                .Select(m => new MovieSearchResultDto
                {
                    TmdbId = m.Id,
                    Title = m.Title,
                    Overview = m.Overview,
                    ReleaseDate = m.ReleaseDate,
                    PosterUrl = string.IsNullOrEmpty(m.PosterPath)
                        ? null
                        : $"https://image.tmdb.org/t/p/w500{m.PosterPath}"
                })
                .ToList();
        }
        
        public async Task<MovieDto> GetMovieByTmdbIdAsync(int tmdbId)
        {
            // Vérifie si le film existe déjà en base
            var existingMovie = await _movieRepository.GetByTmdbIdAsync(tmdbId);
            if (existingMovie != null)
            {
                return new MovieDto
                {
                    ImdbId = existingMovie.ImdbId,
                    Title = existingMovie.Title,
                    PosterUrl = existingMovie.PosterUrl,
                    Plot = existingMovie.Plot,
                    Year = existingMovie.Year,
                    Duration = existingMovie.Duration
                };
            }

            // 1️⃣ Récupération des détails TMDb
            var detailsUrl = $"https://api.themoviedb.org/3/movie/{tmdbId}?language=fr";
            var details = await _httpClient.GetFromJsonAsync<TmdbMovieDetails>(detailsUrl);

            if (details == null)
                throw new Exception($"Impossible de récupérer les détails du film TMDb #{tmdbId}.");

            // 2️⃣ Création et sauvegarde de l'entité Movie
            var movieEntity = new Movie
            {
                Id = Guid.NewGuid(),
                TmdbId = details.Id,
                ImdbId = details.ImdbId ?? details.Id.ToString(),
                Title = details.Title,
                PosterUrl = string.IsNullOrEmpty(details.PosterPath)
                    ? null
                    : $"https://image.tmdb.org/t/p/w500{details.PosterPath}",
                Plot = details.Overview,
                Year = !string.IsNullOrEmpty(details.ReleaseDate)
                    ? DateTime.Parse(details.ReleaseDate).Year.ToString()
                    : null,
                Duration = details.Runtime ?? 0,
                CreatedAt = DateTime.UtcNow
            };

            await _movieRepository.AddAsync(movieEntity);
            await _movieRepository.SaveChangesAsync();

            // 3️⃣ Retourne un DTO
            return new MovieDto
            {
                ImdbId = movieEntity.ImdbId,
                Title = movieEntity.Title,
                PosterUrl = movieEntity.PosterUrl,
                Plot = movieEntity.Plot,
                Year = movieEntity.Year,
                Duration = movieEntity.Duration
            };
        }
        
        private class TmdbSearchResponse
        {
            [JsonPropertyName("results")]
            public List<TmdbMovieResult> Results { get; set; }
        }

        private class TmdbMovieResult
        {
            [JsonPropertyName("id")]
            public int Id { get; set; }

            [JsonPropertyName("title")]
            public string Title { get; set; }

            [JsonPropertyName("release_date")]
            public string ReleaseDate { get; set; }

            [JsonPropertyName("overview")]
            public string Overview { get; set; }

            [JsonPropertyName("poster_path")]
            public string PosterPath { get; set; }
        }

        private class TmdbMovieDetails
        {
            [JsonPropertyName("id")]
            public int Id { get; set; }

            [JsonPropertyName("title")]
            public string Title { get; set; }

            [JsonPropertyName("release_date")]
            public string ReleaseDate { get; set; }

            [JsonPropertyName("overview")]
            public string Overview { get; set; }

            [JsonPropertyName("poster_path")]
            public string PosterPath { get; set; }

            [JsonPropertyName("imdb_id")]
            public string ImdbId { get; set; }

            [JsonPropertyName("runtime")]
            public int? Runtime { get; set; }
        }
    }
}