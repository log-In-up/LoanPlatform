using LoanPlatform.LoanCore.Domain.Loans;

namespace LoanPlatform.LoanCore.Application.Loans.Schedule
{
    public sealed class LoanScheduleCalculator
    {
        public IReadOnlyList<PaymentScheduleItem> Calculate(
            decimal principalAmount,
            decimal annualInterestRate,
            int termMonths,
            PaymentType paymentType,
            DateTime firstPaymentDate)
        {
            if (principalAmount <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(principalAmount),
                    "Principal amount must be greater than zero.");
            }

            if (annualInterestRate < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(annualInterestRate),
                    "Interest rate cannot be negative.");
            }

            if (termMonths <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(termMonths),
                    "Term must be greater than zero.");
            }

            return paymentType switch
            {
                PaymentType.Annuity => CalculateAnnuity(
                    principalAmount,
                    annualInterestRate,
                    termMonths,
                    firstPaymentDate),

                PaymentType.Differentiated => CalculateDifferentiated(
                    principalAmount,
                    annualInterestRate,
                    termMonths,
                    firstPaymentDate),

                _ => throw new ArgumentOutOfRangeException(
                    nameof(paymentType),
                    paymentType,
                    "Unsupported payment type.")
            };
        }

        private static IReadOnlyList<PaymentScheduleItem> CalculateAnnuity(
            decimal principalAmount,
            decimal annualInterestRate,
            int termMonths,
            DateTime firstPaymentDate)
        {
            List<PaymentScheduleItem> schedule = new List<PaymentScheduleItem>(termMonths);

            decimal monthlyRate = annualInterestRate
                                  / LoanScheduleConstants.PercentageBase
                                  / LoanScheduleConstants.MonthsPerYear;

            decimal paymentAmount = monthlyRate == 0
                ? principalAmount / termMonths
                : principalAmount
                  * monthlyRate
                  * (decimal)Math.Pow(
                      (double)(1m + monthlyRate),
                      termMonths)
                  / ((decimal)Math.Pow(
                      (double)(1m + monthlyRate),
                      termMonths) - 1m);

            decimal remainingPrincipal = principalAmount;

            for (int paymentNumber = 1; paymentNumber <= termMonths; paymentNumber++)
            {
                decimal interestAmount = remainingPrincipal * monthlyRate;
                decimal principalPart = paymentAmount - interestAmount;

                if (paymentNumber == termMonths)
                {
                    principalPart = remainingPrincipal;
                    paymentAmount = principalPart + interestAmount;
                }

                remainingPrincipal -= principalPart;

                if (remainingPrincipal < 0)
                {
                    remainingPrincipal = 0;
                }

                schedule.Add(new PaymentScheduleItem(
                    paymentNumber,
                    firstPaymentDate.AddMonths(paymentNumber - 1),
                    decimal.Round(paymentAmount, 2),
                    decimal.Round(principalPart, 2),
                    decimal.Round(interestAmount, 2),
                    decimal.Round(remainingPrincipal, 2)));
            }

            return schedule;
        }

        private static IReadOnlyList<PaymentScheduleItem> CalculateDifferentiated(
            decimal principalAmount,
            decimal annualInterestRate,
            int termMonths,
            DateTime firstPaymentDate)
        {
            List<PaymentScheduleItem> schedule = new List<PaymentScheduleItem>(termMonths);

            decimal monthlyRate = annualInterestRate / 100m / 12m;
            decimal principalPart = principalAmount / termMonths;
            decimal remainingPrincipal = principalAmount;

            for (int paymentNumber = 1; paymentNumber <= termMonths; paymentNumber++)
            {
                decimal interestAmount = remainingPrincipal * monthlyRate;

                decimal currentPrincipalPart = paymentNumber == termMonths
                    ? remainingPrincipal
                    : principalPart;

                decimal paymentAmount = currentPrincipalPart + interestAmount;

                remainingPrincipal -= currentPrincipalPart;

                if (remainingPrincipal < 0)
                {
                    remainingPrincipal = 0;
                }

                schedule.Add(new PaymentScheduleItem(
                    paymentNumber,
                    firstPaymentDate.AddMonths(paymentNumber - 1),
                    decimal.Round(paymentAmount, 2),
                    decimal.Round(currentPrincipalPart, 2),
                    decimal.Round(interestAmount, 2),
                    decimal.Round(remainingPrincipal, 2)));
            }

            return schedule;
        }
    }
}