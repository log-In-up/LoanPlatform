using LoanPlatform.Scoring.Application.Abstractions;
using LoanPlatform.Scoring.Domain.Entities;

namespace LoanPlatform.Scoring.Application.Queries.GetCreditScore
{
    public sealed class GetCreditScoreHandler
    {
        private readonly ICreditScoreRepository _repository;
        private readonly IScoringCache _cache;

        public GetCreditScoreHandler(
            ICreditScoreRepository repository,
            IScoringCache cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<GetCreditScoreResult?> HandleAsync(
            GetCreditScoreQuery query,
            CancellationToken cancellationToken)
        {
            string cacheKey = $"credit-score:{query.ApplicationId}";

            GetCreditScoreResult? cachedResult =
                await _cache.GetAsync<GetCreditScoreResult>(
                    cacheKey,
                    cancellationToken);

            if (cachedResult is not null)
            {
                return cachedResult;
            }

            CreditScore? creditScore =
                await _repository.GetByApplicationIdAsync(
                    query.ApplicationId,
                    cancellationToken);

            if (creditScore is null)
            {
                return null;
            }

            GetCreditScoreResult result = new(
                creditScore.ApplicationId,
                creditScore.Score,
                creditScore.Decision,
                creditScore.CalculatedAt);

            await _cache.SetAsync(
                cacheKey,
                result,
                TimeSpan.FromMinutes(5),
                cancellationToken);

            return result;
        }
    }
}