using LoanPlatform.Scoring.Domain.Enums;

namespace LoanPlatform.Scoring.Application.Queries.GetCreditApplication
{
    public sealed record GetCreditApplicationResult(
        Guid ApplicationId,
        string ApplicantIdentifier,
        ApplicantType ApplicantType,
        decimal RequestedAmount,
        int RequestedTermMonths,
        CreditApplicationStatus Status,
        CreditDecision Decision,
        DateTime CreatedAt);
}