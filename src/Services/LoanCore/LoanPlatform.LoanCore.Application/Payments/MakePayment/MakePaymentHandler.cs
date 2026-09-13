using LoanPlatform.LoanCore.Application.Common;
using LoanPlatform.LoanCore.Application.Loans;
using LoanPlatform.LoanCore.Domain.Loans;
using LoanPlatform.LoanCore.Domain.Payments;

namespace LoanPlatform.LoanCore.Application.Payments.MakePayment
{
    public sealed class MakePaymentHandler
    {
        private readonly ILoanRepository _loanRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public MakePaymentHandler(
            ILoanRepository loanRepository,
            IPaymentRepository paymentRepository,
            IUnitOfWork unitOfWork)
        {
            _loanRepository = loanRepository;
            _paymentRepository = paymentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid?> HandleAsync(
            MakePaymentCommand command,
            CancellationToken cancellationToken = default)
        {
            Guid? paymentId = null;

            await _unitOfWork.ExecuteInTransactionAsync(
                async ct =>
                {
                    Loan? loan = await _loanRepository.GetByIdAsync(
                        command.LoanId,
                        ct);

                    if (loan is null)
                    {
                        return;
                    }

                    if (loan.Status != Domain.Loans.LoanStatus.Active)
                    {
                        throw new InvalidOperationException(
                            "Payments can only be made for active loans.");
                    }

                    loan.RegisterPrincipalPayment(command.PrincipalAmount);

                    Payment payment = new Payment(
                        command.LoanId,
                        command.Amount,
                        command.PrincipalAmount,
                        command.InterestAmount);

                    _paymentRepository.Add(payment);

                    paymentId = payment.Id;
                },
                cancellationToken);

            return paymentId;
        }
    }
}