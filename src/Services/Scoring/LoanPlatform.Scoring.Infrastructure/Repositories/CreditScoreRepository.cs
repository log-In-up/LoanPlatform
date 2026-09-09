using LoanPlatform.Scoring.Application.Abstractions;
using LoanPlatform.Scoring.Domain.Entities;
using LoanPlatform.Scoring.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LoanPlatform.Scoring.Infrastructure.Repositories
{
    public sealed class CreditScoreRepository : ICreditScoreRepository
    {
        private readonly ScoringDbContext _dbContext;

        public CreditScoreRepository(ScoringDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public async Task AddAsync(CreditScore creditScore, CancellationToken cancellationToken)
        {
            await _dbContext.CreditScores.AddAsync(creditScore, cancellationToken);
        }

        public async Task<CreditScore?> GetByApplicationIdAsync(Guid applicationId, CancellationToken cancellationToken)
        {
            return await _dbContext.CreditScores
                .FirstOrDefaultAsync(
                    score => score.ApplicationId == applicationId,
                    cancellationToken);
        }

        public async Task SaveAsync(CancellationToken cancellationToken)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}