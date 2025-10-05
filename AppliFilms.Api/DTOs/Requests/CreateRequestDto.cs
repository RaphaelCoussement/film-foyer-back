using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AppliFilms.Api.DTOs.Requests
{
    public class CreateRequestDto
    {
        [BsonRepresentation(BsonType.String)]
        public Guid MovieId { get; set; }
    }
}