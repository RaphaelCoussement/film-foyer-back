using AppliFilms.Api.Data.Mongo;
using AppliFilms.Api.Entities;
using AppliFilms.Api.Repositories.Interfaces;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace AppliFilms.Api.Repositories
{
    // Implémentation Mongo
    public class MovieRepository : IMovieRepository
    {
        private readonly IMongoCollection<Movie> _movies;

        public MovieRepository(MongoDbService mongoService)
        {
            _movies = mongoService.GetCollection<Movie>("Movies");
        }

        public async Task<Movie?> GetByIdAsync(Guid id) =>
            await _movies.Find(m => m.Id == id).FirstOrDefaultAsync();

        public async Task<Movie?> GetByTitleAsync(string title) =>
            await _movies.Find(m => m.Title == title).FirstOrDefaultAsync();

        public async Task AddAsync(Movie? movie)
        {
            if (movie == null) return;
            await _movies.InsertOneAsync(movie);
        }

        public Task SaveChangesAsync() => Task.CompletedTask;
    }
}