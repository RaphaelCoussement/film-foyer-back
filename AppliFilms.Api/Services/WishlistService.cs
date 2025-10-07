using AppliFilms.Api.DTOs.Requests;
using AppliFilms.Api.DTOs.Wishlist;
using AppliFilms.Api.Entities;
using AppliFilms.Api.Repositories.Interfaces;
using AppliFilms.Api.Services.Interfaces;

namespace AppliFilms.Api.Services;

public class WishlistService : IWishlistService
{
    private readonly IWishlistRepository _wishlistRepository;
    private readonly IMovieRepository _movieRepository;
    private readonly IMovieService _movieService;
    private readonly IRequestService _requestService;

    public WishlistService(
        IWishlistRepository wishlistRepository,
        IMovieRepository movieRepository,
        IMovieService movieService, 
        IRequestService requestService)
    {
        _wishlistRepository = wishlistRepository;
        _movieRepository = movieRepository;
        _movieService = movieService;
        _requestService = requestService;
    }

    public async Task<List<WishlistItemDto>> GetUserWishlistAsync(Guid userId)
    {
        var items = await _wishlistRepository.GetByUserAsync(userId);
        var result = new List<WishlistItemDto>();

        foreach (var item in items)
        {
            var movie = await _movieRepository.GetByIdAsync(item.MovieId);
            if (movie == null) continue;

            result.Add(new WishlistItemDto
            {
                Id = item.Id,
                MovieId = movie.Id,
                Year = movie.Year,
                Duration = movie.Duration,
                Title = movie.Title,
                PosterUrl = movie.PosterUrl,
                AddedAt = item.AddedAt
            });
        }

        return result.OrderByDescending(x => x.AddedAt).ToList();
    }

    public async Task<WishlistItemDto> AddToWishlistAsync(AddToWishlistDto dto, Guid userId)
    {
        Movie movie = null;

        if (dto.MovieId.HasValue)
        {
            movie = await _movieRepository.GetByIdAsync(dto.MovieId.Value);
            if (movie == null)
                throw new Exception("Le film n'existe pas.");
        }
        else if (dto.TmdbId.HasValue)
        {
            movie = await _movieRepository.GetByTmdbIdAsync(dto.TmdbId.Value);
            if (movie == null)
            {
                var movieDto = await _movieService.GetMovieByTmdbIdAsync(dto.TmdbId.Value);

                movie = new Movie
                {
                    Id = Guid.NewGuid(),
                    ImdbId = movieDto.ImdbId,
                    Title = movieDto.Title,
                    PosterUrl = movieDto.PosterUrl,
                    Plot = movieDto.Plot,
                    Year = movieDto.Year,
                    Duration = movieDto.Duration,
                    CreatedAt = DateTime.UtcNow
                };

                await _movieRepository.AddAsync(movie);
                await _movieRepository.SaveChangesAsync();
            }
        }
        else
            throw new Exception("MovieId ou TmdbId requis pour ajouter à la wishlist.");

        // Vérifier doublon
        var existing = await _wishlistRepository.GetByUserAndMovieAsync(userId, movie.Id);
        if (existing != null)
            throw new Exception("Ce film est déjà dans ta wishlist.");

        var item = new WishlistItem
        {
            Id = Guid.NewGuid(),
            MovieId = movie.Id,
            UserId = userId,
            AddedAt = DateTime.UtcNow
        };

        await _wishlistRepository.AddAsync(item);

        return new WishlistItemDto
        {
            Id = item.Id,
            MovieId = movie.Id,
            Title = movie.Title,
            PosterUrl = movie.PosterUrl,
            AddedAt = item.AddedAt
        };
    }

    public async Task<bool> RemoveFromWishlistAsync(Guid movieId, Guid userId)
    {
        var item = await _wishlistRepository.GetByUserAndMovieAsync(userId, movieId);
        if (item == null) return false;

        await _wishlistRepository.RemoveAsync(item);
        return true;
    }
    
    public async Task<RequestDto> SuggestWishlistItemAsync(SuggestWishlistItemDto dto, Guid userId)
    {
        // 1️⃣ Vérifier que le film est bien dans la wishlist
        var wishlistItem = await _wishlistRepository.GetByUserAndMovieAsync(userId, dto.MovieId);
        if (wishlistItem == null)
            throw new Exception("Le film n'est pas dans votre wishlist");

        // 2️⃣ Créer une demande pour ce film via RequestService
        var requestDto = new CreateRequestDto
        {
            MovieId = dto.MovieId
        };

        var request = await _requestService.CreateRequestAsync(requestDto, userId);

        // 3️⃣ Retirer le film de la wishlist
        await _wishlistRepository.RemoveAsync(wishlistItem);

        return request;
    }
}