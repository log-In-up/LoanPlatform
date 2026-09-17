using LoanPlatform.Scoring.Application.Abstractions;
using LoanPlatform.Scoring.Domain.Entities;
using LoanPlatform.Scoring.Domain.Scoring;

namespace LoanPlatform.Scoring.Application.Commands.RunCreditScoring
{
    public sealed class RunCreditScoringHandler
    {
        private readonly ICreditApplicationRepository _repository;
        private readonly ICreditScoreRepository _creditScoreRepository;
        private readonly ITaxHistoryProvider _taxHistoryProvider;
        private readonly CreditScoringCalculator _calculator;
        private readonly IScoringUnitOfWork _unitOfWork;
        private readonly IScoringCache _cache;

        public RunCreditScoringHandler(
            ICreditApplicationRepository repository,
            ICreditScoreRepository creditScoreRepository,
            ITaxHistoryProvider taxHistoryProvider,
            CreditScoringCalculator calculator,
            IScoringUnitOfWork unitOfWork,
            IScoringCache cache)
        {
            _repository = repository;
            _creditScoreRepository = creditScoreRepository;
            _taxHistoryProvider = taxHistoryProvider;
            _calculator = calculator;
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<RunCreditScoringResult> HandleAsync(RunCreditScoringCommand command, CancellationToken cancellationToken)
        {
            CreditApplication? application = await _repository.GetByIdAsync(command.ApplicationId, cancellationToken);

            if (application is null)
            {
                throw new InvalidOperationException($"Credit application '{command.ApplicationId}' was not found.");
            }

            application.StartScoring();

            try
            {
                TaxHistory taxHistory = await _taxHistoryProvider.GetHistoryAsync(application.ApplicantIdentifier,
                        cancellationToken);

                CreditScoringResult result = _calculator.Calculate(taxHistory, application.RequestedAmount);

                application.CompleteScoring(result.Decision);

                CreditScore creditScore = new(application.Id, result.Score, result.Decision);

                await _creditScoreRepository.AddAsync(creditScore, cancellationToken);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await _cache.RemoveAsync(
                    $"credit-application:{application.Id}",
                    cancellationToken);

                await _cache.RemoveAsync(
                    $"credit-score:{application.Id}",
                    cancellationToken);

                return new RunCreditScoringResult(application.Id, result.Score, result.Decision);
            }
            catch
            {
                application.MarkScoringFailed();

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                throw;
            }
        }
    }
}