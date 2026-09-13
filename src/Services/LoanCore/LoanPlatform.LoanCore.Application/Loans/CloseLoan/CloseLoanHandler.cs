using LoanPlatform.LoanCore.Domain.Loans;

namespace LoanPlatform.LoanCore.Application.Loans.CloseLoan
{
    public sealed class CloseLoanHandler
    {
        private readonly ILoanRepository _loanRepository;

        public CloseLoanHandler(ILoanRepository loanRepository)
        {
            _loanRepository = loanRepository;
        }

        public async Task<bool> HandleAsync(CloseLoanCommand command, CancellationToken cancellationToken = default)
        {
            Loan? loan = await _loanRepository.GetByIdAsync(command.LoanId, cancellationToken);

            if (loan is null)
            {
                return false;
            }

            loan.Close();

            await _loanRepository.UpdateAsync(loan, cancellationToken);

            return true;
        }
    }
}