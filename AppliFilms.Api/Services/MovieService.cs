using AppliFilms.Api.DTOs.Movie;
using AppliFilms.Api.Services.Interfaces;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;

namespace AppliFilms.Api.Services
{
    public class MovieService : IMovieService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public MovieService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["OMDb:ApiKey"]; // ajouter dans appsettings.json
        }

        public async Task<MovieDto> GetMovieByTitleAsync(string title)
        {
            var url = $"https://www.omdbapi.com/?apikey={_apiKey}&t={Uri.EscapeDataString(title)}";
            var response = await _httpClient.GetFromJsonAsync<OmdbResponse>(url);

            if (response == null || response.Response != "True")
                throw new Exception("Film non trouvé sur OMDb");

            return new MovieDto
            {
                ImdbId = response.imdbID,
                Title = response.Title,
                PosterUrl = response.Poster,
                Plot = response.Plot,
                Year = response.Year
            };
        }

        private class OmdbResponse
        {
            public string Title { get; set; }
            public string Plot { get; set; }
            public string Poster { get; set; }
            public string imdbID { get; set; }
            public string Response { get; set; }
            public string? Year { get; set; }
        }
    }
}