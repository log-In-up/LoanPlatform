using LoanPlatform.LoanCore.Application.Loans;
using LoanPlatform.LoanCore.Domain.Loans;
using Microsoft.EntityFrameworkCore;

namespace LoanPlatform.LoanCore.Infrastructure.Persistence.Repositories
{
    public sealed class LoanRepository : ILoanRepository
    {
        private readonly LoanDbContext _dbContext;

        public LoanRepository(LoanDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Loan loan, CancellationToken cancellationToken = default)
        {
            await _dbContext.Loans.AddAsync(loan, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<Loan?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Loans.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task UpdateAsync(Loan loan, CancellationToken cancellationToken = default)
        {
            _dbContext.Loans.Update(loan);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}