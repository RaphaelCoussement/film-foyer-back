using AppliFilms.Api.Entities;

namespace AppliFilms.Api.Repositories.Interfaces
{
    public interface IMovieRepository
    {
        Task<Movie?> GetByImdbIdAsync(string imdbId);
        Task<Movie?> GetByTitleAsync(string title);
        Task AddAsync(Movie? movie);
        Task SaveChangesAsync();
    }
}