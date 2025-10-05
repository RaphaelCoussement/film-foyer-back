namespace AppliFilms.Api.DTOs.User;

public class UserRequestDto
{
    public Guid Id { get; set; }
    public string MovieTitle { get; set; }
    public string PosterUrl { get; set; }
    public DateTime CreatedAt { get; set; }
}