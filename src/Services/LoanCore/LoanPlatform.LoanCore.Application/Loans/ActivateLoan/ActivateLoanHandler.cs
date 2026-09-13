using LoanPlatform.LoanCore.Domain.Loans;

namespace LoanPlatform.LoanCore.Application.Loans.ActivateLoan
{
    public sealed class ActivateLoanHandler
    {
        private readonly ILoanRepository _loanRepository;

        public ActivateLoanHandler(ILoanRepository loanRepository)
        {
            _loanRepository = loanRepository;
        }

        public async Task<bool> HandleAsync(
            ActivateLoanCommand command,
            CancellationToken cancellationToken = default)
        {
            Loan? loan = await _loanRepository.GetByIdAsync(command.LoanId, cancellationToken);

            if (loan is null)
            {
                return false;
            }

            loan.Activate();

            await _loanRepository.UpdateAsync(loan, cancellationToken);

            return true;
        }
    }
}