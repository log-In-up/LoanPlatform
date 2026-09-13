using LoanPlatform.LoanCore.Domain.Loans;

namespace LoanPlatform.LoanCore.Tests
{
    public class LoanTests
    {
        [Fact]
        public void Constructor_ShouldCreatePendingLoan()
        {
            // Arrange & Act
            Loan loan = new Loan(
                "123456789012",
                1_000_000m,
                18m,
                24,
                PaymentType.Annuity);
    
            // Assert
            Assert.NotEqual(Guid.Empty, loan.Id);
            Assert.Equal("123456789012", loan.ApplicantIdentifier);
            Assert.Equal(1_000_000m, loan.PrincipalAmount);
            Assert.Equal(18m, loan.InterestRate);
            Assert.Equal(24, loan.TermMonths);
            Assert.Equal(PaymentType.Annuity, loan.PaymentType);
            Assert.Equal(LoanStatus.Pending, loan.Status);
        }
    
        [Fact]
        public void Approve_ShouldChangeStatusToApproved()
        {
            Loan loan = CreateLoan();
    
            loan.Approve();
    
            Assert.Equal(LoanStatus.Approved, loan.Status);
        }

        [Fact]
        public void Activate_ShouldChangeApprovedLoanToActive()
        {
            Loan loan = CreateLoan();
    
            loan.Approve();
            loan.Activate();
    
            Assert.Equal(LoanStatus.Active, loan.Status);
        }
    
        [Fact]
        public void Close_ShouldChangeActiveLoanToClosed()
        {
            Loan loan = CreateLoan();
    
            loan.Approve();
            loan.Activate();
            loan.Close();
    
            Assert.Equal(LoanStatus.Closed, loan.Status);
        }

        [Fact]
        public void Reject_ShouldChangePendingLoanToRejected()
        {
            Loan loan = CreateLoan();
    
            loan.Reject();
    
            Assert.Equal(LoanStatus.Rejected, loan.Status);
        }
    
        [Fact]
        public void Approve_ShouldThrow_WhenLoanIsNotPending()
        {
            Loan loan = CreateLoan();
    
            loan.Approve();
            loan.Activate();
    
            Assert.Throws<InvalidOperationException>(
                () => loan.Approve());
        }

        [Fact]
        public void Activate_ShouldThrow_WhenLoanIsNotApproved()
        {
            Loan loan = CreateLoan();
    
            Assert.Throws<InvalidOperationException>(
                () => loan.Activate());
        }
    
        [Fact]
        public void Close_ShouldThrow_WhenLoanIsNotActive()
        {
            Loan loan = CreateLoan();
    
            Assert.Throws<InvalidOperationException>(
                () => loan.Close());
        }
    
        [Fact]
        public void Constructor_ShouldThrow_WhenAmountIsInvalid()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new Loan(
                    "123456789012",
                    0m,
                    18m,
                    24,
                    PaymentType.Annuity));
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenTermIsInvalid()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new Loan(
                    "123456789012",
                    1_000_000m,
                    18m,
                    0,
                    PaymentType.Annuity));
        }

        private static Loan CreateLoan()
        {
            return new Loan(
                "123456789012",
                1_000_000m,
                18m,
                24,
                PaymentType.Annuity);
        }
    }
}