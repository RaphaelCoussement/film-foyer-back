using AppliFilms.Api.Entities;

namespace AppliFilms.Api.Repositories.Interfaces
{
    public interface IRequestRepository
    {
        Task<Request?> GetByIdAsync(Guid id);
        Task<Request> GetByMovieAndDateAsync(Guid movieId, DateTime eventDate);
        Task<List<Request>> GetByDateAsync(DateTime eventDate);
        Task AddAsync(Request? request);
        Task RemoveAsync(Request? request);
        Task SaveChangesAsync();
    }
}