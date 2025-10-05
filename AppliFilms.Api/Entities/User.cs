using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AppliFilms.Api.Entities
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string DisplayName { get; set; }
        public bool IsAdmin { get; set; } = false;
        
        [BsonRepresentation(BsonType.String)]
        public List<Guid> FavoriteMovieIds { get; set; } = new();
    }
}