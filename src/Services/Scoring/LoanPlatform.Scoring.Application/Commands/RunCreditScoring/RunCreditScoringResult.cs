using LoanPlatform.Scoring.Domain.Enums;

namespace LoanPlatform.Scoring.Application.Commands.RunCreditScoring
{
    public sealed record RunCreditScoringResult(Guid ApplicationId, int Score, CreditDecision Decision);
}