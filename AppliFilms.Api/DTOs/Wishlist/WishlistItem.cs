namespace AppliFilms.Api.DTOs.Wishlist;

public class WishlistItemDto
{
    public Guid Id { get; set; }
    public Guid MovieId { get; set; }
    public string Title { get; set; }
    public int? Duration { get; set; }
    public string? Year { get; set; }
    public string PosterUrl { get; set; }
    public DateTime AddedAt { get; set; }
}