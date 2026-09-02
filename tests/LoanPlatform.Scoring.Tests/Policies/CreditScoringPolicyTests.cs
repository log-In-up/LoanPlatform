using LoanPlatform.Scoring.Domain.Enums;
using LoanPlatform.Scoring.Domain.Policies;

namespace LoanPlatform.Scoring.Tests.Policies
{
    public class CreditScoringPolicyTests
    {
        [Theory]
        [InlineData(0, CreditDecision.Rejected)]
        [InlineData(500, CreditDecision.Rejected)]
        [InlineData(600, CreditDecision.Rejected)]
        [InlineData(601, CreditDecision.PreApproved)]
        [InlineData(700, CreditDecision.PreApproved)]
        [InlineData(1000, CreditDecision.PreApproved)]
        public void Decide_ReturnsExpectedDecision(int score, CreditDecision expected)
        {
            // Act
            CreditDecision result = CreditScoringPolicy.Decide(score);

            // Assert
            Assert.Equal(expected, result);
        }
        
        [Theory]
        [InlineData(-1)]
        [InlineData(-100)]
        [InlineData(1001)]
        [InlineData(5000)]
        public void Decide_WithInvalidScore_ThrowsArgumentOutOfRangeException(int score)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => CreditScoringPolicy.Decide(score));
        }
    }
}