namespace LoanPlatform.LoanCore.Domain.Loans
{
    public class Loan
    {
        public Guid Id { get; private set; }

        public string ApplicantIdentifier { get; private set; } = null!;

        public decimal PrincipalAmount { get; private set; }

        public decimal PaidPrincipalAmount { get; private set; }

        public decimal OutstandingPrincipalAmount =>
            PrincipalAmount - PaidPrincipalAmount;

        public decimal InterestRate { get; private set; }
        
        public int TermMonths { get; private set; }
        
        public LoanStatus Status { get; private set; }
        
        public PaymentType PaymentType { get; private set; }
        
        public DateTime CreatedAt { get; private set; }

        private Loan() { }
        
        public Loan(
            string applicantIdentifier,
            decimal principalAmount,
            decimal interestRate,
            int termMonths,
            PaymentType paymentType)
        {
            if (string.IsNullOrWhiteSpace(applicantIdentifier))
            {
                throw new ArgumentException("Applicant identifier is required.", nameof(applicantIdentifier));
            }

            if (principalAmount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(principalAmount), "Principal amount must be greater than zero.");
            }

            if (interestRate < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(interestRate), "Interest rate cannot be negative.");
            }

            if (termMonths <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(termMonths), "Term must be greater than zero.");
            }
    
            Id = Guid.NewGuid();
            ApplicantIdentifier = applicantIdentifier;
            PrincipalAmount = principalAmount;
            PaidPrincipalAmount = 0;
            InterestRate = interestRate;
            TermMonths = termMonths;
            PaymentType = paymentType;
            Status = LoanStatus.Pending;
            CreatedAt = DateTime.UtcNow;
        }

        public void Approve()
        {
            if (Status != LoanStatus.Pending)
            {
                throw new InvalidOperationException("Only pending loans can be approved.");
            }

            Status = LoanStatus.Approved;
        }

        public void Activate()
        {
            if (Status != LoanStatus.Approved)
            {
                throw new InvalidOperationException("Only approved loans can be activated.");
            }

            Status = LoanStatus.Active;
        }

        public void Close()
        {
            if (Status != LoanStatus.Active)
            {
                throw new InvalidOperationException("Only active loans can be closed.");
            }

            Status = LoanStatus.Closed;
        }

        public void Reject()
        {
            if (Status != LoanStatus.Pending)
            {
                throw new InvalidOperationException("Only pending loans can be rejected.");
            }

            Status = LoanStatus.Rejected;
        }
        
        public void RegisterPrincipalPayment(decimal principalAmount)
        {
            if (principalAmount <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(principalAmount),
                    "Principal payment must be greater than zero.");
            }

            if (Status != LoanStatus.Active)
            {
                throw new InvalidOperationException(
                    "Principal payments can only be made for active loans.");
            }

            if (principalAmount > OutstandingPrincipalAmount)
            {
                throw new InvalidOperationException(
                    "Principal payment cannot exceed the outstanding principal amount.");
            }

            PaidPrincipalAmount += principalAmount;

            if (PaidPrincipalAmount == PrincipalAmount)
            {
                Status = LoanStatus.Closed;
            }
        }
    }
}