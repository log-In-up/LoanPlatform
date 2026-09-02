using LoanPlatform.Scoring.Domain.Enums;

namespace LoanPlatform.Scoring.Domain.Scoring
{
    public sealed record CreditScoringResult(int Score, CreditDecision Decision);
}