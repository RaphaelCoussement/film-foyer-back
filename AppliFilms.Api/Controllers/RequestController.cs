using AppliFilms.Api.DTOs;
using AppliFilms.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AppliFilms.Api.DTOs.Requests;

namespace AppliFilms.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RequestController : ControllerBase
    {
        private readonly IRequestService _requestService;

        public RequestController(IRequestService requestService)
        {
            _requestService = requestService;
        }

        private Guid GetUserId() =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRequestDto dto)
        {
            try
            {
                var request = await _requestService.CreateRequestAsync(dto, GetUserId());
                return Ok(request);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var requests = await _requestService.GetAllRequestsAsync();
            return Ok(requests);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _requestService.DeleteRequestAsync(id, GetUserId());
            if (!result) return BadRequest(new { message = "Impossible de supprimer cette demande" });
            return NoContent();
        }
        
        [HttpDelete("clear")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ClearAllRequests()
        {
            try
            {
                await _requestService.DeleteAllRequestsAsync();
                return Ok(new { message = "Toutes les demandes ont été supprimées avec succès." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}