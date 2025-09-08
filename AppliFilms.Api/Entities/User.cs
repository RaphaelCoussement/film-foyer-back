namespace AppliFilms.Api.Entities
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string DisplayName { get; set; }
        public bool IsAdmin { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Relations
        public ICollection<Request> Requests { get; set; }
        public ICollection<Approval> Approvals { get; set; }
    }
}