using LoanPlatform.LoanCore.Application.Loans.Schedule;
using LoanPlatform.LoanCore.Domain.Loans;

namespace LoanPlatform.LoanCore.Tests.Schedule
{
    public class LoanScheduleCalculatorTests
    {
        private readonly LoanScheduleCalculator _calculator = new();

        [Fact]
        public void Calculate_Annuity_ReturnsCorrectNumberOfPayments()
        {
            const decimal principalAmount = 1_000_000m;
            const decimal annualInterestRate = 12m;
            const int termMonths = 12;
    
            IReadOnlyList<PaymentScheduleItem> schedule = _calculator.Calculate(
                principalAmount,
                annualInterestRate,
                termMonths,
                PaymentType.Annuity,
                new DateTime(2026, 10, 1));
    
            Assert.Equal(termMonths, schedule.Count);
        }

        [Fact]
        public void Calculate_Annuity_LastPaymentClosesPrincipal()
        {
            const decimal principalAmount = 1_000_000m;
            const decimal annualInterestRate = 12m;
            const int termMonths = 12;
    
            IReadOnlyList<PaymentScheduleItem> schedule = _calculator.Calculate(
                principalAmount,
                annualInterestRate,
                termMonths,
                PaymentType.Annuity,
                new DateTime(2026, 10, 1));
    
            PaymentScheduleItem lastPayment = schedule[^1];
    
            Assert.Equal(0m, lastPayment.RemainingPrincipal);
        }

        [Fact]
        public void Calculate_Annuity_PaymentsAreApproximatelyEqual()
        {
            const decimal principalAmount = 1_000_000m;
            const decimal annualInterestRate = 12m;
            const int termMonths = 12;
    
            IReadOnlyList<PaymentScheduleItem> schedule = _calculator.Calculate(
                principalAmount,
                annualInterestRate,
                termMonths,
                PaymentType.Annuity,
                new DateTime(2026, 10, 1));
    
            decimal firstPayment = schedule[0].PaymentAmount;
            decimal lastPayment = schedule[^1].PaymentAmount;
    
            Assert.InRange(
                Math.Abs(firstPayment - lastPayment),
                0m,
                0.02m);
        }

        [Fact]
        public void Calculate_Differentiated_ReturnsCorrectNumberOfPayments()
        {
            const decimal principalAmount = 1_000_000m;
            const decimal annualInterestRate = 12m;
            const int termMonths = 12;
    
            IReadOnlyList<PaymentScheduleItem> schedule = _calculator.Calculate(
                principalAmount,
                annualInterestRate,
                termMonths,
                PaymentType.Differentiated,
                new DateTime(2026, 10, 1));
    
            Assert.Equal(termMonths, schedule.Count);
        }

        [Fact]
        public void Calculate_Differentiated_LastPaymentClosesPrincipal()
        {
            const decimal principalAmount = 1_000_000m;
            const decimal annualInterestRate = 12m;
            const int termMonths = 12;
    
            IReadOnlyList<PaymentScheduleItem> schedule = _calculator.Calculate(
                principalAmount,
                annualInterestRate,
                termMonths,
                PaymentType.Differentiated,
                new DateTime(2026, 10, 1));
    
            PaymentScheduleItem lastPayment = schedule[^1];
    
            Assert.Equal(0m, lastPayment.RemainingPrincipal);
        }

        [Fact]
        public void Calculate_Differentiated_PaymentsDecreaseOverTime()
        {
            const decimal principalAmount = 1_000_000m;
            const decimal annualInterestRate = 12m;
            const int termMonths = 12;
    
            IReadOnlyList<PaymentScheduleItem> schedule = _calculator.Calculate(
                principalAmount,
                annualInterestRate,
                termMonths,
                PaymentType.Differentiated,
                new DateTime(2026, 10, 1));
    
            for (int i = 1; i < schedule.Count; i++)
            {
                Assert.True(
                    schedule[i].PaymentAmount < schedule[i - 1].PaymentAmount);
            }
        }

        [Fact]
        public void Calculate_ZeroInterest_Annuity_DividesPrincipalEqually()
        {
            const decimal principalAmount = 1_200_000m;
            const decimal annualInterestRate = 0m;
            const int termMonths = 12;
    
            IReadOnlyList<PaymentScheduleItem> schedule = _calculator.Calculate(
                principalAmount,
                annualInterestRate,
                termMonths,
                PaymentType.Annuity,
                new DateTime(2026, 10, 1));
    
            foreach (PaymentScheduleItem payment in schedule)
            {
                Assert.Equal(100_000m, payment.PaymentAmount);
                Assert.Equal(0m, payment.InterestAmount);
            }
    
            Assert.Equal(0m, schedule[^1].RemainingPrincipal);
        }

        [Fact]
        public void Calculate_InvalidPrincipal_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                _calculator.Calculate(
                    0m,
                    12m,
                    12,
                    PaymentType.Annuity,
                    new DateTime(2026, 10, 1)));
        }

        [Fact]
        public void Calculate_InvalidTerm_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                _calculator.Calculate(
                    1_000_000m,
                    12m,
                    0,
                    PaymentType.Annuity,
                    new DateTime(2026, 10, 1)));
        }
        
        [Fact]
        public void Calculate_Annuity_ReturnsExpectedMonthlyPayment()
        {
            const decimal principalAmount = 1_000_000m;
            const decimal annualInterestRate = 12m;
            const int termMonths = 12;

            IReadOnlyList<PaymentScheduleItem> schedule = _calculator.Calculate(
                principalAmount,
                annualInterestRate,
                termMonths,
                PaymentType.Annuity,
                new DateTime(2026, 10, 1));

            const decimal expectedPayment = 88_848.79m;

            Assert.All(
                schedule.Take(termMonths - 1),
                payment => Assert.Equal(
                    expectedPayment,
                    payment.PaymentAmount));
        }
        
        [Fact]
        public void Calculate_Differentiated_ReturnsExpectedFirstPayment()
        {
            const decimal principalAmount = 1_000_000m;
            const decimal annualInterestRate = 12m;
            const int termMonths = 12;

            IReadOnlyList<PaymentScheduleItem> schedule = _calculator.Calculate(
                principalAmount,
                annualInterestRate,
                termMonths,
                PaymentType.Differentiated,
                new DateTime(2026, 10, 1));

            const decimal expectedFirstPayment = 93_333.33m;

            Assert.Equal(
                expectedFirstPayment,
                schedule[0].PaymentAmount);
        }
    }
}