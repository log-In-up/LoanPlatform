using LoanPlatform.Scoring.Application.Abstractions;
using LoanPlatform.Scoring.Domain.Entities;

namespace LoanPlatform.Scoring.Application.Queries.GetCreditApplication
{
    public class GetCreditApplicationHandler
    {
        private readonly ICreditApplicationRepository _repository;

        public GetCreditApplicationHandler(ICreditApplicationRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetCreditApplicationResult?> HandleAsync(
            GetCreditApplicationQuery query,
            CancellationToken cancellationToken)
        {
            CreditApplication? application = await _repository.GetByIdAsync(
                query.ApplicationId,
                cancellationToken);

            if (application is null)
            {
                return null;
            }

            return new GetCreditApplicationResult(
                application.Id,
                application.ApplicantIdentifier.Value,
                application.ApplicantType,
                application.RequestedAmount,
                application.RequestedTermMonths,
                application.Status,
                application.Decision,
                application.CreatedAt);
        }
    }
}