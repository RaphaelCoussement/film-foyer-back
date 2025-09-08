using AppliFilms.Api.Data;
using AppliFilms.Api.Entities;
using AppliFilms.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AppliFilms.Api.Repositories
{
    public class RequestRepository(AppDbContext context) : IRequestRepository
    {
        public async Task<Request?> GetByIdAsync(Guid id) =>
            await context.Requests.Include(r => r.Movie).Include(r => r.RequestedBy).FirstOrDefaultAsync(r => r.Id == id);

        public async Task<Request?> GetByMovieAndDateAsync(Guid movieId, DateTime eventDate)
        {
            var dateOnly = eventDate.Date.ToUniversalTime();
            return await context.Requests
                .FirstOrDefaultAsync(r => r.MovieId == movieId && r.EventDate.Date == dateOnly);
        }

        public async Task<List<Request>> GetByDateAsync(DateTime eventDate)
        {
            var dateOnly = eventDate.Date;

            return await context.Requests
                .Include(r => r.Movie)
                .Include(r => r.RequestedBy)
                .Include(r => r.Approvals)
                .Where(r => r.EventDate >= dateOnly && r.EventDate < dateOnly.AddDays(1))
                .ToListAsync();
        }

        public async Task AddAsync(Request? request) =>
            await context.Requests.AddAsync(request);

        public Task RemoveAsync(Request? request) =>
            Task.FromResult(context.Requests.Remove(request));

        public async Task SaveChangesAsync() =>
            await context.SaveChangesAsync();
    }
}