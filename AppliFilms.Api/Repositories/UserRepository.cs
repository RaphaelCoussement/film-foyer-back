using AppliFilms.Api.Data.Mongo;
using AppliFilms.Api.Entities;
using AppliFilms.Api.Repositories.Interfaces;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace AppliFilms.Api.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _users;

        public UserRepository(MongoDbService mongoService)
        {
            _users = mongoService.GetCollection<User>("Users");
        }

        public async Task<User?> GetByIdAsync(Guid id) =>
            await _users.Find(u => u.Id == id).FirstOrDefaultAsync();

        public async Task<User> GetByEmailAsync(string email) =>
            await _users.Find(u => u.Email == email).FirstOrDefaultAsync();

        public async Task AddAsync(User? user)
        {
            if (user == null) return;
            await _users.InsertOneAsync(user);
        }
        
        public async Task SaveChangesAsync(User user)
        {
            await _users.ReplaceOneAsync(u => u.Id == user.Id, user);
        }

        public Task SaveChangesAsync() => Task.CompletedTask;
    }
}