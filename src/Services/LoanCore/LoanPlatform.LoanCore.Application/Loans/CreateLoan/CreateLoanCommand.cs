using LoanPlatform.LoanCore.Domain.Loans;

namespace LoanPlatform.LoanCore.Application.Loans.CreateLoan
{
    public sealed record CreateLoanCommand(
        string ApplicantIdentifier,
        decimal PrincipalAmount,
        decimal InterestRate,
        int TermMonths,
        PaymentType PaymentType);
}