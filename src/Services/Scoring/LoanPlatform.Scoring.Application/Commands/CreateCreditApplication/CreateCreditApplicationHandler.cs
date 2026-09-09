using LoanPlatform.Scoring.Application.Abstractions;
using LoanPlatform.Scoring.Domain.Entities;
using LoanPlatform.Scoring.Domain.ValueObjects;

namespace LoanPlatform.Scoring.Application.Commands.CreateCreditApplication
{
    public sealed class CreateCreditApplicationHandler
    {
        private readonly ICreditApplicationRepository _repository;

        public CreateCreditApplicationHandler(ICreditApplicationRepository repository)
        {
            _repository = repository;
        }

        public async Task<CreateCreditApplicationResult> HandleAsync(
            CreateCreditApplicationCommand command,
            CancellationToken cancellationToken)
        {
            ApplicantIdentifier applicantIdentifier = new(command.ApplicantIdentifier);

            CreditApplication application = new(
                applicantIdentifier,
                command.ApplicantType,
                command.RequestedAmount,
                command.RequestedTermMonths);

            await _repository.AddAsync(application, cancellationToken);

            await _repository.SaveAsync(cancellationToken);

            return new CreateCreditApplicationResult(
                application.Id,
                application.ApplicantIdentifier.Value,
                application.ApplicantType,
                application.RequestedAmount,
                application.RequestedTermMonths,
                application.Status,
                application.Decision);
        }
    }
}