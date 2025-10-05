using AppliFilms.Api.DTOs.User;
using AppliFilms.Api.Repositories.Interfaces;
using AppliFilms.Api.Services.Interfaces;
using System.Security.Cryptography;
using System.Text;
using AppliFilms.Api.DTOs.Movie;

namespace AppliFilms.Api.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRequestRepository _requestRepository;
        private readonly IMovieRepository _movieRepository;

        public UserService(IUserRepository userRepository, IRequestRepository requestRepository, IMovieRepository movieRepository)
        {
            _userRepository = userRepository;
            _requestRepository = requestRepository;
            _movieRepository = movieRepository;
        }

        public async Task<UserDto> GetCurrentUserAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new Exception("Utilisateur introuvable.");

            return new UserDto
            {
                Id = user.Id,
                DisplayName = user.DisplayName,
                Email = user.Email,
                IsAdmin = user.IsAdmin
            };
        }

        public async Task<UserDto> UpdateDisplayNameAsync(Guid userId, string newDisplayName)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new Exception("Utilisateur introuvable.");

            user.DisplayName = newDisplayName;
            await _userRepository.SaveChangesAsync(user);

            return new UserDto
            {
                Id = user.Id,
                DisplayName = user.DisplayName,
                Email = user.Email,
                IsAdmin = user.IsAdmin
            };
        }
        
        public async Task ChangePasswordAsync(Guid userId, string oldPassword, string newPassword)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new Exception("Utilisateur introuvable.");

            // Vérifie l'ancien mot de passe avec BCrypt
            if (!BCrypt.Net.BCrypt.Verify(oldPassword, user.PasswordHash))
                throw new Exception("Ancien mot de passe incorrect.");

            // Hache le nouveau mot de passe avec BCrypt
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);

            await _userRepository.SaveChangesAsync(user);
        }
        
        public async Task<IEnumerable<UserRequestDto>> GetUserRequestsAsync(Guid userId)
        {
            var requests = await _requestRepository.GetByUserIdAsync(userId);

            var result = new List<UserRequestDto>();

            foreach (var r in requests)
            {
                var movie = await _movieRepository.GetByIdAsync(r.MovieId);
                result.Add(new UserRequestDto
                {
                    Id = r.Id,
                    MovieTitle = movie?.Title ?? "Film inconnu",
                    PosterUrl = movie?.PosterUrl ?? "",
                    CreatedAt = r.CreatedAt,
                });
            }

            return result;
        }

        // Utilitaires de hachage
        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
        
        public async Task AddFavoriteAsync(Guid userId, Guid movieId)
        {
            var user = await _userRepository.GetByIdAsync(userId)
                       ?? throw new Exception("Utilisateur introuvable.");

            if (!user.FavoriteMovieIds.Contains(movieId))
            {
                user.FavoriteMovieIds.Add(movieId);
                await _userRepository.SaveChangesAsync(user);
            }
        }

        public async Task RemoveFavoriteAsync(Guid userId, Guid movieId)
        {
            var user = await _userRepository.GetByIdAsync(userId)
                       ?? throw new Exception("Utilisateur introuvable.");

            if (user.FavoriteMovieIds.Contains(movieId))
            {
                user.FavoriteMovieIds.Remove(movieId);
                await _userRepository.SaveChangesAsync(user);
            }
        }

        public async Task<IEnumerable<MovieDto>> GetFavoritesAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId)
                       ?? throw new Exception("Utilisateur introuvable.");

            var favorites = new List<MovieDto>();

            foreach (var movieId in user.FavoriteMovieIds)
            {
                var movie = await _movieRepository.GetByIdAsync(movieId);
                if (movie != null)
                {
                    favorites.Add(new MovieDto
                    {
                        Id = movie.Id,
                        ImdbId = movie.ImdbId,
                        Title = movie.Title,
                        PosterUrl = movie.PosterUrl,
                        Plot = movie.Plot,
                        Year = movie.Year,
                        Duration = movie.Duration
                    });
                }
            }

            return favorites;
        }
    }
}