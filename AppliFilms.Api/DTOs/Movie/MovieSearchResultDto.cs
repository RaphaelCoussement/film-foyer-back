namespace AppliFilms.Api.DTOs.Movie;

public class MovieSearchResultDto
{
    public int TmdbId { get; set; }
    public string Title { get; set; }
    public string Overview { get; set; }
    public string ReleaseDate { get; set; }
    public string PosterUrl { get; set; }
}