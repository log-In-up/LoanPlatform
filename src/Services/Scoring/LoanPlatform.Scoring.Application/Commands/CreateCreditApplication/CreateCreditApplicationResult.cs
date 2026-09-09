using LoanPlatform.Scoring.Domain.Enums;

namespace LoanPlatform.Scoring.Application.Commands.CreateCreditApplication
{
    public sealed record CreateCreditApplicationResult(
        Guid ApplicationId,
        string ApplicantIdentifier,
        ApplicantType ApplicantType,
        decimal RequestedAmount,
        int RequestedTermMonths,
        CreditApplicationStatus Status,
        CreditDecision Decision);
}