using AppliFilms.Api.Entities;

namespace AppliFilms.Api.Repositories.Interfaces
{
    public interface IRequestRepository
    {
        Task<Request?> GetByIdAsync(Guid id);
        Task<Request?> GetByMovieAsync(Guid movieId);
        Task<List<Request>> GetAllAsync();
        Task AddAsync(Request? request);
        Task RemoveAsync(Request? request);
    }
}