namespace AppliFilms.Api.DTOs.Wishlist;

public class AddToWishlistDto
{
    public Guid? MovieId { get; set; }
    public int? TmdbId { get; set; }
}