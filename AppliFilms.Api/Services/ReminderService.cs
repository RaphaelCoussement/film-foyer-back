using AppliFilms.Api.DTOs.Reminders;
using AppliFilms.Api.Repositories.Interfaces;
using AppliFilms.Api.Services.Interfaces;

namespace AppliFilms.Api.Services
{
    public class ReminderService : IReminderService
    {
        private readonly IRequestRepository _requestRepository;
        private readonly EmailService _emailService;

        public ReminderService(IRequestRepository requestRepository, EmailService emailService)
        {
            _requestRepository = requestRepository;
            _emailService = emailService;
        }

        public async Task<ReminderDto> SendReminderAsync(DateTime eventDate)
        {
            var requests = await _requestRepository.GetByDateAsync(eventDate.Date);
            if (!requests.Any()) return null;

            var winner = requests
                .OrderByDescending(r => r.Approvals?.Count ?? 0)
                .FirstOrDefault();

            if (winner == null) return null;

            var message = $"[Reminder] Le film choisi pour {eventDate:dd/MM} est {winner.Movie.Title} !";
            await _emailService.SendEmailAsync("Rappel Film AppliFilms", message);

            return new ReminderDto
            {
                MovieTitle = winner.Movie.Title,
                EventDate = winner.EventDate,
                ApprovalCount = winner.Approvals?.Count ?? 0
            };
        }
    }
}