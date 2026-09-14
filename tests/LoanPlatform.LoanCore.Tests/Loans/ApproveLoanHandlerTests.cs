using LoanPlatform.Contracts.Events;
using LoanPlatform.LoanCore.Application.Common;
using LoanPlatform.LoanCore.Application.Loans;
using LoanPlatform.LoanCore.Application.Loans.ApproveLoan;
using LoanPlatform.LoanCore.Domain.Loans;
using Moq;

namespace LoanPlatform.LoanCore.Tests.Loans
{
    public class ApproveLoanHandlerTests
    {
        [Fact]
        public async Task HandleAsync_ShouldApproveLoanAndPublishEvent()
        {
            // Arrange
            string applicantIdentifier = "TEST123456";
            decimal principalAmount = 100000m;

            Loan loan = new(
                applicantIdentifier,
                principalAmount,
                12m,
                12,
                PaymentType.Annuity);

            Guid loanId = loan.Id;

            Mock<ILoanRepository> loanRepository = new();
            Mock<ILoanApprovedPublisher> publisher = new();

            loanRepository
                .Setup(x => x.GetByIdAsync(
                    loanId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(loan);

            LoanApprovedEvent? publishedEvent = null;

            publisher
                .Setup(x => x.PublishAsync(
                    It.IsAny<LoanApprovedEvent>(),
                    It.IsAny<CancellationToken>()))
                .Callback<LoanApprovedEvent, CancellationToken>(
                    (@event, _) => publishedEvent = @event)
                .Returns(Task.CompletedTask);

            ApproveLoanHandler handler = new(loanRepository.Object, publisher.Object);

            ApproveLoanCommand command = new(loanId);

            // Act
            bool result = await handler.HandleAsync(command);

            // Assert
            Assert.True(result);
            Assert.Equal(LoanStatus.Approved, loan.Status);

            loanRepository.Verify(
                x => x.UpdateAsync(
                    loan,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            publisher.Verify(
                x => x.PublishAsync(
                    It.IsAny<LoanApprovedEvent>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            Assert.NotNull(publishedEvent);
            Assert.Equal(loanId, publishedEvent!.LoanId);
            Assert.Equal(applicantIdentifier, publishedEvent.ApplicantIdentifier);
            Assert.Equal(principalAmount, publishedEvent.PrincipalAmount);
        }
    }
}