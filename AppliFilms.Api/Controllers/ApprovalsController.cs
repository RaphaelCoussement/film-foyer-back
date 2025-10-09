using System.Security.Claims;
using AppliFilms.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppliFilms.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApprovalsController : ControllerBase
{
    private readonly IApprovalService _approvalService;

    public ApprovalsController(IApprovalService approvalService)
    {
        _approvalService = approvalService;
    }

    [HttpPost("{requestId}")]
    [Authorize]
    public async Task<IActionResult> Approve(Guid requestId)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var approval = await _approvalService.ApproveRequestAsync(requestId, userId);

            return Ok(approval);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    
    [HttpDelete("{requestId}")]
    [Authorize]
    public async Task<IActionResult> Unapprove(Guid requestId)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            await _approvalService.UnapproveRequestAsync(requestId, userId);
            return Ok(new { message = "Vote retiré avec succès" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}