using LoanPlatform.Scoring.Domain.Enums;

namespace LoanPlatform.Scoring.Application.Commands.CreateCreditApplication
{
    public sealed record CreateCreditApplicationCommand(
        string ApplicantIdentifier,
        ApplicantType ApplicantType,
        decimal RequestedAmount,
        int RequestedTermMonths);
}