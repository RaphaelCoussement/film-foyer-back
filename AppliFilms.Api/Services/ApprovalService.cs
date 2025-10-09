using AppliFilms.Api.Entities;
using AppliFilms.Api.Repositories.Interfaces;
using AppliFilms.Api.Services.Interfaces;
using System;
using System.Threading.Tasks;
using AppliFilms.Api.DTOs.Approvals;

namespace AppliFilms.Api.Services
{
    public class ApprovalService : IApprovalService
    {
        private readonly IApprovalRepository _approvalRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRequestRepository _requestRepository;

        public ApprovalService(IApprovalRepository approvalRepository, IUserRepository userRepository, IRequestRepository requestRepository)
        {
            _approvalRepository = approvalRepository;
            _userRepository = userRepository;
            _requestRepository = requestRepository;
        }

        public async Task<ApprovalDto> ApproveRequestAsync(Guid requestId, Guid userId)
        {
            var existingApproval = await _approvalRepository.GetByRequestAndUserAsync(requestId, userId);
            if (existingApproval != null)
                throw new Exception("Vous avez déjà approuvé cette demande");

            var request = await _requestRepository.GetByIdAsync(requestId);
            if (request == null)
                throw new Exception("Demande introuvable");

            var approval = new Approval
            {
                Id = Guid.NewGuid(),
                RequestId = requestId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            await _approvalRepository.AddAsync(approval);

            // === Mettre à jour le compteur et la liste des ApprovalIds ===
            var approvalIds = request.ApprovalIds.ToList();
            approvalIds.Add(approval.Id);
            request.ApprovalIds = approvalIds.ToArray();
            request.ApprovalCount = request.ApprovalIds.Length; // nombre de votes
            await _requestRepository.UpdateAsync(request);

            var user = await _userRepository.GetByIdAsync(userId);

            return new ApprovalDto()
            {
                Id = approval.Id,
                RequestId = requestId,
                UserDisplayName = user?.DisplayName ?? "Inconnu",
                CreatedAt = approval.CreatedAt,
                ApprovalCount = request.ApprovalCount
            };
        }
        
        public async Task UnapproveRequestAsync(Guid requestId, Guid userId)
        {
            var existingApproval = await _approvalRepository.GetByRequestAndUserAsync(requestId, userId);
            if (existingApproval == null)
                throw new Exception("Vous n'avez pas encore voté pour cette demande");

            // Supprimer l'approbation
            await _approvalRepository.DeleteAsync(existingApproval.Id);

            // Mettre à jour la demande
            var request = await _requestRepository.GetByIdAsync(requestId);
            if (request == null)
                throw new Exception("Demande introuvable");

            var approvalIds = request.ApprovalIds.ToList();
            approvalIds.Remove(existingApproval.Id);
            request.ApprovalIds = approvalIds.ToArray();
            request.ApprovalCount = request.ApprovalIds.Length;
            await _requestRepository.UpdateAsync(request);
        }
        
        public async Task<List<Guid>> GetUserApprovalsAsync(Guid userId)
        {
            var approvals = await _approvalRepository.GetByUserAsync(userId);
            return approvals.Select(a => a.RequestId).ToList();
        }

    }
}
