using AppliFilms.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AppliFilms.Api.DTOs.Wishlist;

namespace AppliFilms.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _wishlistService;

        public WishlistController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        private Guid GetUserId() =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        [HttpGet]
        public async Task<IActionResult> GetMyWishlist()
        {
            var userId = GetUserId();
            var wishlist = await _wishlistService.GetUserWishlistAsync(userId);
            return Ok(wishlist);
        }

        [HttpPost]
        public async Task<IActionResult> AddToWishlist([FromBody] AddToWishlistDto dto)
        {
            var userId = GetUserId();
            var item = await _wishlistService.AddToWishlistAsync(dto, userId);
            return Ok(item);
        }

        [HttpDelete("{movieId}")]
        public async Task<IActionResult> RemoveFromWishlist(Guid movieId)
        {
            var userId = GetUserId();
            var success = await _wishlistService.RemoveFromWishlistAsync(movieId, userId);
            if (!success) return NotFound(new { message = "Film non trouvé dans ta wishlist." });
            return NoContent();
        }
        
        [HttpPost("suggest")]
        public async Task<IActionResult> SuggestMovie([FromBody] SuggestWishlistItemDto dto)
        {
            try
            {
                var userId = GetUserId();
                var request = await _wishlistService.SuggestWishlistItemAsync(dto, userId);
                return Ok(request);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}