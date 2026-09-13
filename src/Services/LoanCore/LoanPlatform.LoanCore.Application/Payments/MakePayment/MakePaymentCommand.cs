namespace LoanPlatform.LoanCore.Application.Payments.MakePayment
{
    public sealed record MakePaymentCommand(
        Guid LoanId,
        decimal Amount,
        decimal PrincipalAmount,
        decimal InterestAmount);
}