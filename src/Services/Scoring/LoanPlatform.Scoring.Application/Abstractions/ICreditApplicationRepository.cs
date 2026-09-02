using LoanPlatform.Scoring.Domain.Entities;

namespace LoanPlatform.Scoring.Application.Abstractions
{
    public interface ICreditApplicationRepository
    {
        Task<CreditApplication?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

        Task AddAsync(CreditApplication application, CancellationToken cancellationToken);

        Task SaveAsync(CancellationToken cancellationToken);
    }
}