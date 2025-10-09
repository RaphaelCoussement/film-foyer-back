using AppliFilms.Api.Data.Mongo;
using AppliFilms.Api.Entities;
using AppliFilms.Api.Repositories.Interfaces;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppliFilms.Api.Repositories
{
    public class ApprovalRepository : IApprovalRepository
    {
        private readonly IMongoCollection<Approval> _approvals;

        public ApprovalRepository(MongoDbService mongoService)
        {
            _approvals = mongoService.GetCollection<Approval>("Approvals");
        }

        public async Task<Approval?> GetByRequestAndUserAsync(Guid requestId, Guid userId) =>
            await _approvals.Find(a => a.RequestId == requestId && a.UserId == userId)
                .FirstOrDefaultAsync();

        public async Task<int> CountByRequestAsync(Guid requestId) =>
            (int)await _approvals.CountDocumentsAsync(a => a.RequestId == requestId);

        public async Task AddAsync(Approval? approval)
        {
            if (approval == null) return;
            await _approvals.InsertOneAsync(approval);
        }

        public Task SaveChangesAsync() => Task.CompletedTask;

        public async Task<List<Approval>> GetByRequestAsync(Guid requestId) =>
            await _approvals.Find(a => a.RequestId == requestId).ToListAsync();
        
        public async Task DeleteAsync(Guid id)
        {
            await _approvals.DeleteOneAsync(a => a.Id == id);
        }
    }
}