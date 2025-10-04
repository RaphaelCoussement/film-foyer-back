using AppliFilms.Api.Entities;

public interface IMovieRepository
{
    Task<Movie?> GetByIdAsync(Guid id);
    Task<Movie?> GetByTitleAsync(string title);
    Task AddAsync(Movie? movie);
    Task SaveChangesAsync();
}