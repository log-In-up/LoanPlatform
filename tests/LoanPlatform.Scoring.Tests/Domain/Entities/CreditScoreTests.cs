using LoanPlatform.Scoring.Domain.Entities;
using LoanPlatform.Scoring.Domain.Enums;

namespace LoanPlatform.Scoring.Tests.Domain
{
    public class CreditScoreTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(500)]
        [InlineData(600)]
        [InlineData(1000)]
        public void Constructor_WithValidScore_CreatesCreditScore(int score)
        {
            // Arrange
            Guid applicationId = Guid.NewGuid();

            // Act
            CreditScore creditScore = new CreditScore(
                applicationId,
                score,
                CreditDecision.Pending);

            // Assert
            Assert.Equal(score, creditScore.Score);
            Assert.Equal(applicationId, creditScore.ApplicationId);
        }
        
        [Theory]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Constructor_WithNegativeScore_ThrowsArgumentOutOfRangeException(int score)
        {
            Guid applicationId = Guid.NewGuid();

            Assert.Throws<ArgumentOutOfRangeException>(
                () => new CreditScore(
                    applicationId,
                    score,
                    CreditDecision.Pending));
        }
        
        [Theory]
        [InlineData(1001)]
        [InlineData(1500)]
        public void Constructor_WithScoreGreaterThan1000_ThrowsArgumentOutOfRangeException(int score)
        {
            Guid applicationId = Guid.NewGuid();

            Assert.Throws<ArgumentOutOfRangeException>(
                () => new CreditScore(
                    applicationId,
                    score,
                    CreditDecision.Pending));
        }
        
        [Fact]
        public void Constructor_WithEmptyApplicationId_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(
                () => new CreditScore(
                    Guid.Empty,
                    700,
                    CreditDecision.PreApproved));
        }
    }
}