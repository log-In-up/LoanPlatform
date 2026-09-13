namespace LoanPlatform.LoanCore.Domain.Payments
{
    public class Payment
    {
        public Guid Id { get; private set; }

        public Guid LoanId { get; private set; }

        public decimal Amount { get; private set; }

        public decimal PrincipalAmount { get; private set; }

        public decimal InterestAmount { get; private set; }

        public DateTime PaidAt { get; private set; }

        private Payment() { }

        public Payment(
            Guid loanId,
            decimal amount,
            decimal principalAmount,
            decimal interestAmount)
        {
            if (loanId == Guid.Empty)
            {
                throw new ArgumentException("Loan identifier is required.", nameof(loanId));
            }

            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(amount),
                    "Payment amount must be greater than zero.");
            }

            if (principalAmount < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(principalAmount),
                    "Principal payment cannot be negative.");
            }

            if (interestAmount < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(interestAmount),
                    "Interest payment cannot be negative.");
            }

            if (principalAmount + interestAmount != amount)
            {
                throw new ArgumentException(
                    "Principal and interest amounts must equal the total payment amount.");
            }

            Id = Guid.NewGuid();
            LoanId = loanId;
            Amount = amount;
            PrincipalAmount = principalAmount;
            InterestAmount = interestAmount;
            PaidAt = DateTime.UtcNow;
        }
    }
}