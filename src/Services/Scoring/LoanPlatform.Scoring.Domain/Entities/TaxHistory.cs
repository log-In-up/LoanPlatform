using LoanPlatform.Scoring.Domain.ValueObjects;

namespace LoanPlatform.Scoring.Domain.Entities
{
    public sealed class TaxHistory
    {
        private readonly List<TaxPayment> _payments = [];

        public ApplicantIdentifier ApplicantIdentifier { get; }

        public IReadOnlyCollection<TaxPayment> Payments => _payments.AsReadOnly();

        public TaxHistory(ApplicantIdentifier applicantIdentifier, IEnumerable<TaxPayment> payments)
        {
            ApplicantIdentifier = applicantIdentifier
                                  ?? throw new ArgumentNullException(nameof(applicantIdentifier));

            ArgumentNullException.ThrowIfNull(payments);

            _payments.AddRange(payments);

            if (_payments.Count == 0)
            {
                throw new ArgumentException("Tax history must contain at least one payment.", nameof(payments));
            }
        }
    }
}
