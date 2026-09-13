namespace LoanPlatform.LoanCore.Application.Loans.Schedule
{
    public sealed record PaymentScheduleItem(
        int PaymentNumber,
        DateTime PaymentDate,
        decimal PaymentAmount,
        decimal PrincipalAmount,
        decimal InterestAmount,
        decimal RemainingPrincipal);
}