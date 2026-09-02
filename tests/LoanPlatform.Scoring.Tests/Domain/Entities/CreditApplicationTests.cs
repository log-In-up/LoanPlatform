using LoanPlatform.Scoring.Domain.Entities;
using LoanPlatform.Scoring.Domain.Enums;
using LoanPlatform.Scoring.Domain.ValueObjects;

namespace LoanPlatform.Scoring.Tests.Domain
{
    public class CreditApplicationTests
    {
        private static ApplicantIdentifier CreateIdentifier()
        {
            return new ApplicantIdentifier("123456789012");
        }

        private static CreditApplication CreateApplication()
        {
            return new CreditApplication(
                CreateIdentifier(),
                ApplicantType.IndividualEntrepreneur,
                10_000_000m,
                24);
        }
        
        [Fact]
        public void Constructor_WithValidData_CreatesApplication()
        {
            // Act
            CreditApplication application = CreateApplication();

            // Assert
            Assert.NotEqual(Guid.Empty, application.Id);
            Assert.Equal(
                ApplicantType.IndividualEntrepreneur,
                application.ApplicantType);

            Assert.Equal(10_000_000m, application.RequestedAmount);
            Assert.Equal(24, application.RequestedTermMonths);

            Assert.Equal(
                CreditApplicationStatus.Created,
                application.Status);

            Assert.Equal(
                CreditDecision.Pending,
                application.Decision);
        }
        
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100000)]
        public void Constructor_WithInvalidAmount_ThrowsArgumentOutOfRangeException(decimal amount)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new CreditApplication(
                    CreateIdentifier(),
                    ApplicantType.IndividualEntrepreneur,
                    amount,
                    24));
        }
        
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Constructor_WithInvalidTerm_ThrowsArgumentOutOfRangeException(int termMonths)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new CreditApplication(
                    CreateIdentifier(),
                    ApplicantType.IndividualEntrepreneur,
                    10_000_000m,
                    termMonths));
        }
        
        [Fact]
        public void StartScoring_WhenApplicationIsCreated_ChangesStatusToScoringInProgress()
        {
            // Arrange
            CreditApplication application = CreateApplication();

            // Act
            application.StartScoring();

            // Assert
            Assert.Equal(
                CreditApplicationStatus.ScoringInProgress,
                application.Status);
        }
        
        [Fact]
        public void StartScoring_WhenScoringIsAlreadyInProgress_Throws()
        {
            // Arrange
            CreditApplication application = CreateApplication();

            application.StartScoring();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => application.StartScoring());
        }
        
        [Fact]
        public void CompleteScoring_WhenScoringIsInProgress_ChangesStatusToScored()
        {
            // Arrange
            CreditApplication application = CreateApplication();

            application.StartScoring();

            // Act
            application.CompleteScoring(CreditDecision.PreApproved);

            // Assert
            Assert.Equal(CreditApplicationStatus.Scored, application.Status);

            Assert.Equal(CreditDecision.PreApproved, application.Decision);
        }
        
        [Fact]
        public void CompleteScoring_WhenApplicationIsCreated_Throws()
        {
            // Arrange
            CreditApplication application = CreateApplication();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(
                () => application.CompleteScoring(
                    CreditDecision.PreApproved));
        }
        
        [Fact]
        public void MarkScoringFailed_WhenScoringIsInProgress_MarksApplicationAsFailed()
        {
            // Arrange
            CreditApplication application = CreateApplication();

            application.StartScoring();

            // Act
            application.MarkScoringFailed();

            // Assert
            Assert.Equal(
                CreditApplicationStatus.Failed,
                application.Status);

            Assert.Equal(
                CreditDecision.Pending,
                application.Decision);
        }
        
        [Fact]
        public void MarkScoringFailed_WhenApplicationIsCreated_Throws()
        {
            // Arrange
            CreditApplication application = CreateApplication();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(
                () => application.MarkScoringFailed());
        }
    }
}