using LoanPlatform.Scoring.Domain.Enums;
using LoanPlatform.Scoring.Domain.ValueObjects;

namespace LoanPlatform.Scoring.Domain.Entities
{
    public sealed class CreditApplication
    {
        public Guid Id { get; private set; }

        public ApplicantIdentifier ApplicantIdentifier { get; private set; }

        public ApplicantType ApplicantType { get; private set; }

        public decimal RequestedAmount { get; private set; }

        public int RequestedTermMonths { get; private set; }

        public CreditApplicationStatus Status { get; private set; }

        public CreditDecision Decision { get; private set; }

        public DateTime CreatedAt { get; private set; }

        private CreditApplication()
        {
            ApplicantIdentifier = null!;
        }

        public CreditApplication(
            ApplicantIdentifier applicantIdentifier,
            ApplicantType applicantType,
            decimal requestedAmount,
            int requestedTermMonths)
        {
            if (requestedAmount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(requestedAmount), "Requested amount must be greater than zero.");
            }

            if (requestedTermMonths <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(requestedTermMonths), "Requested term must be greater than zero.");
            }

            Id = Guid.NewGuid();

            ApplicantIdentifier = applicantIdentifier;
            ApplicantType = applicantType;

            RequestedAmount = requestedAmount;
            RequestedTermMonths = requestedTermMonths;

            Status = CreditApplicationStatus.Created;
            Decision = CreditDecision.Pending;

            CreatedAt = DateTime.UtcNow;
        }
        
        public void StartScoring()
        {
            if (Status != CreditApplicationStatus.Created)
            {
                throw new InvalidOperationException("Scoring can only be started for a newly created application.");
            }

            Status = CreditApplicationStatus.ScoringInProgress;
        }

        public void CompleteScoring(CreditDecision decision)
        {
            if (Status != CreditApplicationStatus.ScoringInProgress)
            {
                throw new InvalidOperationException("Scoring can only be completed when scoring is in progress.");
            }

            Status = CreditApplicationStatus.Scored;
            Decision = decision;
        }

        public void MarkScoringFailed()
        {
            if (Status != CreditApplicationStatus.ScoringInProgress)
            {
                throw new InvalidOperationException("Only an application in scoring can be marked as failed.");
            }

            Status = CreditApplicationStatus.Failed;
            Decision = CreditDecision.Pending;
        }
    }
}