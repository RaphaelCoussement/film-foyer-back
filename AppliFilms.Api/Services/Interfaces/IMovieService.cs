using AppliFilms.Api.DTOs.Movie;

namespace AppliFilms.Api.Services.Interfaces
{
    public interface IMovieService
    {
        Task<MovieDto> GetMovieByTitleAsync(string title);
        Task<List<MovieSearchResultDto>> SearchMoviesAsync(string title);
    }
}