using LoanPlatform.LoanCore.Application.Loans;
using LoanPlatform.LoanCore.Domain.Loans;

namespace LoanPlatform.LoanCore.Application.Loans.RejectLoan
{
    public sealed class RejectLoanHandler
    {
        private readonly ILoanRepository _loanRepository;

        public RejectLoanHandler(ILoanRepository loanRepository)
        {
            _loanRepository = loanRepository;
        }

        public async Task<bool> HandleAsync(RejectLoanCommand command, CancellationToken cancellationToken = default)
        {
            Loan? loan = await _loanRepository.GetByIdAsync(command.LoanId, cancellationToken);

            if (loan is null)
            {
                return false;
            }

            loan.Reject();

            await _loanRepository.UpdateAsync(loan, cancellationToken);

            return true;
        }
    }
}