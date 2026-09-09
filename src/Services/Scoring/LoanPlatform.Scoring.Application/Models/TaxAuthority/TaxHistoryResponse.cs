namespace LoanPlatform.Scoring.Application.Models.TaxAuthority
{
    public sealed record TaxHistoryResponse(
        string ApplicantIdentifier,
        IReadOnlyCollection<TaxPaymentResponse> Payments);

    public sealed record TaxPaymentResponse(
        int Year,
        decimal Amount);
}