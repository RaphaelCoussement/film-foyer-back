using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AppliFilms.Api.Entities
{
    public class Request
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; } = Guid.NewGuid();

        [BsonRepresentation(BsonType.String)]
        public Guid MovieId { get; set; }
        [BsonRepresentation(BsonType.String)]
        public Guid? RequestedById { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonRepresentation(BsonType.String)]
        public Guid[] ApprovalIds { get; set; } = Array.Empty<Guid>();
    }
}