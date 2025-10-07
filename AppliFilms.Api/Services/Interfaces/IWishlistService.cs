using AppliFilms.Api.DTOs.Requests;
using AppliFilms.Api.DTOs.Wishlist;

namespace AppliFilms.Api.Services.Interfaces;

public interface IWishlistService
{
    Task<List<WishlistItemDto>> GetUserWishlistAsync(Guid userId);
    Task<WishlistItemDto> AddToWishlistAsync(AddToWishlistDto dto, Guid userId);
    Task<bool> RemoveFromWishlistAsync(Guid movieId, Guid userId);
    Task<RequestDto> SuggestWishlistItemAsync(SuggestWishlistItemDto dto, Guid userId);
}
