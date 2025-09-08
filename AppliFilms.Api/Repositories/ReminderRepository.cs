using AppliFilms.Api.Data;
using AppliFilms.Api.Entities;
using AppliFilms.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AppliFilms.Api.Repositories
{
    public class ReminderRepository(AppDbContext context) : IReminderRepository
    {
        public async Task AddAsync(Reminder reminder) =>
            await context.Reminders.AddAsync(reminder);

        public async Task<List<Reminder>> GetByDateAsync(DateTime date) =>
            await context.Reminders
                .Include(r => r.Request)
                .ThenInclude(req => req.Movie)
                .Where(r => r.SentAt.Date == date.Date)
                .ToListAsync();

        public async Task SaveChangesAsync() =>
            await context.SaveChangesAsync();
    }
}