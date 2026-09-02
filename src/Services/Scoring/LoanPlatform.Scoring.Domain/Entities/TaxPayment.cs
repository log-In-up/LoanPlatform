namespace LoanPlatform.Scoring.Domain.Entities
{
    public sealed class TaxPayment
    {
        public int Year { get; }

        public decimal Amount { get; }

        public TaxPayment(int year, decimal amount)
        {
            if (year < 2000 || year > DateTime.UtcNow.Year)
            {
                throw new ArgumentOutOfRangeException(nameof(year), "Tax payment year is invalid.");
            }

            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Tax payment amount cannot be negative.");
            }

            Year = year;
            Amount = amount;
        }
    }
}