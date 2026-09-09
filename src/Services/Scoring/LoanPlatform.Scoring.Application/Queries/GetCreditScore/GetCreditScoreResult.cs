using LoanPlatform.Scoring.Domain.Enums;

namespace LoanPlatform.Scoring.Application.Queries.GetCreditScore
{
    public sealed record GetCreditScoreResult(
        Guid ApplicationId,
        int Score,
        CreditDecision Decision,
        DateTime CalculatedAt);
}