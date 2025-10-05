using AppliFilms.Api.DTOs.User;
using AppliFilms.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AppliFilms.Api.DTOs.Movie;

namespace AppliFilms.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        private Guid GetUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.Parse(userIdClaim!);
        }

        [HttpGet("me")]
        public async Task<ActionResult<UserDto>> GetProfile()
        {
            var userId = GetUserId();
            var user = await _userService.GetCurrentUserAsync(userId);
            return Ok(user);
        }

        [HttpPut("displayName")]
        public async Task<ActionResult<UserDto>> UpdateDisplayName([FromBody] UpdateDisplayNameDto dto)
        {
            var userId = GetUserId();
            var updated = await _userService.UpdateDisplayNameAsync(userId, dto.NewDisplayName);
            return Ok(updated);
        }

        [HttpPut("password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var userId = GetUserId();
            await _userService.ChangePasswordAsync(userId, dto.OldPassword, dto.NewPassword);
            return NoContent();
        }
        
        [HttpGet("requests")]
        public async Task<IActionResult> GetUserRequests()
        {
            var userId = GetUserId();
            var requests = await _userService.GetUserRequestsAsync(userId);
            return Ok(requests);
        }
        
        [HttpPost("favorites")]
        public async Task<IActionResult> AddFavorite([FromBody] FavoriteMovieDto favorite)
        {
            var userId = GetUserId();
            await _userService.AddFavoriteAsync(userId, favorite.MovieId);
            return NoContent();
        }

        [HttpDelete("favorites/{movieId}")]
        public async Task<IActionResult> RemoveFavorite(Guid movieId)
        {
            var userId = GetUserId();
            await _userService.RemoveFavoriteAsync(userId, movieId);
            return NoContent();
        }

        [HttpGet("favorites")]
        public async Task<ActionResult<IEnumerable<MovieDto>>> GetFavorites()
        {
            var userId = GetUserId();
            var favorites = await _userService.GetFavoritesAsync(userId);
            return Ok(favorites);
        }
    }
}