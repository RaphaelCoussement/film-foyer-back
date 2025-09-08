using AppliFilms.Api.Data;
using AppliFilms.Api.Entities;
using AppliFilms.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AppliFilms.Api.Repositories
{
    public class MovieRepository(AppDbContext context) : IMovieRepository
    {
        public async Task<Movie?> GetByImdbIdAsync(string imdbId) =>
            await context.Movies.FirstOrDefaultAsync(m => m.ImdbId == imdbId);
        
        public async Task<Movie?> GetByTitleAsync(string title) =>
            await context.Movies.FirstOrDefaultAsync(m => m.Title == title);

        public async Task AddAsync(Movie? movie) =>
            await context.Movies.AddAsync(movie);

        public async Task SaveChangesAsync() =>
            await context.SaveChangesAsync();
    }
}