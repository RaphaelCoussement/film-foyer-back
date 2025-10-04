namespace AppliFilms.Api.DTOs.Movie
{
    public class MovieDto
    {
        public string ImdbId { get; set; }
        public string? Title { get; set; }
        public string? Year { get; set; }
        public string? PosterUrl { get; set; }
        public string? Plot { get; set; }
        public int? Duration { get; set; }
    }
}