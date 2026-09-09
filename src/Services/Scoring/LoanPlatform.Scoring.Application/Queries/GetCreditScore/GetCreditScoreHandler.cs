using LoanPlatform.Scoring.Application.Abstractions;
using LoanPlatform.Scoring.Domain.Entities;

namespace LoanPlatform.Scoring.Application.Queries.GetCreditScore
{
    public sealed class GetCreditScoreHandler
    {
        private readonly ICreditScoreRepository _repository;

        public GetCreditScoreHandler(ICreditScoreRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetCreditScoreResult?> HandleAsync(
            GetCreditScoreQuery query,
            CancellationToken cancellationToken)
        {
            CreditScore? creditScore =
                await _repository.GetByApplicationIdAsync(
                    query.ApplicationId,
                    cancellationToken);

            if (creditScore is null)
            {
                return null;
            }

            return new GetCreditScoreResult(
                creditScore.ApplicationId,
                creditScore.Score,
                creditScore.Decision,
                creditScore.CalculatedAt);
        }
    }
}