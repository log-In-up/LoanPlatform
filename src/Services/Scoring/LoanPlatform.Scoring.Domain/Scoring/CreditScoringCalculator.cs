using LoanPlatform.Scoring.Domain.Entities;
using LoanPlatform.Scoring.Domain.Policies;

namespace LoanPlatform.Scoring.Domain.Scoring
{
    public sealed class CreditScoringCalculator
    {
        public CreditScoringResult Calculate(TaxHistory taxHistory, decimal requestedAmount)
        {
            ArgumentNullException.ThrowIfNull(taxHistory);

            if (requestedAmount <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(requestedAmount),
                    "Requested amount must be greater than zero.");
            }

            int taxActivityScore = CalculateTaxActivityScore(taxHistory, requestedAmount);

            int stabilityScore = CalculateStabilityScore(taxHistory);

            int growthScore = CalculateGrowthScore(taxHistory);

            int regularityScore = CalculateRegularityScore(taxHistory);

            int historyScore = CalculateHistoryScore(taxHistory);

            int totalScore =
                taxActivityScore +
                stabilityScore +
                growthScore +
                regularityScore +
                historyScore;

            int score = Math.Clamp(totalScore, 0, 1000);

            return new CreditScoringResult(
                score,
                CreditScoringPolicy.Decide(score));
        }

        private static int CalculateTaxActivityScore(TaxHistory history, decimal requestedAmount)
        {
            decimal averageTaxPayment = history.Payments.Average(x => x.Amount);

            decimal ratio = averageTaxPayment / requestedAmount;

            return (int)Math.Clamp(ratio * 300, 0, 300);
        }

        private static int CalculateStabilityScore(TaxHistory history)
        {
            if (history.Payments.Count < 2)
            {
                return 0;
            }

            decimal average = history.Payments.Average(x => x.Amount);

            if (average == 0)
            {
                return 0;
            }

            decimal deviations =
                history.Payments
                    .Select(x => Math.Abs(x.Amount - average))
                    .Average();

            decimal coefficient = deviations / average;

            return (int)Math.Clamp(250 * (1 - coefficient), 0, 250);
        }

        private static int CalculateGrowthScore(TaxHistory history)
        {
            if (history.Payments.Count < 2)
            {
                return 0;
            }

            List<TaxPayment> ordered =
                history.Payments
                    .OrderBy(x => x.Year)
                    .ToList();

            decimal first = ordered.First().Amount;
            decimal last = ordered.Last().Amount;

            if (first == 0)
            {
                return last > 0 ? 200 : 0;
            }

            decimal growth = (last - first) / first;

            return (int)Math.Clamp(growth * 200, 0, 200);
        }

        private static int CalculateRegularityScore(TaxHistory history)
        {
            int distinctYears =
                history.Payments
                    .Select(x => x.Year)
                    .Distinct()
                    .Count();

            return Math.Clamp(distinctYears * 50, 0, 150);
        }

        private static int CalculateHistoryScore(TaxHistory history)
        {
            int years =
                history.Payments
                    .Select(x => x.Year)
                    .Distinct()
                    .Count();

            return Math.Clamp(years * 25, 0, 100);
        }
    }
}