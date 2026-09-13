using LoanPlatform.LoanCore.Domain.Loans;

namespace LoanPlatform.LoanCore.Application.Loans
{
    public interface ILoanRepository
    {
        Task AddAsync(Loan loan, CancellationToken cancellationToken = default);

        Task<Loan?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task UpdateAsync(Loan loan, CancellationToken cancellationToken = default);
    }
}