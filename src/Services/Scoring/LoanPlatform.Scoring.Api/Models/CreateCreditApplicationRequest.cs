using LoanPlatform.Scoring.Domain.Enums;

namespace LoanPlatform.Scoring.Api.Models
{
    public sealed record CreateCreditApplicationRequest(
        string ApplicantIdentifier,
        ApplicantType ApplicantType,
        decimal RequestedAmount,
        int RequestedTermMonths);
}