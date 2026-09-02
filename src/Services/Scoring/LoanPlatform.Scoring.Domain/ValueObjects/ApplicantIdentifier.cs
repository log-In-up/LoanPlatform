namespace LoanPlatform.Scoring.Domain.ValueObjects
{
    
    public sealed record ApplicantIdentifier
    {
        public const int IDENTIFICATOR_LENGTH = 12;
        
        public string Value { get; }

        public ApplicantIdentifier(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Applicant identifier cannot be empty.", nameof(value));
            }

            if (value.Length != IDENTIFICATOR_LENGTH)
            {
                throw new ArgumentException("Applicant identifier must contain exactly 12 digits.", nameof(value));
            }

            if (!value.All(char.IsDigit))
            {
                throw new ArgumentException("Applicant identifier must contain only digits.", nameof(value));
            }

            Value = value;
        }

        public override string ToString() => Value;
    }
}