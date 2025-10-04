using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AppliFilms.Api.Entities
{
    public class Movie
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string ImdbId { get; set; }  // identifiant unique OMDb
        public string? Title { get; set; }
        public string? Year { get; set; }
        public string? PosterUrl { get; set; }
        public string? Plot { get; set; }
        public int? Duration { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}