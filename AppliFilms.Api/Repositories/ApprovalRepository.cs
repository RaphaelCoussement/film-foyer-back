using AppliFilms.Api.Data;
using AppliFilms.Api.Entities;
using AppliFilms.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AppliFilms.Api.Repositories
{
    public class ApprovalRepository(AppDbContext context) : IApprovalRepository
    {
        public async Task<Approval?> GetByRequestAndUserAsync(Guid requestId, Guid userId) =>
            await context.Approvals.FirstOrDefaultAsync(a => a.RequestId == requestId && a.UserId == userId);

        public async Task<int> CountByRequestAsync(Guid requestId) =>
            await context.Approvals.CountAsync(a => a.RequestId == requestId);

        public async Task AddAsync(Approval? approval) =>
            await context.Approvals.AddAsync(approval);

        public async Task SaveChangesAsync() =>
            await context.SaveChangesAsync();
        
        public async Task<List<Approval>> GetByRequestAsync(Guid requestId) =>
            await context.Approvals
                .Include(a => a.User) // pour avoir le DisplayName
                .Where(a => a.RequestId == requestId)
                .ToListAsync();
    }
}