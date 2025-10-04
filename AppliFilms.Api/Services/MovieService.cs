using System.Net.Http.Headers;
using AppliFilms.Api.DTOs.Movie;
using AppliFilms.Api.Entities;
using AppliFilms.Api.Repositories.Interfaces;
using AppliFilms.Api.Services.Interfaces;
using System.Text.Json.Serialization;
using GTranslate.Translators;

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
            // Vérifier si le film existe déjà en base
            var existingMovie = await _movieRepository.GetByTitleAsync(title);
            if (existingMovie != null)
            {
                return new MovieDto
                {
                    ImdbId = existingMovie.ImdbId,
                    Title = existingMovie.Title,
                    PosterUrl = existingMovie.PosterUrl,
                    Plot = existingMovie.Plot,
                    Year = existingMovie.Year
                };
            }

            // Rechercher le film sur TMDb
            var searchUrl = $"https://api.themoviedb.org/3/search/movie?query={Uri.EscapeDataString(title)}";
            var searchResponse = await _httpClient.GetFromJsonAsync<TmdbSearchResponse>(searchUrl);

            var movie = searchResponse?.Results?.FirstOrDefault();
            if (movie == null)
                throw new Exception("Film non trouvé sur TMDb");
            
            var detailsUrl = $"https://api.themoviedb.org/3/movie/{movie.Id}";
            var details = await _httpClient.GetFromJsonAsync<TmdbMovieDetails>(detailsUrl);

            if (details == null)
                throw new Exception("Impossible de récupérer les détails du film");
            
            string translatedPlot = null;
            if (!string.IsNullOrEmpty(details.Overview))
            {
                var translator = new GoogleTranslator();
                var result = await translator.TranslateAsync(details.Overview, "fr");
                translatedPlot = result.Translation;
            }

            // Créer l'entité Movie
            var movieEntity = new Movie
            {
                Id = Guid.NewGuid(),
                ImdbId = details.ImdbId ?? details.Id.ToString(),
                Title = details.Title,
                PosterUrl = string.IsNullOrEmpty(details.PosterPath) ? null : $"https://image.tmdb.org/t/p/w500{details.PosterPath}",
                Plot = translatedPlot ?? details.Overview,
                Year = !string.IsNullOrEmpty(details.ReleaseDate) ? DateTime.Parse(details.ReleaseDate).Year.ToString() : null,
                CreatedAt = DateTime.UtcNow
            };

            // Ajouter dans MongoDB
            await _movieRepository.AddAsync(movieEntity);
            await _movieRepository.SaveChangesAsync();

            return new MovieDto
            {
                ImdbId = movieEntity.ImdbId,
                Title = movieEntity.Title,
                PosterUrl = movieEntity.PosterUrl,
                Plot = movieEntity.Plot,
                Year = movieEntity.Year
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

            [JsonPropertyName("original_title")]
            public string Title { get; set; }

            [JsonPropertyName("release_date")]
            public string ReleaseDate { get; set; }

            [JsonPropertyName("overview")]
            public string Overview { get; set; }

            [JsonPropertyName("poster_path")]
            public string PosterPath { get; set; }

            [JsonPropertyName("imdb_id")]
            public string ImdbId { get; set; }
        }
    }
}