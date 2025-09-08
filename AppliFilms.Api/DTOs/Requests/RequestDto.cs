using AppliFilms.Api.DTOs.Movie;

namespace AppliFilms.Api.DTOs.Requests
{
    public class RequestDto
    {
        public Guid Id { get; set; }
        public MovieDto Movie { get; set; }
        public string RequestedBy { get; set; }   // nom affiché
        public DateTime EventDate { get; set; }
        public int ApprovalCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}