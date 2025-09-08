using AppliFilms.Api.DTOs.Reminders;

namespace AppliFilms.Api.Services.Interfaces
{
    public interface IReminderService
    {
        Task <ReminderDto> SendReminderAsync(DateTime eventDate);
    }
}
