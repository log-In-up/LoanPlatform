using LoanPlatform.LoanCore.Domain.Payments;

namespace LoanPlatform.LoanCore.Application.Payments.GetPaymentsByLoan
{
    public sealed class GetPaymentsByLoanHandler
    {
        private readonly IPaymentRepository _paymentRepository;

        public GetPaymentsByLoanHandler(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public Task<IReadOnlyList<Payment>> HandleAsync(Guid loanId, CancellationToken cancellationToken = default)
        {
            return _paymentRepository.GetByLoanIdAsync(loanId, cancellationToken);
        }
    }
}