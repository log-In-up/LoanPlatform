using LoanPlatform.Contracts.Events;
using LoanPlatform.LoanCore.Application.Common;
using LoanPlatform.LoanCore.Domain.Loans;

namespace LoanPlatform.LoanCore.Application.Loans.ApproveLoan
{
    public sealed class ApproveLoanHandler
    {
        private readonly ILoanRepository _loanRepository;
        private readonly ILoanApprovedPublisher _publisher;

        public ApproveLoanHandler(
            ILoanRepository loanRepository,
            ILoanApprovedPublisher publisher)
        {
            _loanRepository = loanRepository;
            _publisher = publisher;
        }

        public async Task<bool> HandleAsync(ApproveLoanCommand command, CancellationToken cancellationToken = default)
        {
            Loan? loan = await _loanRepository.GetByIdAsync(command.LoanId, cancellationToken);

            if (loan is null)
            {
                return false;
            }

            loan.Approve();

            await _loanRepository.UpdateAsync(loan, cancellationToken);

            LoanApprovedEvent @event = new(
                Guid.NewGuid(),
                loan.Id,
                loan.ApplicantIdentifier,
                loan.PrincipalAmount,
                DateTime.UtcNow);

            await _publisher.PublishAsync(@event, cancellationToken);

            return true;
        }
    }
}