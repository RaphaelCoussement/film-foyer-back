using AppliFilms.Api.Data.Mongo;
using AppliFilms.Api.Entities;
using AppliFilms.Api.Repositories.Interfaces;
using MongoDB.Driver;

namespace AppliFilms.Api.Repositories
{
    public class RequestRepository : IRequestRepository
    {
        private readonly IMongoCollection<Request> _requests;

        public RequestRepository(MongoDbService mongoService)
        {
            _requests = mongoService.GetCollection<Request>("Requests");
        }

        public async Task<Request?> GetByIdAsync(Guid id) =>
            await _requests.Find(r => r.Id == id).FirstOrDefaultAsync();

        public async Task<Request?> GetByMovieAsync(Guid movieId) =>
            await _requests.Find(r => r.MovieId == movieId).FirstOrDefaultAsync();

        public async Task<List<Request>> GetAllAsync() =>
            await _requests.Find(_ => true).ToListAsync();

        public async Task AddAsync(Request? request)
        {
            if (request == null) return;
            await _requests.InsertOneAsync(request);
        }

        public async Task RemoveAsync(Request? request)
        {
            if (request == null) return;
            await _requests.DeleteOneAsync(r => r.Id == request.Id);
        }
    }
}