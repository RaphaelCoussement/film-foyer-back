namespace AppliFilms.Api.Entities
{
    public class Movie
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string ImdbId { get; set; }  // identifiant unique OMDb
        public string? Title { get; set; }
        public string? Year { get; set; }
        public string? PosterUrl { get; set; }
        public string? Plot { get; set; }
        public string? Raw { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Relations
        public ICollection<Request> Requests { get; set; }
    }
}