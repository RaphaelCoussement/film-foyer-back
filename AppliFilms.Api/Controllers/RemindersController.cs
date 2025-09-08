using AppliFilms.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AppliFilms.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RemindersController : ControllerBase
{
    private readonly IReminderService _reminderService;

    public RemindersController(IReminderService reminderService)
    {
        _reminderService = reminderService;
    }

    [HttpGet("send")]
    public async Task<IActionResult> SendReminder()
    {
        var today = DateTime.UtcNow.Date;
        var reminder = await _reminderService.SendReminderAsync(today);

        if (reminder == null)
            return NotFound(new { message = "Aucun film trouvé pour aujourd'hui" });

        return Ok(reminder);
    }
}
