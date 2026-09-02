using LoanPlatform.Scoring.Domain.Enums;

namespace LoanPlatform.Scoring.Domain.Entities
{
    public class CreditScore
    {
        public Guid Id { get; private set; }

        public Guid ApplicationId { get; private set; }

        public int Score { get; private set; }

        public CreditDecision Decision { get; private set; }

        public DateTime CalculatedAt { get; private set; }

        private CreditScore()
        {
        }
        
        public CreditScore(
            Guid applicationId,
            int score,
            CreditDecision decision)
        {
            if (applicationId == Guid.Empty)
            {
                throw new ArgumentException("Application ID cannot be empty.", nameof(applicationId));
            }

            if (score is < 0 or > 1000)
            {
                throw new ArgumentOutOfRangeException(nameof(score), "Score must be between 0 and 1000.");
            }

            Id = Guid.NewGuid();
            ApplicationId = applicationId;
            Score = score;
            Decision = decision;
            CalculatedAt = DateTime.UtcNow;
        }
    }
}