using LoanPlatform.Scoring.Application.Abstractions;
using LoanPlatform.Scoring.Domain.Entities;
using LoanPlatform.Scoring.Domain.Scoring;

namespace LoanPlatform.Scoring.Application.Commands.RunCreditScoring
{
    public sealed class RunCreditScoringHandler
    {
        private readonly ICreditApplicationRepository _repository;
        private readonly ITaxHistoryProvider _taxHistoryProvider;
        private readonly CreditScoringCalculator _calculator;

        public RunCreditScoringHandler(
            ICreditApplicationRepository repository,
            ITaxHistoryProvider taxHistoryProvider,
            CreditScoringCalculator calculator)
        {
            _repository = repository;
            _taxHistoryProvider = taxHistoryProvider;
            _calculator = calculator;
        }

        public async Task<RunCreditScoringResult> HandleAsync(RunCreditScoringCommand command, CancellationToken cancellationToken)
        {
            CreditApplication? application = await _repository.GetByIdAsync(command.ApplicationId, cancellationToken);

            if (application is null)
            {
                throw new InvalidOperationException(
                    $"Credit application '{command.ApplicationId}' was not found.");
            }

            application.StartScoring();

            try
            {
                TaxHistory taxHistory =
                    await _taxHistoryProvider.GetHistoryAsync(
                        application.ApplicantIdentifier,
                        cancellationToken);

                CreditScoringResult result =
                    _calculator.Calculate(
                        taxHistory,
                        application.RequestedAmount);

                application.CompleteScoring(result.Decision);

                await _repository.SaveAsync(
                    cancellationToken);

                return new RunCreditScoringResult(
                    application.Id,
                    result.Score,
                    result.Decision);
            }
            catch
            {
                application.MarkScoringFailed();

                await _repository.SaveAsync(
                    cancellationToken);

                throw;
            }
        }
    }
}