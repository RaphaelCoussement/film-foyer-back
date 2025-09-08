namespace AppliFilms.Api.Entities
{
    public class Approval
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid RequestId { get; set; }
        public Request Request { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}