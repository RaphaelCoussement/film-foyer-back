namespace AppliFilms.Api.DTOs.Approvals
{
    public class ApprovalDto
    {
        public Guid Id { get; set; }
        public Guid RequestId { get; set; }
        public string UserDisplayName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}