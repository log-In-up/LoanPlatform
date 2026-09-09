using LoanPlatform.Scoring.Domain.Entities;

namespace LoanPlatform.Scoring.Application.Abstractions
{
    public interface ICreditScoreRepository
    {
        Task AddAsync(CreditScore creditScore, CancellationToken cancellationToken);

        Task<CreditScore?> GetByApplicationIdAsync(Guid applicationId, CancellationToken cancellationToken);

        Task SaveAsync(CancellationToken cancellationToken);
    }
}