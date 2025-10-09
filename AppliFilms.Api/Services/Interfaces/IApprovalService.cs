using AppliFilms.Api.DTOs.Approvals;

namespace AppliFilms.Api.Services.Interfaces
{
    public interface IApprovalService
    {
        Task<ApprovalDto> ApproveRequestAsync(Guid requestId, Guid userId);
        Task UnapproveRequestAsync(Guid requestId, Guid userId);

    }
}