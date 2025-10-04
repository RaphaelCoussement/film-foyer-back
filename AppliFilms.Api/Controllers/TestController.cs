using AppliFilms.Api.Data.Mongo;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace AppliFilms.Api.Controllers
{
    [ApiController]
    [Route("api/test")]
    public class TestController : ControllerBase
    {
        private readonly MongoDbService _mongoService;

        public TestController(MongoDbService mongoService)
        {
            _mongoService = mongoService;
        }
        
        [HttpGet]
        public IActionResult Get() => Ok("Alive");

        [HttpGet("mongodb")]
        public IActionResult CheckMongoConnection()
        {
            try
            {
                var movies = _mongoService.GetCollection<BsonDocument>("Movies");
                var count = movies.CountDocuments(FilterDefinition<BsonDocument>.Empty);

                return Ok(new { message = "Connexion à MongoDB OK", movieCount = count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur de connexion à MongoDB", error = ex.Message });
            }
        }
    }
}