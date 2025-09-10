using System.Net.Http.Headers;
using AppliFilms.Api.DTOs.Movie;
using AppliFilms.Api.Services.Interfaces;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;

namespace AppliFilms.Api.Services
{
    public class MovieService : IMovieService
{
    private readonly HttpClient _httpClient;
    private readonly string _bearerToken;

    public MovieService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _bearerToken = configuration["TMDb:BearerToken"];

        // Ajouter le header Authorization à chaque requête
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", _bearerToken);
    }

    public async Task<MovieDto> GetMovieByTitleAsync(string title)
    {
        // 1. Rechercher le film
        var searchUrl = $"https://api.themoviedb.org/3/search/movie?query={Uri.EscapeDataString(title)}";
        var searchResponse = await _httpClient.GetFromJsonAsync<TmdbSearchResponse>(searchUrl);

        var movie = searchResponse?.Results?.FirstOrDefault();
        if (movie == null)
            throw new Exception("Film non trouvé sur TMDb");

        // 2. Obtenir les détails complets (inclut imdb_id)
        var detailsUrl = $"https://api.themoviedb.org/3/movie/{movie.Id}";
        var details = await _httpClient.GetFromJsonAsync<TmdbMovieDetails>(detailsUrl);

        if (details == null)
            throw new Exception("Impossible de récupérer les détails du film");

        return new MovieDto
        {
            ImdbId = details.Id.ToString(),
            Title = details.Title,
            PosterUrl = string.IsNullOrEmpty(details.PosterPath) ? null : $"https://image.tmdb.org/t/p/w500{details.PosterPath}",
            Plot = details.Overview,
            Year = !string.IsNullOrEmpty(details.ReleaseDate) ? DateTime.Parse(details.ReleaseDate).Year.ToString() : null
        };
    }

    // --- Classes pour la désérialisation ---
    private class TmdbSearchResponse
    {
        public List<TmdbMovieResult> Results { get; set; }
    }

    private class TmdbMovieResult
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ReleaseDate { get; set; }
        public string Overview { get; set; }
        public string PosterPath { get; set; }
    }

    private class TmdbMovieDetails
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ReleaseDate { get; set; }
        public string Overview { get; set; }
        public string PosterPath { get; set; }
        public string ImdbId { get; set; }
    }
}
}