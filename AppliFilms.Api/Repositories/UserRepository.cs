using AppliFilms.Api.Data;
using AppliFilms.Api.Entities;
using AppliFilms.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AppliFilms.Api.Repositories
{
    public class UserRepository(AppDbContext context) : IUserRepository
    {
        public async Task<User?> GetByIdAsync(Guid id) =>
            await context.Users.FindAsync(id);

        public async Task<User> GetByEmailAsync(string email) =>
            await context.Users.FirstOrDefaultAsync(u => u.Email == email);

        public async Task AddAsync(User? user) =>
            await context.Users.AddAsync(user);

        public async Task SaveChangesAsync() =>
            await context.SaveChangesAsync();
    }
}