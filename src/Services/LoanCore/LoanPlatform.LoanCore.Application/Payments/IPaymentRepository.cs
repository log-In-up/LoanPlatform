using LoanPlatform.LoanCore.Domain.Payments;

namespace LoanPlatform.LoanCore.Application.Payments
{
    public interface IPaymentRepository
    {
        void Add(Payment payment);

        Task<Payment?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Payment>> GetByLoanIdAsync(
            Guid loanId,
            CancellationToken cancellationToken = default);
    }
}