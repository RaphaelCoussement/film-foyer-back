using AppliFilms.Api.Entities;

namespace AppliFilms.Api.Repositories.Interfaces
{
    public interface IApprovalRepository
    {
        Task<Approval?> GetByRequestAndUserAsync(Guid requestId, Guid userId);
        Task<int> CountByRequestAsync(Guid requestId);
        Task AddAsync(Approval? approval);
        Task SaveChangesAsync();
        
        Task<List<Approval>> GetByRequestAsync(Guid requestId);
        Task DeleteAsync(Guid id);
    }
}