using AppliFilms.Api.Data.Mongo;
using AppliFilms.Api.Entities;
using AppliFilms.Api.Repositories.Interfaces;
using MongoDB.Driver;

namespace AppliFilms.Api.Repositories;

public class WishlistRepository : IWishlistRepository
{
    private readonly IMongoCollection<WishlistItem> _wishlist;

    public WishlistRepository(MongoDbService mongoService)
    {
        _wishlist = mongoService.GetCollection<WishlistItem>("Wishlist");
    }

    public async Task<List<WishlistItem>> GetByUserAsync(Guid userId) =>
        await _wishlist.Find(x => x.UserId == userId).ToListAsync();

    public async Task<WishlistItem> GetByUserAndMovieAsync(Guid userId, Guid movieId) =>
        await _wishlist.Find(x => x.UserId == userId && x.MovieId == movieId).FirstOrDefaultAsync();

    public async Task AddAsync(WishlistItem item) =>
        await _wishlist.InsertOneAsync(item);

    public async Task RemoveAsync(WishlistItem item) =>
        await _wishlist.DeleteOneAsync(x => x.Id == item.Id);

    public async Task SaveChangesAsync() { }
}