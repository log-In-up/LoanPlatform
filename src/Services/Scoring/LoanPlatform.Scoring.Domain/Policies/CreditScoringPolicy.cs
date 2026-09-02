using LoanPlatform.Scoring.Domain.Enums;

namespace LoanPlatform.Scoring.Domain.Policies
{
    public static class CreditScoringPolicy
    {
        private const int MinimumScore = 0;
        private const int MaximumScore = 1000;
        private const int PreApprovalThreshold = 600;

        public static CreditDecision Decide(int score)
        {
            if (score is < MinimumScore or > MaximumScore)
            {
                throw new ArgumentOutOfRangeException(nameof(score), $"Score must be between {MinimumScore} and {MaximumScore}.");
            }

            return score > PreApprovalThreshold ? CreditDecision.PreApproved : CreditDecision.Rejected;
        }
    }
}