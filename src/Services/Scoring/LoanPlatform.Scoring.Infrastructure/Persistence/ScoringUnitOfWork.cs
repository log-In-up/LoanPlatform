using LoanPlatform.Scoring.Application.Abstractions;

namespace LoanPlatform.Scoring.Infrastructure.Persistence
{
    public sealed class ScoringUnitOfWork : IScoringUnitOfWork
    {
        private readonly ScoringDbContext _dbContext;

        public ScoringUnitOfWork(ScoringDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}