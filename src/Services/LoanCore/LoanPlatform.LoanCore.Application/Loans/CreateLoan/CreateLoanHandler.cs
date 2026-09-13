using LoanPlatform.LoanCore.Domain.Loans;

namespace LoanPlatform.LoanCore.Application.Loans.CreateLoan
{
    public sealed class CreateLoanHandler
    {
        private readonly ILoanRepository _loanRepository;

        public CreateLoanHandler(ILoanRepository loanRepository)
        {
            _loanRepository = loanRepository;
        }

        public async Task<Guid> HandleAsync(CreateLoanCommand command, CancellationToken cancellationToken = default)
        {
            Loan loan = new Loan(
                command.ApplicantIdentifier,
                command.PrincipalAmount,
                command.InterestRate,
                command.TermMonths,
                command.PaymentType);

            await _loanRepository.AddAsync(loan, cancellationToken);

            return loan.Id;
        }
    }
}