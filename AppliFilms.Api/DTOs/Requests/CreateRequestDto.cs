using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AppliFilms.Api.DTOs.Requests
{
    public class CreateRequestDto
    {
        public Guid? MovieId { get; set; }      // pour film déjà en DB
        public int? TmdbId { get; set; }        // pour film à créer
    }
}