using AppliFilms.Api.Entities;

public interface IMovieRepository
{
    Task<Movie?> GetByIdAsync(Guid id);
    Task<Movie?> GetByTitleAsync(string title);
    Task<Movie?> GetByTmdbIdAsync(int tmdbId);
    Task AddAsync(Movie? movie);
    Task SaveChangesAsync();
}