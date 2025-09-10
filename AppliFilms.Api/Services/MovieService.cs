using System.Net.Http.Headers;
using AppliFilms.Api.DTOs.Movie;
using AppliFilms.Api.Services.Interfaces;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

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

        // 2. Récupérer les détails complets
        var detailsUrl = $"https://api.themoviedb.org/3/movie/{movie.Id}";
        var details = await _httpClient.GetFromJsonAsync<TmdbMovieDetails>(detailsUrl);

        if (details == null)
            throw new Exception("Impossible de récupérer les détails du film");

        // 3. Retourner le DTO correctement
        return new MovieDto
        {
            ImdbId = details.Id.ToString(), // vrai IMDb ID
            Title = details.Title,   // original_title
            PosterUrl = string.IsNullOrEmpty(details.PosterPath)
                ? null
                : $"https://image.tmdb.org/t/p/w500{details.PosterPath}",
            Plot = details.Overview,
            Year = !string.IsNullOrEmpty(details.ReleaseDate)
                ? DateTime.Parse(details.ReleaseDate).Year.ToString()
                : null
        };
    }

    // === Classes internes pour System.Text.Json ===
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