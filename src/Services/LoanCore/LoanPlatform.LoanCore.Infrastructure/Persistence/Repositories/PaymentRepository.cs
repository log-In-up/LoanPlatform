using LoanPlatform.LoanCore.Application.Payments;
using LoanPlatform.LoanCore.Domain.Payments;
using Microsoft.EntityFrameworkCore;

namespace LoanPlatform.LoanCore.Infrastructure.Persistence.Repositories
{
    public sealed class PaymentRepository : IPaymentRepository
    {
        private readonly LoanDbContext _dbContext;

        public PaymentRepository(LoanDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(Payment payment)
        {
            _dbContext.Payments.Add(payment);
        }

        public async Task<IReadOnlyList<Payment>> GetByLoanIdAsync(Guid loanId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Payments
                .AsNoTracking()
                .Where(x => x.LoanId == loanId)
                .OrderBy(x => x.PaidAt)
                .ToListAsync(cancellationToken);
        }
        
        public async Task<Payment?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.Payments
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }
    }
}