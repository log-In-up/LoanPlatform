using LoanPlatform.LoanCore.Domain.Loans;

namespace LoanPlatform.LoanCore.Application.Loans.GetLoan
{
    public class GetLoanHandler
    {
        private readonly ILoanRepository _loanRepository;

        public GetLoanHandler(ILoanRepository loanRepository)
        {
            _loanRepository = loanRepository;
        }

        public async Task<Loan?> HandleAsync(GetLoanQuery query, CancellationToken cancellationToken = default)
        {
            return await _loanRepository.GetByIdAsync(query.LoanId, cancellationToken);
        }
    }
}