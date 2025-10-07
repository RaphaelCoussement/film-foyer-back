using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AppliFilms.Api.Entities;

public class WishlistItem
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; } = Guid.NewGuid();

    [BsonRepresentation(BsonType.String)]
    public Guid UserId { get; set; }

    [BsonRepresentation(BsonType.String)]
    public Guid MovieId { get; set; }

    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}