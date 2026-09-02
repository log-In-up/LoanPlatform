using LoanPlatform.Scoring.Domain.Enums;

namespace LoanPlatform.Scoring.Application.Commands.RunCreditScoring
{
    public sealed record RunCreditScoringCommand(Guid ApplicationId);
}