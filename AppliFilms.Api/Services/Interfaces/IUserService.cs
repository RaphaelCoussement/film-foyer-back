using AppliFilms.Api.DTOs.Movie;
using AppliFilms.Api.DTOs.User;

namespace AppliFilms.Api.Services.Interfaces;

public interface IUserService
{
    Task<UserDto> GetCurrentUserAsync(Guid userId);
    Task<UserDto> UpdateDisplayNameAsync(Guid userId, string newDisplayName);
    Task ChangePasswordAsync(Guid userId, string oldPassword, string newPassword);
    Task<IEnumerable<UserRequestDto>> GetUserRequestsAsync(Guid userId);
    Task AddFavoriteAsync(Guid userId, Guid movieId);
    Task RemoveFavoriteAsync(Guid userId, Guid movieId);
    Task<IEnumerable<MovieDto>> GetFavoritesAsync(Guid userId);

}