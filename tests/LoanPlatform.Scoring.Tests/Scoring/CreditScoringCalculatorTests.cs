using LoanPlatform.Scoring.Domain.Entities;
using LoanPlatform.Scoring.Domain.Enums;
using LoanPlatform.Scoring.Domain.Scoring;
using LoanPlatform.Scoring.Domain.ValueObjects;

namespace LoanPlatform.Scoring.Tests.Scoring
{
    public class CreditScoringCalculatorTests
    {
        private static ApplicantIdentifier CreateIdentifier()
        {
            return new ApplicantIdentifier("123456789012");
        }
        
        [Fact]
        public void Calculate_WithStrongTaxHistory_ReturnsHighScore()
        {
            // Arrange
            ApplicantIdentifier identifier = CreateIdentifier();

            TaxHistory history = new TaxHistory(
                identifier,
                [
                    new TaxPayment(2023, 8_000_000m),
                    new TaxPayment(2024, 9_000_000m),
                    new TaxPayment(2025, 10_000_000m)
                ]);

            CreditScoringCalculator calculator = new CreditScoringCalculator();

            // Act
            CreditScoringResult result = calculator.Calculate(history, 10_000_000m);

            // Assert
            Assert.InRange(result.Score, 600, 1000);
            Assert.Equal(CreditDecision.PreApproved, result.Decision);
        }
        
        [Fact]
        public void Calculate_WithWeakTaxHistory_ReturnsLowScore()
        {
            // Arrange
            ApplicantIdentifier identifier = CreateIdentifier();

            TaxHistory history = new TaxHistory(
                identifier,
                [
                    new TaxPayment(2023, 100_000m),
                    new TaxPayment(2024, 120_000m),
                    new TaxPayment(2025, 90_000m)
                ]);

            CreditScoringCalculator calculator = new CreditScoringCalculator();

            // Act
            CreditScoringResult result = calculator.Calculate(history, 100_000_000m);

            // Assert
            Assert.InRange(result.Score, 0, 600);

            Assert.Equal(CreditDecision.Rejected, result.Decision);
        }

        [Fact]
        public void Calculate_ResultDecision_IsConsistentWithScore()
        {
            // Arrange
            ApplicantIdentifier identifier = CreateIdentifier();

            TaxHistory history = new TaxHistory(
                identifier,
                [
                    new TaxPayment(2023, 5_000_000m),
                    new TaxPayment(2024, 5_000_000m),
                    new TaxPayment(2025, 5_000_000m)
                ]);

            CreditScoringCalculator calculator = new CreditScoringCalculator();

            // Act
            CreditScoringResult result =
                calculator.Calculate(
                    history,
                    5_000_000m);

            // Assert
            if (result.Score > 600)
            {
                Assert.Equal(CreditDecision.PreApproved, result.Decision);
            }
            else
            {
                Assert.Equal(CreditDecision.Rejected, result.Decision);
            }
        }
        
        [Fact]
        public void Calculate_ScoreIsAlwaysWithinValidRange()
        {
            // Arrange
            ApplicantIdentifier identifier = CreateIdentifier();

            TaxHistory history = new TaxHistory(
                identifier,
                [
                    new TaxPayment(2023, 100_000_000m),
                    new TaxPayment(2024, 100_000_000m),
                    new TaxPayment(2025, 100_000_000m)
                ]);

            CreditScoringCalculator calculator = new CreditScoringCalculator();

            // Act
            CreditScoringResult result =
                calculator.Calculate(
                    history,
                    1_000m);

            // Assert
            Assert.InRange(result.Score, 0, 1000);
        }
        
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Calculate_WithInvalidRequestedAmount_Throws(decimal requestedAmount)
        {
            // Arrange
            ApplicantIdentifier identifier = CreateIdentifier();

            TaxHistory history = new TaxHistory(
                identifier,
                [
                    new TaxPayment(2025, 1_000_000m)
                ]);

            CreditScoringCalculator calculator = new CreditScoringCalculator();

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(
                () => calculator.Calculate(
                    history,
                    requestedAmount));
        }
        
        [Fact]
        public void TaxHistory_WithNoPayments_Throws()
        {
            ApplicantIdentifier identifier = CreateIdentifier();

            Assert.Throws<ArgumentException>(
                () => new TaxHistory(
                    identifier,
                    []));
        }
    }
}