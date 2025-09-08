using AppliFilms.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AppliFilms.Api.Controllers;

[ApiController]
[Route("api/test")]
public class TestController : ControllerBase
{
    private readonly EmailService _emailService;

    public TestController(EmailService emailService)
    {
        _emailService = emailService;
    }

    [HttpGet("send-mail")]
    public async Task<IActionResult> SendMail()
    {
        await _emailService.SendEmailAsync(
            "Test Mail AppliFilms",
            "Ceci est un mail de test envoyé via Mailtrap !"
        );

        return Ok("Mail envoyé (check Mailtrap) !");
    }
}
