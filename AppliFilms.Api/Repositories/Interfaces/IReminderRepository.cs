using AppliFilms.Api.Entities;

namespace AppliFilms.Api.Repositories.Interfaces
{
    public interface IReminderRepository
    {
        Task AddAsync(Reminder reminder);
        Task<List<Reminder>> GetByDateAsync(DateTime date);
        Task SaveChangesAsync();
    }
}