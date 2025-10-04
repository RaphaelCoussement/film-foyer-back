using AppliFilms.Api.DTOs;
using AppliFilms.Api.DTOs.Requests;

namespace AppliFilms.Api.Services.Interfaces
{
    public interface IRequestService
    {
        Task<RequestDto> CreateRequestAsync(CreateRequestDto dto, Guid userId);
        Task<List<RequestDto>> GetAllRequestsAsync();
        Task<bool> DeleteRequestAsync(Guid requestId, Guid userId);

        Task DeleteAllRequestsAsync();
    }
}