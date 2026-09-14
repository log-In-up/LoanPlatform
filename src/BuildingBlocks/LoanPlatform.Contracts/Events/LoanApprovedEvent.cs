namespace LoanPlatform.Contracts.Events
{
    public sealed record LoanApprovedEvent(
        Guid EventId,
        Guid LoanId,
        string ApplicantIdentifier,
        decimal PrincipalAmount,
        DateTime ApprovedAt);
}