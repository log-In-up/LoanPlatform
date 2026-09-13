using LoanPlatform.LoanCore.Domain.Payments;

namespace LoanPlatform.LoanCore.Application.Payments.GetPayment
{
    public sealed class GetPaymentHandler
    {
        private readonly IPaymentRepository _paymentRepository;

        public GetPaymentHandler(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public Task<Payment?> HandleAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return _paymentRepository.GetByIdAsync(
                id,
                cancellationToken);
        }
    }
}