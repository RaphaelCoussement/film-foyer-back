namespace AppliFilms.Api.DTOs.Reminders
{
    public class ReminderDto
    {
        public string MovieTitle { get; set; }
        public DateTime EventDate { get; set; }
        public int ApprovalCount { get; set; }
    }
}