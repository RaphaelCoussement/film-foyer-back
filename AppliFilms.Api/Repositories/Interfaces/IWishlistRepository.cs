using AppliFilms.Api.Entities;

namespace AppliFilms.Api.Repositories.Interfaces;

public interface IWishlistRepository
{
    Task<List<WishlistItem>> GetByUserAsync(Guid userId);
    Task<WishlistItem> GetByUserAndMovieAsync(Guid userId, Guid movieId);
    Task AddAsync(WishlistItem item);
    Task RemoveAsync(WishlistItem item);
    Task SaveChangesAsync();
}
