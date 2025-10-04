using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AppliFilms.Api.Entities
{
    public class Approval
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; } = Guid.NewGuid();

        [BsonRepresentation(BsonType.String)]
        public Guid RequestId { get; set; }
        
        [BsonRepresentation(BsonType.String)]
        public Guid UserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}