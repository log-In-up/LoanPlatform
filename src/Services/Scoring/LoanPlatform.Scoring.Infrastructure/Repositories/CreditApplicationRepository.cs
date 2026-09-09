using LoanPlatform.Scoring.Application.Abstractions;
using LoanPlatform.Scoring.Domain.Entities;
using LoanPlatform.Scoring.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LoanPlatform.Scoring.Infrastructure.Repositories
{
    public sealed class CreditApplicationRepository : ICreditApplicationRepository
    {
        private readonly ScoringDbContext _dbContext;

        public CreditApplicationRepository(ScoringDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<CreditApplication?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _dbContext.CreditApplications
                .FirstOrDefaultAsync(
                    application => application.Id == id,
                    cancellationToken);
        }

        public async Task AddAsync(CreditApplication application, CancellationToken cancellationToken)
        {
            await _dbContext.CreditApplications.AddAsync(
                application,
                cancellationToken);
        }

        public async Task SaveAsync(CancellationToken cancellationToken)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}