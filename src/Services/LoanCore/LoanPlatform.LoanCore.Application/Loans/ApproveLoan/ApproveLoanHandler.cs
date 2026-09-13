using LoanPlatform.LoanCore.Domain.Loans;

namespace LoanPlatform.LoanCore.Application.Loans.ApproveLoan
{
    public sealed class ApproveLoanHandler
    {
        private readonly ILoanRepository _loanRepository;

        public ApproveLoanHandler(ILoanRepository loanRepository)
        {
            _loanRepository = loanRepository;
        }

        public async Task<bool> HandleAsync(
            ApproveLoanCommand command,
            CancellationToken cancellationToken = default)
        {
            Loan? loan = await _loanRepository.GetByIdAsync(
                command.LoanId,
                cancellationToken);

            if (loan is null)
            {
                return false;
            }

            loan.Approve();

            await _loanRepository.UpdateAsync(loan, cancellationToken);

            return true;
        }
    }
}