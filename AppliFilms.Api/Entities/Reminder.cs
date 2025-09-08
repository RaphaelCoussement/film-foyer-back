namespace AppliFilms.Api.Entities
{
    public class Reminder
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid RequestId { get; set; }
        public Request Request { get; set; }

        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public string Channel { get; set; } // ex: "whatsapp"
        public string Payload { get; set; } // JSON du message envoyé
    }
}