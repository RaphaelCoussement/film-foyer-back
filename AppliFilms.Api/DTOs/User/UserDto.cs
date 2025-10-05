namespace AppliFilms.Api.DTOs.User;

public class UserDto
{
    public Guid Id { get; set; }
    public string DisplayName { get; set; }
    public string Email { get; set; }
    public bool IsAdmin { get; set; }
}