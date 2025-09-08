namespace AppliFilms.Api.Entities
{
    public class Request
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid MovieId { get; set; }
        public Movie Movie { get; set; }

        public Guid? RequestedById { get; set; }
        public User RequestedBy { get; set; }

        public DateTime EventDate { get; set; }  // la date du samedi concerné
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Relations
        public ICollection<Approval> Approvals { get; set; }
    }
}