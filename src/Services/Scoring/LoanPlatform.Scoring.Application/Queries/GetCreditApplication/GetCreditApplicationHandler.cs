using LoanPlatform.Scoring.Application.Abstractions;
using LoanPlatform.Scoring.Domain.Entities;

namespace LoanPlatform.Scoring.Application.Queries.GetCreditApplication
{
    public class GetCreditApplicationHandler
    {
        private readonly ICreditApplicationRepository _repository;
        private readonly IScoringCache _cache;

        public GetCreditApplicationHandler(
            ICreditApplicationRepository repository,
            IScoringCache cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<GetCreditApplicationResult?> HandleAsync(
            GetCreditApplicationQuery query,
            CancellationToken cancellationToken)
        {
            string cacheKey = $"credit-application:{query.ApplicationId}";

            GetCreditApplicationResult? cachedResult =
                await _cache.GetAsync<GetCreditApplicationResult>(
                    cacheKey,
                    cancellationToken);

            if (cachedResult is not null)
            {
                return cachedResult;
            }

            CreditApplication? application = await _repository.GetByIdAsync(
                query.ApplicationId,
                cancellationToken);

            if (application is null)
            {
                return null;
            }

            GetCreditApplicationResult result = new(
                application.Id,
                application.ApplicantIdentifier.Value,
                application.ApplicantType,
                application.RequestedAmount,
                application.RequestedTermMonths,
                application.Status,
                application.Decision,
                application.CreatedAt);

            await _cache.SetAsync(cacheKey,
                result,
                TimeSpan.FromMinutes(5),
                cancellationToken);

            return result;
        }
    }
}