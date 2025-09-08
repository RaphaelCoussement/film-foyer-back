namespace AppliFilms.Api.DTOs.Auth
{
    public class AuthResponseDto
    {
        public string Token { get; set; }
        public string DisplayName { get; set; }
        public bool IsAdmin { get; set; }
    }
}